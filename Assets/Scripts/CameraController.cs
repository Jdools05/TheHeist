using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 FirstPoint;
    Vector3 SecondPoint;
    float xAngle;
    float yAngle;
    float xAngleTemp;
    float yAngleTemp;
    // represents area where user can drag to rotate camera
    Rect rightSide = new Rect(Screen.width / 2, 0, Screen.width / 2, Screen.height);
    GameObject player;

    void Start()
    {
        xAngle = 0;
        yAngle = 0;
        this.transform.rotation = Quaternion.Euler(yAngle, xAngle, 0);
        player = FindObjectOfType<PlayerController>().gameObject;
    }

    void Update()
    {
        // handles camera rotation with touch
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch t = Input.GetTouch(i);
                if (rightSide.Contains(t.position)) {
                    if (Input.GetTouch(i).phase == TouchPhase.Began)
                    {
                        FirstPoint = Input.GetTouch(i).position;
                        xAngleTemp = xAngle;
                        yAngleTemp = yAngle;
                    }
                    if (Input.GetTouch(i).phase == TouchPhase.Moved)
                    {
                        SecondPoint = Input.GetTouch(i).position;
                        xAngle = xAngleTemp + (SecondPoint.x - FirstPoint.x) * 180 / Screen.width;
                        yAngle = yAngleTemp + (SecondPoint.y - FirstPoint.y) * 90 / Screen.height;
                        yAngle = Mathf.Clamp(yAngle, -85f, 85f);
                        this.transform.rotation = Quaternion.Euler(-yAngle, xAngle, 0.0f);
                    }
                }
            }
        }
        // camera offset for player
        transform.position = player.transform.position + new Vector3(0, 1.2f, 0);
    }
}
