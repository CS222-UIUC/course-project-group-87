using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Transform playerCamera = null;
    [SerializeField] public float sensX = 10.0f;
    [SerializeField] public float sensY = 10.0f;
    [SerializeField] public float walkSpeed = 10.0f;
    [SerializeField] [Range(0.0f, 0.5f)] public float moveSmoothTime = 0.3f;
    [SerializeField] [Range(0.0f, 0.5f)] public float mouseSmoothTime = 0.3f;
    [SerializeField] private bool lockCursor = true;
    [SerializeField] private Camera cameraObject;
    [SerializeField] private Transform gCheck = null;
    [SerializeField] private float groundDistance;
    [SerializeField] private float JumpHeight;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchHeight;
    [SerializeField] private float standHeight;
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject particlePrefab;


    public static List<PlayerController> ActivePlayers = new List<PlayerController>();
    private int _score = 0;
    public LayerMask Ground;
    public LayerMask Interactable;
    private float _cameraPitch = 0.0f;
    private CharacterController _controller = null;
    private float _grav = -20.0f;
    private bool _isGrounded;
    private bool _isInteractable;
    private bool _crouching;
    private Vector3 velocity;

    private Vector2 _currentDir = Vector2.zero;
    private Vector2 _currentDirVelocity = Vector2.zero;
    
    private Vector2 _currentMouseDelta = Vector2.zero;
    private Vector2 _currentMouseDeltaVelocity = Vector2.zero;

    private NetworkVariable<int> randomNumber = new NetworkVariable<int>(0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        ActivePlayers.Add(this);
        randomNumber.OnValueChanged += (int previousValue, int newValue) =>
        {
            Debug.Log(OwnerClientId);
        };
        // get player controller
        _controller = GetComponent<CharacterController>();
        
        cameraObject.enabled = false;

        // lock cursor to window and hide
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // protected override void OnDestroy()
    // {
    //     ActivePlayers.Remove(this);
    // }

    // Update is called once per frame
    private void Update()
    {
        if (!IsOwner) return;
        cameraObject.enabled = true;
        _isGrounded = Physics.CheckSphere(gCheck.transform.position, groundDistance, Ground, QueryTriggerInteraction.Ignore);
        _isInteractable = Physics.CheckSphere(gCheck.transform.position, groundDistance, Interactable, QueryTriggerInteraction.Ignore);
        _crouching = Input.GetKey(KeyCode.LeftControl);
        if (Input.GetKeyDown(KeyCode.T))
        {
            // Debug.Log(_score);
            GameStateServerRpc();
            // randomNumber.Value += 1;
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            TestServerRpc();
        }
        UpdateMouseLook();
        UpdateMovement();
        //Debug.Log(playerCamera.transform.position);
    }

    private void FixedUpdate()
    {
        var desiredHeight = _crouching ? crouchHeight : standHeight;
        
        if (_controller.height != desiredHeight)
        {
            AdjustHeight(desiredHeight);
        }
    }

    private void AdjustHeight(float height)
    {
        float center = height / 2;

        var camPos = playerCamera.position;

        _controller.height = Mathf.Lerp(_controller.height, height, crouchSpeed);
        _controller.center = Vector3.Lerp(_controller.center, new Vector3(0, center, 0), crouchSpeed);

        transform.localScale = new Vector3(0.0f, _controller.height / 2, 0.0f);
        model.transform.localScale = new Vector3(0.0f, _controller.height / 2, 0.0f);
        // playerCamera.position = new Vector3(playerCamera.position.x, 0.99f * _controller.height, playerCamera.position.z);
        // Debug.Log(0.99f * _controller.height);
    }

    void UpdateMouseLook()
    {
        // mouse movement
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        _currentMouseDelta = Vector2.SmoothDamp(_currentMouseDelta, targetMouseDelta, ref _currentMouseDeltaVelocity,
            mouseSmoothTime);

        _cameraPitch -= _currentMouseDelta.y * sensY * Time.fixedDeltaTime;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -90.0f, 90.0f);

        playerCamera.localEulerAngles = Vector3.right * _cameraPitch;
        
        // rotate player by horizontal mouse input
        transform.Rotate(Vector3.up * (_currentMouseDelta.x * sensX * Time.fixedDeltaTime));
        
        
    }

    void UpdateMovement()
    {
        if ((_isGrounded || _isInteractable) && velocity.y < 0)
        {
            velocity.y = 0f;
        }
        var targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        _currentDir = Vector2.SmoothDamp(_currentDir, targetDir, ref _currentDirVelocity, moveSmoothTime);

        var move = (transform.forward * _currentDir.y + transform.right * _currentDir.x) * walkSpeed;
        _controller.Move(move * Time.deltaTime);
        
        velocity.y += _grav*Time.deltaTime;
        if (Input.GetKeyDown("space") && (_isGrounded || _isInteractable))
        {
            velocity.y = Mathf.Sqrt(JumpHeight * -2f * _grav);
        }

        _controller.Move(velocity * Time.deltaTime);
    }
    
    [ServerRpc]
    private void GameStateServerRpc()
    {
        if (!IsHost)
        {
            return;
        }
        // _score++;
        // Debug.Log("SCORE: " + _score);
        // if (_score > 10)
        // {
        //     Debug.Log("YOU WIN");
        // }
        // randomNumber.Value += 1;
        foreach (PlayerController p in ActivePlayers)
        {
            p.GameStateClientRpc();
        }
    }

    [ServerRpc]
    private void TestServerRpc()
    {
        Debug.Log(OwnerClientId + "CALLED FUNCTION");
        TestClientRpc();
    }

    [ClientRpc]
    private void TestClientRpc()
    {
        Debug.Log("test");
        Instantiate(particlePrefab, transform.position, transform.rotation);
    }

    [ClientRpc]
    private void GameStateClientRpc()
    {
        _score++;
        Debug.Log(_score);
        if (_score >= 10)
        {
            Debug.Log("YOU WIN");
        }

    }
    
}
