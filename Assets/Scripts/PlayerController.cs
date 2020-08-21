using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public float gravity;
    private Vector3 moveDir = Vector3.zero;
    public CharacterController controller;

    protected Joystick joystick;
    protected JoyButton joyButton;


    void Start()
    {   
        // collects proper gameobjects needed
        if (!controller) controller = gameObject.GetComponent<CharacterController>();
        Joystick[] tempj = FindObjectsOfType<Joystick>();
        foreach (Joystick j in tempj)
        {
            if (j.CompareTag("MovementJoystick")) joystick = j;
        }
        JoyButton[] tempb = FindObjectsOfType<JoyButton>();
        foreach (JoyButton b in tempb)
        {
            if (b.CompareTag("JumpButton")) joyButton = b;
        }
    }

    void Update()
    {

        if (controller.isGrounded)
        {
            // defines players physics
            moveDir = new Vector3(joystick.Horizontal + Input.GetAxis("Horizontal"), 0, joystick.Vertical + Input.GetAxis("Vertical"));
            moveDir = transform.TransformDirection(moveDir);
            moveDir *= speed;

            if (joyButton.Pressed)
            {
                moveDir.y = jumpForce;
            }
        }
        // impliments player physics
        moveDir.y -= gravity * Time.deltaTime;
        controller.Move(moveDir * Time.deltaTime);
        // handles player rotation based on camera rotation
        transform.Rotate(0, (Camera.main.transform.rotation.y - transform.rotation.y), 0);
    }
}
