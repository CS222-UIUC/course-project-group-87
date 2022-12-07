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
using UnityEngine.SceneManagement;
using Random = System.Random;

public class PlayerController : MonoBehaviour
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
    [SerializeField] private GameObject enemy;


    public static List<PlayerController> ActivePlayers = new List<PlayerController>();
    private int _score = 0;

    public float health = 2000f;
    
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

    // Start is called before the first frame update
    void Start()
    {
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

    // Update is called once per frame
    private void Update()
    {

        cameraObject.enabled = true;
        _isGrounded = Physics.CheckSphere(gCheck.transform.position, groundDistance, Ground, QueryTriggerInteraction.Ignore);
        _isInteractable = Physics.CheckSphere(gCheck.transform.position, groundDistance, Interactable, QueryTriggerInteraction.Ignore);
        _crouching = Input.GetKey(KeyCode.LeftControl);
        UpdateMouseLook();
        UpdateMovement();
        //Debug.Log(playerCamera.transform.position);
        if (Input.GetKeyDown("t"))
        {
            Instantiate(enemy);
        }

        if (Input.GetKeyDown("r")) {
            velocity = Vector3.up * 10;

            StartCoroutine(DeathWait());

            Die();
        }
    }

    IEnumerator DeathWait() {
        yield return new WaitForSeconds(3); 
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

    public void TakeDamage (float amount)
    {
        health -= amount;
        Debug.Log(health);

        if (health <= 0f) {
            Die();
        }
    }

    void Die() {
        //Destroy(gameObject);

        //gameObject.transform.position = new Vector3(-66, 0, 10);
        health = 2000f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
