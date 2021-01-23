using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class FPSController : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float flyingSpeed = 30f;
    public float flyingSpeedFast = 50f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 90.0f;
    public bool flying = false;
    public bool colliding = true;
    public bool canLook = true;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            flying = !flying;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            playerCamera.orthographic = !playerCamera.orthographic;
            playerCamera.orthographicSize = 60;
            if (playerCamera.orthographic)
            {
                playerCamera.nearClipPlane = 0f;
            } else
            {
                playerCamera.nearClipPlane = 0.3f;
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            playerCamera.orthographicSize += 5;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            playerCamera.orthographicSize -= 5;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            colliding = !colliding;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            BRS.Instance.LoadBRS();
        }

        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            playerCamera.orthographicSize += 5;
        }
        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            playerCamera.orthographicSize -= 5;
        }
        if (Input.GetKey(KeyCode.Keypad6))
        {
            transform.rotation *= Quaternion.Euler(0, 0.1f, 0);
        }
        if (Input.GetKey(KeyCode.Keypad4))
        {
            transform.rotation *= Quaternion.Euler(0, -0.1f, 0);
        }

        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftControl);
        float movementDirectionY = moveDirection.y;
        if (!flying)
        {
            float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        } else
        {
            float curSpeedX = canMove ? (isRunning ? flyingSpeedFast : flyingSpeed) * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? (isRunning ? flyingSpeedFast : flyingSpeed) * Input.GetAxis("Horizontal") : 0;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        }
        
        if (Input.GetButton("Jump") && canMove && (characterController.isGrounded || flying))
        {
            moveDirection.y = jumpSpeed;
            if (flying)
            {
                moveDirection.y *= (isRunning?2:1);
            }
        }
        else if (flying && Input.GetKey(KeyCode.LeftShift))
        {
            moveDirection.y = -jumpSpeed * (isRunning?2:1);
        }
        else if (flying)
        {
            moveDirection.y = 0;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded && !flying)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove && canLook)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}