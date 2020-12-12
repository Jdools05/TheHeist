using System.Dynamic;
using TMPro;
using UnityEngine;

public class PlayerAbilityController : MonoBehaviour
{
    public float power = 100f;
    public JoyButton fixedJoyButtonUse;
    private bool onDown = true;

    void Start()
    {
        Ability.id = 0;
    }

    void Update()
    {
        if (fixedJoyButtonUse.Pressed)
        {
            if (onDown) Ability.UseAbility();
            onDown = false;
        }
        else
        {
            onDown = true;
        }
        if (Ability.isActive)
        {
            power -= Ability.activeAbilityCost * Time.deltaTime;
        }
        if (power <= 0) Ability.EndOverride();
    }
}

public class Ability
{
    public static bool isActive = false;
    public static int id = 0;
    public static float activeAbilityCost = 0;

    public static void EndOverride()
    {
        EndAbility();
    }

    public static void UseAbility()
    {
        if (isActive) EndAbility(); else ActivateAbility();
        isActive = !isActive;
    }

    private static void ActivateAbility()
    {
        switch (id)
        {
            case 0:
                ToggleFOV(true);
                break;
        }
    }

    private static void EndAbility()
    {
        switch (id)
        {
            case 0:
                ToggleFOV(false);
                break;
        }
    }

    private static void ToggleFOV(bool b)
    {
        activeAbilityCost = b ? 5 : 0;
        FieldOfView[] fovs = Object.FindObjectsOfType<FieldOfView>();
        foreach (FieldOfView f in fovs)
        {
            f.gameObject.GetComponent<MeshRenderer>().enabled = b;
        }
    }

}
