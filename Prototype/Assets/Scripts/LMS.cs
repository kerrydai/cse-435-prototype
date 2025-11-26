using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class LMS : MonoBehaviour
{
    [Header("LDWS Settings")]
    public TMP_Text LDWSText;
    public TMP_Text LMSText;
    public float leftLane_X = -12.3f;
    public float middleLane_X = -2.34f;
    public float rightLane_X = 4.7f;
    public float warningZoneBuffer = 2f; // Distance from lane edge to show yellow warning

    [Header("Car Settings")]
    public Transform car;
    public float forwardSpeed = 10f;
    public float turnSpeed = 5f;
    public float maxLeanAngle = 25f;      // Maximum lean angle in degrees
    public float leanSpeed = 8f;          // How fast the car leans
    public float autoCenterStrength = 1.5f; // LMS gentle pull in green/yellow
    public float redZoneStrength = 8f;    // LMS strong pull in red zone

    private bool lmsEnabled = true;
    private Vector2 moveInput;

    void Update()
    {
        if (car == null) return;

        // --- Get Mouse Input (normalized -1 to 1) ---
        moveInput.x = (Mouse.current.position.ReadValue().x / Screen.width - 0.5f) * 2f;

        // --- Forward movement ---
        car.Translate(Vector3.forward * forwardSpeed * Time.deltaTime, Space.World);

        // --- LMS Steering ---
        float newX = car.position.x;

        if (lmsEnabled)
        {
            float targetX = middleLane_X;
            float pullStrength = 0f;

            // Determine which zone we're in
            if (car.position.x < leftLane_X || car.position.x > rightLane_X)
            {
                // RED ZONE - Strong correction
                pullStrength = redZoneStrength;
            }
            else if (car.position.x < leftLane_X + warningZoneBuffer || car.position.x > rightLane_X - warningZoneBuffer)
            {
                // YELLOW ZONE - Medium correction
                pullStrength = autoCenterStrength * 1.5f;
            }
            else
            {
                // GREEN ZONE - Gentle centering
                pullStrength = autoCenterStrength;
            }

            // Apply LMS pull towards center lane
            float lmsCorrection = (targetX - car.position.x) * pullStrength * Time.deltaTime;
            newX = car.position.x + lmsCorrection;

            // Add player input (LMS will fight against it)
            newX += moveInput.x * turnSpeed * Time.deltaTime;
        }
        else
        {
            // LMS off → full manual control
            newX = car.position.x + moveInput.x * turnSpeed * Time.deltaTime;
        }

        // --- Apply horizontal movement ---
        car.position = new Vector3(newX, car.position.y, car.position.z);

        // --- Apply LEAN based on mouse input (Z-axis rotation) ---
        float targetZRotation = -moveInput.x * maxLeanAngle;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetZRotation);
        car.rotation = Quaternion.Lerp(car.rotation, targetRotation, Time.deltaTime * leanSpeed);

        // --- LDWS Status Updates ---
        float car_X = car.position.x;

        if (car_X < leftLane_X || car_X > rightLane_X)
        {
            // RED ZONE
            LDWSText.text = "LDWS: Lane Departure Warning!";
            LDWSText.color = Color.red;
        }
        else if (car_X < leftLane_X + warningZoneBuffer || car_X > rightLane_X - warningZoneBuffer)
        {
            // YELLOW ZONE
            LDWSText.text = "LDWS: Lane Approach Warning";
            LDWSText.color = Color.yellow;
        }
        else
        {
            // GREEN ZONE
            LDWSText.text = "LDWS: Ready";
            LDWSText.color = Color.green;
        }

        // --- LMS Status ---
        if (LMSText != null)
            LMSText.text = "LMS: " + (lmsEnabled ? "ON" : "OFF");
    }

    // Toggle LMS auto-centering
    public void ToggleLMS()
    {
        lmsEnabled = !lmsEnabled;
    }

    void LateUpdate()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            ToggleLMS();
        }
    }

    // Reset scenario
    public void Scenario1()
    {
        if (car != null)
            car.position = new Vector3(middleLane_X, car.position.y, 0f);

        if (LDWSText != null)
            LDWSText.text = "Scenario 1";
    }

    // Draw lane markers in Scene view
    void OnDrawGizmos()
    {
        if (car == null) return;

        float laneZStart = car.position.z - 10f;
        float laneZEnd = car.position.z + 50f;

        // Left lane boundary (RED)
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(leftLane_X, car.position.y, laneZStart),
                        new Vector3(leftLane_X, car.position.y, laneZEnd));

        // Left warning zone (YELLOW)
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(leftLane_X + warningZoneBuffer, car.position.y, laneZStart),
                        new Vector3(leftLane_X + warningZoneBuffer, car.position.y, laneZEnd));

        // Middle lane (GREEN)
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(middleLane_X, car.position.y, laneZStart),
                        new Vector3(middleLane_X, car.position.y, laneZEnd));

        // Right warning zone (YELLOW)
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(rightLane_X - warningZoneBuffer, car.position.y, laneZStart),
                        new Vector3(rightLane_X - warningZoneBuffer, car.position.y, laneZEnd));

        // Right lane boundary (RED)
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(rightLane_X, car.position.y, laneZStart),
                        new Vector3(rightLane_X, car.position.y, laneZEnd));
    }
}