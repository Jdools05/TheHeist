using UnityEngine;

public class AlertBar : MonoBehaviour
{
    public GameObject alertLevelBar;
    private PlayerController playerController;
    private float alertLevel;
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        alertLevel = playerController.GetAlert();
        float scale = Remap(alertLevel, 0, 100, 0, 750);
        float xPos = scale / 2f;
        alertLevelBar.transform.localPosition = new Vector3(xPos, 0, 0);
        alertLevelBar.transform.localScale = new Vector3 (scale, 60, 1);
    }

    float Remap (float val, float l1, float h1, float l2, float h2)
    {
        return l2 + (h2 - l2) * ((val - l1) / (h1 - l1));
    }
}
