using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static float playerSpeed = 15f;
    public float speed;
    public float jumpForce;
    public float gravity;
    public float alertSpeed;
    public float turnSmoothTime = 0.1f;
    public CharacterController controller;
    public Camera thirdPersonCamera;
    public Camera firstPersonCamera;
    public ThirdPersonCameraController thirdPersonCameraController;
    public FirstPersonCameraController firstPersonCameraController;
    [HideInInspector]
    public bool headOnObject;


    protected Joystick joystick;
    public JoyButton jumpJoyButton;
    private Vector3 moveDir = Vector3.zero;
    private float alertLevel = 0f;


    float yspeed = 0f;

    void Start()
    {
        playerSpeed = speed;
        thirdPersonCamera.enabled = true;
        firstPersonCamera.enabled = false;
        thirdPersonCameraController.enabled = true;
        firstPersonCameraController.enabled = false;
        // collects proper gameobjects needed
        Joystick[] tempj = FindObjectsOfType<Joystick>();
        foreach (Joystick j in tempj)
        {
            if (j.CompareTag("MovementJoystick")) joystick = j;
        }
        if (!jumpJoyButton)
        {
            JoyButton[] tempb = FindObjectsOfType<JoyButton>();
            foreach (JoyButton b in tempb)
            {
                if (b.CompareTag("JumpButton")) jumpJoyButton = b;
            }
        }
    }

    void Update()
    {
        if (alertLevel >= 100f) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SwitchCamera();
        }
        if (controller.isGrounded)
        {
            yspeed = 0f;
            if (jumpJoyButton.Pressed)
            {
                yspeed = jumpForce;
            }
        }
        else
        {
            yspeed -= gravity * Time.deltaTime;
        }
        CollisionFlags flags = controller.Move(new Vector3(0f, yspeed, 0f));
        if (CollisionFlags.CollidedAbove == flags) yspeed = 0f;

        if (firstPersonCamera.enabled)
        {
            moveDir = new Vector3(joystick.Horizontal + Input.GetAxis("Horizontal"), 0, joystick.Vertical + Input.GetAxis("Vertical")).normalized;
            if (moveDir.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg + firstPersonCamera.transform.eulerAngles.y;
                Vector3 endDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

                controller.Move(endDir * playerSpeed * Time.deltaTime * moveDir.magnitude);
            }

        }
    }

    public void SwitchCamera()
    {
        thirdPersonCamera.enabled = !thirdPersonCamera.enabled;
        firstPersonCamera.enabled = !firstPersonCamera.enabled;
        thirdPersonCameraController.enabled = thirdPersonCamera.enabled;
        firstPersonCameraController.enabled = firstPersonCamera.enabled;
    }

    public void AddAlert()
    {
        alertLevel += alertSpeed * Time.deltaTime;
    }
    public float GetAlert()
    {
        return alertLevel;
    }
}