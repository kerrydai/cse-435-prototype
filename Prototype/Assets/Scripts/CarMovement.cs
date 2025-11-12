using UnityEngine;
using System.Collections;

public class CarMovement : MonoBehaviour
{
    [Header("Lane Movement Settings")]
    public float speed = 10f;              // Forward speed
    public float swerveAmount = 1.5f;      // How far to swerve from center
    public float swerveSpeed = 2f;         // How fast it swerves
    public float timeBetweenSwerves = 3f;  // Time before next random swerve
    public float swerveHoldTime = 0.5f;    // How long to hold before returning

    [Header("Visual Tilt Settings")]
    public Transform carBody;              // Assign in Inspector (child mesh)
    public float tiltAngle = 15f;          // Max tilt angle when steering
    public float tiltSpeed = 4f;           // How fast tilt adjusts

    private Vector3 centerPos;             // Starting (middle lane) position
    private float targetOffset = 0f;       // Lateral offset (x) from center
    private bool isSwerving = false;
    private float nextSwerveTime;
    private float currentTilt = 0f;

    void Start()
    {
        centerPos = transform.position;
        nextSwerveTime = Time.time + Random.Range(1f, timeBetweenSwerves);
    }

    void Update()
    {
        // Move forward in local space so turning affects direction
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);

        // Handle random swerve
        if (!isSwerving && Time.time >= nextSwerveTime)
        {
            StartCoroutine(DoRandomSwerve());
            nextSwerveTime = Time.time + timeBetweenSwerves;
        }

        // Smoothly move toward the target lateral offset
        Vector3 pos = transform.position;
        float newX = Mathf.Lerp(pos.x, centerPos.x + targetOffset, Time.deltaTime * swerveSpeed);
        transform.position = new Vector3(newX, pos.y, pos.z);

        // Smooth tilt visual only (not whole car direction)
        float targetTilt = Mathf.Lerp(currentTilt, (targetOffset > 0 ? -tiltAngle : (targetOffset < 0 ? tiltAngle : 0)), Time.deltaTime * tiltSpeed);
        currentTilt = targetTilt;
        if (carBody != null)
        {
            carBody.localRotation = Quaternion.Euler(0f, 0f, targetTilt);
        }
    }

    private IEnumerator DoRandomSwerve()
    {
        isSwerving = true;

        // Choose direction
        float direction = Random.value < 0.5f ? -1f : 1f;
        targetOffset = direction * swerveAmount;

        // Slightly rotate the car in that direction to simulate steering
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = Quaternion.Euler(0f, direction * 10f, 0f);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * swerveSpeed;
            transform.rotation = Quaternion.Lerp(startRot, targetRot, t);
            yield return null;
        }

        // Hold the swerve
        yield return new WaitForSeconds(swerveHoldTime);

        // Return to center
        targetOffset = 0f;
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * swerveSpeed;
            transform.rotation = Quaternion.Lerp(targetRot, startRot, t);
            yield return null;
        }

        isSwerving = false;
    }
}
