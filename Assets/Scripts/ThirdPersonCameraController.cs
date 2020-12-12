using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ThirdPersonCameraController : MonoBehaviour
{
    public Transform cam;
    public CharacterController controller;
    protected Joystick joystick;

    public static float TouchSensitivity_x = 10f;
    public static float TouchSensitivity_y = 10f;
    public float turnSmoothTime = 0.1f;


    float turnSmoothVelocity;
    private Vector3 moveDir = Vector3.zero;
       
    void Start()
    {
        CinemachineCore.GetInputAxis = HandleAxisInputDelegate;
        if (!controller) controller = gameObject.GetComponent<CharacterController>();
        Joystick[] tempj = FindObjectsOfType<Joystick>();
        foreach (Joystick j in tempj)
        {
            if (j.CompareTag("MovementJoystick")) joystick = j;
        }

    }
    void Update()
    {
        moveDir = new Vector3(joystick.Horizontal + Input.GetAxis("Horizontal"), 0, joystick.Vertical + Input.GetAxis("Vertical")).normalized;
        if (moveDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 endDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(endDir.normalized * PlayerController.playerSpeed * Time.deltaTime * moveDir.magnitude);
        }
    }

    public static float HandleAxisInputDelegate(string axisName)
    {
        Rect r = new Rect(Screen.width / 2, 0, Screen.width / 2, Screen.height);
        switch (axisName)
        {

            case "Mouse X":

                if (Input.touchCount > 0)
                {
                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        Touch t = Input.GetTouch(i);
                        if (r.Contains(t.position)) return t.deltaPosition.x / TouchSensitivity_x;
                    }
                    return Input.GetAxis(axisName);
                }
                else
                {
                    return Input.GetAxis(axisName);
                }

            case "Mouse Y":
                if (Input.touchCount > 0)
                {
                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        Touch t = Input.GetTouch(i);
                        if (r.Contains(t.position)) return t.deltaPosition.y / TouchSensitivity_y;
                    }
                    return Input.GetAxis(axisName);
                }
                else
                {
                    return Input.GetAxis(axisName);
                }

            default:
                Debug.LogError("Input <" + axisName + "> not recognized.");
                break;
        }

        return 0f;
    }
}
