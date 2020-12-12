using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

public class MountedCamera : MonoBehaviour
{
    [SerializeField] private FieldOfView fovScript;
    public Transform focalPoint;
    public bool lookAround;
    public float lookAroundSpeed;
    public float radLeft;
    public float radRight;
    public Transform originPoint;

    private bool flip = false;
    private float rad = 0f;
    public static GameObject player;
    [SerializeField] private LayerMask layerMask;

    void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
    }


    void Update()
    {
        if (lookAround) // The camera should pan from side to side
        {
            float clamped = Mathf.Clamp(rad, -radLeft, radRight);
            focalPoint.localPosition = new Vector3(Mathf.Cos(clamped) * 10, 0, Mathf.Sin(clamped) * 10);
            rad += flip ? -lookAroundSpeed / 10000f : lookAroundSpeed / 10000f;
            if (rad  < -radLeft - 0.1f) flip = false;
            if (rad  > radRight + 0.1f) flip = true;
        }
        fovScript.SetAimDirection(focalPoint.localPosition);
        fovScript.SetOrigin(originPoint.position);
        RaycastCheck();
    }

    private void RaycastCheck()
    {
        if ((player.transform.position - fovScript.transform.position).magnitude <= fovScript.viewDistance)
        {
            Ray ray = new Ray(transform.position, player.transform.position - transform.position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.tag == "Player")
                {
                    float playerAngle = FieldOfView.GetAngleFromVectorFloat(hit.point - transform.position);
                    float focalPointAngle = FieldOfView.GetAngleFromVectorFloat(focalPoint.localPosition);
                    if (Mathf.Abs(playerAngle - focalPointAngle) <= fovScript.fov / 2f)
                    {
                        PlayerController pc = player.GetComponent<PlayerController>();
                        pc.AddAlert();
                    }
                }
            }
        }
    }
}
