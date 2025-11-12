using UnityEngine;
using TMPro;

public class LMS : MonoBehaviour
{
    [Header("UI Reference")]
    public TMP_Text Text;

    [Header("Car Reference")]
    public CarMovement carMovement;  // Drag your car object here in the Inspector

    void Start()
    {
        Text.text = "LMS Scenario Testing:\nClick buttons to test different LMS functionalities.";
        if (carMovement != null)
        {
            carMovement.enabled = false; // Start with car stopped
        }
    }

    public void Scenario1()
    {
        Text.text = "LMS Scenario 1: Normal Driving.";

        if (carMovement != null)
        {
            carMovement.enabled = true; // Enable movement
        }
    }

    public void Scenario2()
    {
        Text.text = "LMS Scenario 2: Lane Warning.";

        if (carMovement != null)
        {
            carMovement.enabled = false; // Stop movement
        }
    }
}
