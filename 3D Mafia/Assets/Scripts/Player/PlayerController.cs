using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
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
    [SerializeField] private Camera CameraObject;



    private float _cameraPitch = 0.0f;
    private CharacterController _controller = null;

    private Vector2 _currentDir = Vector2.zero;
    private Vector2 _currentDirVelocity = Vector2.zero;
    
    private Vector2 _currentMouseDelta = Vector2.zero;
    private Vector2 _currentMouseDeltaVelocity = Vector2.zero;
    // Start is called before the first frame update
    void Start()
    {
        // get player controller
        _controller = GetComponent<CharacterController>();
        CameraObject.enabled = false;
        
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
    void Update()
    {
        if (!IsOwner) return;
        CameraObject.enabled = true;
        UpdateMouseLook();
        UpdateMovement();
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
        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        _currentDir = Vector2.SmoothDamp(_currentDir, targetDir, ref _currentDirVelocity, moveSmoothTime);
        
        Vector3 velocity = (transform.forward * _currentDir.y + transform.right * _currentDir.x) * walkSpeed;
        _controller.Move(velocity * Time.deltaTime);
    }
}
