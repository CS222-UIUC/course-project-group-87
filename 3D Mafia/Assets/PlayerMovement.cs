using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    private float movementSpeed = 10f;
    public float walkSpeed = 10f;
    public float sprintSpeed = 30f;

    public KeyCode sprintKey = KeyCode.LeftShift;

    public float gravity = -9.81f;
    public float jumpHeight = 20f;
    // public float sprintingMultiplier = 4f;

    public Transform groundCheck;
    public float groundDistance = 1.5f;
    public LayerMask groundMask;

    Vector3 velocity;
    public bool isGrounded;
    public bool isJumping;
    public bool isSprinting;

    // We don't need Start method

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }

        float x_coor = Input.GetAxis("Horizontal");
        float z_coor = Input.GetAxis("Vertical");

        isJumping = Input.GetButtonDown("Jump") && isGrounded;

        // default "Jump" maps to space key
        if (isJumping) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        isSprinting = Input.GetKey(sprintKey) && isGrounded && !isJumping;

        if (isSprinting) {
            // quadruple movement speed if sprinting
            movementSpeed = sprintSpeed;
        } else {
            movementSpeed = walkSpeed;
        }

        Vector3 move = transform.right * x_coor + transform.forward * z_coor;
        controller.Move(move * movementSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}