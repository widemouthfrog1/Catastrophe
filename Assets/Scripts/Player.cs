using UnityEngine;
using UnityEngine.InputSystem;

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
    float xRotation = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
        //Cursor.lockState = CursorLockMode.Locked;
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        move = moveAction.ReadValue<Vector2>();
        look = lookAction.ReadValue<Vector2>();
        Look();
        Move();
    }

    private void Move()
    {
        Vector3 movement = transform.right * move.x + transform.forward * move.y;
        controller.Move(movement * speed * Time.deltaTime);
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
