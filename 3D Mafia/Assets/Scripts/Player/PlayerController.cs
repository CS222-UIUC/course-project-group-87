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
using Random = UnityEngine.Random;
using TMPro;

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
    //[SerializeField] private GameObject enemy;


    public static List<PlayerController> ActivePlayers = new List<PlayerController>();
    public int score = 0;

    public float health = 50f;
    public float restartDelay = 10f;

    public CharacterController cc;
    public Canvas c1;
    public Canvas c2;
    public Canvas c3;
    public Canvas deathCanvas;
    public Canvas scoreCanvas;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI scoreText;
    
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

    private Vector3 v1 = new Vector3(0.5999994f, -4.35f, 9.15f);
    private Vector3 v2 = new Vector3(-25.5f, 1.7f, 0f);
    private Vector3 v3 = new Vector3(13.05f, 28.66f, -3.51f);
    private Vector3 v4 = new Vector3(-16.61094f, -10.7f, 13.41543f);

    // Start is called before the first frame update
    void Start()
    {
        ActivePlayers.Add(this);
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

        //Instantiate(enemy);

        var r = Random.value;

        if (r <= 0.25) {
            this.transform.position = v1;
        } else if (r <= 0.50) {
            this.transform.position = v2;
        } else if (r <= 0.75) {
            this.transform.position = v3;
        } else {
            this.transform.position = v4;
        }
        
        scoreCanvas.sortingOrder = 1;
    }

    // Update is called once per frame
    private void Update()
    {

        cameraObject.enabled = true;
        _isGrounded = Physics.CheckSphere(gCheck.transform.position, groundDistance, Ground, QueryTriggerInteraction.Ignore);
        _isInteractable = Physics.CheckSphere(gCheck.transform.position, groundDistance, Interactable, QueryTriggerInteraction.Ignore);
        //_crouching = Input.GetKey(KeyCode.LeftControl);
        UpdateMouseLook();
        UpdateMovement();
        //Debug.Log(playerCamera.transform.position);
        
        if (Input.GetKeyDown("l")) {
            TakeDamage(10);
            Debug.Log("Player health is " + health);
        }

        healthText.SetText("Health: " + Mathf.Ceil(health));
        scoreText.SetText("Score: " + score);
    }

    IEnumerator DeathWait() {
        yield return new WaitForSeconds(5); 
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

        if (health <= 0f) {
            Invoke("Die", restartDelay);
            cc.enabled = false;
            c1.enabled = false;
            c2.enabled = false;
            c3.enabled = false;
            deathCanvas.enabled = true;
        }
    }

    void Die() {
        //Destroy(gameObject);

        //gameObject.transform.position = new Vector3(-66, 0, 10);
        health = 50f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("YOU DIED");
    }

}
