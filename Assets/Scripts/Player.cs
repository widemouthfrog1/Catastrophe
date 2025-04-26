using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    Vector2 move = Vector2.zero;
    Vector2 look = Vector2.zero;
    InputAction moveAction;
    CharacterController controller;
    public float speed = 10;
    InputAction lookAction;
    public float sensitivity = 10;
    new Camera camera;
    RectTransform jumpArrow;
    float jumpPower = 0;
    float xRotation = 0;
    bool jumping = false;
    bool isJumpSliderIncreasing;
    public float jumpStrength = 10;
    public float gravity = -3;
    public float pawSpeed = 0.1f;
    public float sizeConstant = 1;
    float pawSign = 1;
    Vector3 velocity = Vector3.zero;
    InputAction rubAction;

    InputAction jumpAction;

    Transform leftPaw;
    Transform rightPaw;

    float pawRotationAmount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
        jumpAction = InputSystem.actions.FindAction("Jump");
        rubAction = InputSystem.actions.FindAction("Rub");
        //Cursor.lockState = CursorLockMode.Locked;
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        jumpArrow = GameObject.Find("JumpArrow").GetComponent<RectTransform>();
        controller = GetComponent<CharacterController>();
        leftPaw = GameObject.Find("LeftPaw").transform;
        rightPaw = GameObject.Find("RightPaw").transform;
    }

    // Update is called once per frame
    void Update()
    {
        move = moveAction.ReadValue<Vector2>();
        look = lookAction.ReadValue<Vector2>();
        Look();
        if(controller.isGrounded && !jumpAction.WasReleasedThisFrame())
        {
            velocity = new Vector3(0, -1, 0);
        }
        
        Gravity();
        Jump();
        Move();
        
        controller.Move(velocity * Time.deltaTime);

        if(rubAction.IsPressed())
        {
            Rub();
        } else
        {
            //pawRotationAmount = 0;
        }
    }


    private void Rub()
    {
        
        if (pawRotationAmount < 0.5 && pawSign == 1)
        {
            leftPaw.transform.Rotate(Vector3.right, -22.5f * pawRotationAmount);
            rightPaw.transform.Rotate(Vector3.right, 22.5f * pawRotationAmount);
        }
        else if(pawRotationAmount >= 0.5 && pawSign == 1) 
        {
        
            leftPaw.transform.Rotate(Vector3.right, -22.5f * (1 - pawRotationAmount));
            rightPaw.transform.Rotate(Vector3.right, 22.5f * (1 - pawRotationAmount));
        }
        else if (pawRotationAmount > -0.5 && pawSign == -1)
        {
            leftPaw.transform.Rotate(Vector3.right, -22.5f * pawRotationAmount);
            rightPaw.transform.Rotate(Vector3.right, 22.5f * pawRotationAmount);
        }
        else if (pawRotationAmount <= -0.5 && pawSign == -1)
        {
            leftPaw.transform.Rotate(Vector3.right, -22.5f * (-1 - pawRotationAmount));
            rightPaw.transform.Rotate(Vector3.right, 22.5f * (-1 - pawRotationAmount));
        }

        pawRotationAmount += pawSign * pawSpeed * Time.deltaTime;
        if (pawRotationAmount > 1 || pawRotationAmount < -1)
        {
            pawRotationAmount = 0;
            pawSign *= -1;
        }

        if (pawRotationAmount < 0.5 && pawSign == 1)
        {
            leftPaw.transform.Rotate(Vector3.right, 22.5f * pawRotationAmount);
            rightPaw.transform.Rotate(Vector3.right, -22.5f * pawRotationAmount);
        }
        else if (pawRotationAmount >= 0.5 && pawSign == 1)
        {
            leftPaw.transform.Rotate(Vector3.right, 22.5f * (1 - pawRotationAmount));
            rightPaw.transform.Rotate(Vector3.right, -22.5f * (1 - pawRotationAmount));
        }
        else if (pawRotationAmount > -0.5 && pawSign == -1)
        {
            leftPaw.transform.Rotate(Vector3.right, 22.5f * pawRotationAmount);
            rightPaw.transform.Rotate(Vector3.right, -22.5f * pawRotationAmount);
        }
        else if (pawRotationAmount <= -0.5 && pawSign == -1)
        {
            leftPaw.transform.Rotate(Vector3.right, 22.5f * (-1 - pawRotationAmount));
            rightPaw.transform.Rotate(Vector3.right, -22.5f * (-1 - pawRotationAmount));
        }

    }


    private void Gravity()
    {
        velocity.y += gravity * Time.deltaTime;
    }

    private void Jump()
    {
        if (!jumping && jumpAction.WasPressedThisFrame() && controller.isGrounded)
        {
            jumping = true;
        }
        if(jumping)
        {
            jumpArrow.transform.Translate(new Vector3(0, -jumpPower * sizeConstant));
            if (isJumpSliderIncreasing)
            {
                jumpPower += 1.5f * Time.deltaTime;
            }
            else
            {
                jumpPower -= 1.5f * Time.deltaTime;
            }
            if (jumpPower >= 1)
            {
                jumpPower = 1;
                isJumpSliderIncreasing = false;
            }

            if (jumpPower <= 0)
            {
                jumpPower = 0;
                isJumpSliderIncreasing = true;
            }
            jumpArrow.transform.Translate(new Vector3(0, jumpPower*sizeConstant));
        }

        if (jumping && jumpAction.WasReleasedThisFrame())
        {
            jumpArrow.transform.Translate(new Vector3(0, -jumpPower * sizeConstant));
            velocity = camera.transform.forward * jumpPower * jumpStrength;
            jumpPower = 0;
            jumping = false;
        }

    }

    private void Move()
    {
        Vector3 movement = transform.right * move.x + transform.forward * move.y;
        if(!jumping && controller.isGrounded)
        {
            velocity += movement * speed;
        }
        
    }

    private void Look()
    {
        transform.Rotate(Vector3.up, look.x * sensitivity * Time.deltaTime);
        xRotation -= look.y * sensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -85, 85);
        Vector3 targetRotation = transform.eulerAngles;
        targetRotation.x = xRotation;
        camera.transform.eulerAngles = targetRotation;
    }

    private void FixedUpdate()
    {
        
    }
}
