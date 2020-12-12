using UnityEngine;
using ExtensionsMethods;

namespace ExtensionsMethods {
    public static class Vector3Extensions {
        public static Vector3 GlobalToLocal(this Vector3 position, Vector3 origin) {
            return position - origin;
        }

        public static Vector3 PointFromAngle(this Vector3 position, float angle, float radius) {
            return position + (new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle))* Mathf.Rad2Deg);
        }

        public static float AngleToPoint(this Vector3 position, Vector3 point) {
            return -(Mathf.Atan2(point.z - position.z, point.x - position.x) * Mathf.Rad2Deg);
        }

        public static float Distance(this Vector3 position, Vector3 point) {
            return position.GlobalToLocal(point).magnitude;
        }
    }

    public static class FloatExtensions {
        public static float Abs(this float value) {
            return Mathf.Abs(value);
        }
        
        public static float Map(this float n, float start1, float stop1, float start2, float stop2, float desiredMax)
        {
            var newval = (n - start1) / (stop1 - start1) * (stop2 - start2) + start2;
            return start2 < stop2 ? newval.Clamp(start2, stop2) : newval.Clamp(stop2, start2);
        }

        public static float Clamp(this float value, float min, float max) {
            return Mathf.Max(Mathf.Min(value, max), min);
        }

        public static float Mod(this float value, float max) {
            return ((value % max) + max) % max;
        }
    }
}

public class Robot : MonoBehaviour {
    public Transform[] waypoints;
    public float waypointRadius;
    public float movementSpeed;
    public float rotationSpeed;
    public Transform focalPoint;
    public FieldOfView fov;
    public GameObject graphics;
    public Transform originPoint;

    private int waypointIndex = 0;
    private GameObject player;
    private bool isLookingAtWaypoint = false;
    [SerializeField] private LayerMask layerMask;


    void Start() {
        player = FindObjectOfType<PlayerController>().gameObject;
    }

    Transform GetCurrentWaypoint() => waypoints[waypointIndex];

    public void IncrementWaypoint() {
        waypointIndex = (waypointIndex + 1) % waypoints.Length;
        isLookingAtWaypoint = false;
    }

    void Update() {
        Vector3 waypoint = GetCurrentWaypoint().position;
        Vector3 position = transform.position;

        float targetAngle = position.AngleToPoint(waypoint).Mod(360);
        float currentAngle = graphics.transform.rotation.eulerAngles.y.Mod(360);
        float difference = (targetAngle - currentAngle).Mod(360);

        if (difference.Abs() > rotationSpeed / 2f && !isLookingAtWaypoint)
        {
            //Debug.Log("Rotate " + targetAngle);
            bool negative = (difference > 180);

            float step = rotationSpeed * Time.deltaTime;
            step = step <= difference ? negative ? -step : step : difference;
            graphics.transform.rotation = Quaternion.Euler(0, graphics.transform.rotation.eulerAngles.y + step, 0);
        } else
        {
            if (difference != 0)
            {
                isLookingAtWaypoint = true;
                graphics.transform.rotation = Quaternion.Euler(0, targetAngle, 0);
            }

            //Debug.Log("Move");
            float step = movementSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, waypoint, step);
        }

        CheckMoveNext(position, waypoint);

 
        fov.SetOrigin(originPoint.position);
        fov.SetAimDirection(focalPoint.position - originPoint.position);
        RaycastCheck();
    }

    void CheckMoveNext(Vector3 pos, Vector3 wp) {
        if (pos.Distance(wp) < waypointRadius) {
            IncrementWaypoint();
        }
    }

    private void RaycastCheck()
    {
        if ((player.transform.position - fov.transform.position).magnitude <= fov.viewDistance)
        {
            Ray ray = new Ray(transform.position, player.transform.position - transform.position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                Debug.Log("Hit");
                if (hit.collider.tag == "Player")
                {
                    Debug.Log("Player Collision");
                    float playerAngle = FieldOfView.GetAngleFromVectorFloat(hit.point - transform.position);
                    float focalPointAngle = FieldOfView.GetAngleFromVectorFloat(focalPoint.position - originPoint.position);
                    if (Mathf.Abs(playerAngle - focalPointAngle) <= fov.fov / 2f)
                    {
                        PlayerController pc = player.GetComponent<PlayerController>();
                        pc.AddAlert();
                    }
                }
            }
        }
    }
}