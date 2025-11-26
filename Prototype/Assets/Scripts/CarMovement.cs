using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class CarMovement : MonoBehaviour
{
    [Header("Movement")]
    public float forwardSpeed = 10f;
    public float turnSpeed = 3f;
    public float autoCenterStrength = 5f;

    [Header("Lane References")]
    public Transform laneParent;
    public float laneThreshold = 1.5f;

    [Header("UI")]
    public TMP_Text laneStatusText;

    private Transform[] lanePoints;
    private Transform closestLanePoint;
    private float currentSidewaysDistance;

    void Start()
    {
        if (laneParent != null)
        {
            lanePoints = new Transform[laneParent.childCount];
            for (int i = 0; i < laneParent.childCount; i++)
                lanePoints[i] = laneParent.GetChild(i);
        }
        else
        {
            lanePoints = new Transform[0];
        }
    }

    void Update()
    {
        MoveForward();
        TurnWithMouse();

        closestLanePoint = GetClosestLanePoint();

        AutoCenterToLane();
        UpdateLaneStatusText();
    }

    void MoveForward()
    {
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
    }

    void TurnWithMouse()
    {
        if (Mouse.current == null) return;

        float mouseX = Mouse.current.delta.x.ReadValue();
        float turn = mouseX * turnSpeed * Time.deltaTime;

        transform.Rotate(0, turn, 0);
    }

    Transform GetClosestLanePoint()
    {
        if (lanePoints == null || lanePoints.Length == 0)
            return null;

        float best = Mathf.Infinity;
        Transform bestPoint = null;

        foreach (var p in lanePoints)
        {
            if (p == null) continue;

            float d = Vector3.Distance(transform.position, p.position);

            if (d < best)
            {
                best = d;
                bestPoint = p;
            }
        }

        return bestPoint;
    }

    void AutoCenterToLane()
    {
        if (closestLanePoint == null)
            return;

        Vector3 offsetWorld = closestLanePoint.position - transform.position;
        float sideways = Vector3.Dot(offsetWorld, transform.right);
        currentSidewaysDistance = Mathf.Abs(sideways);

        // Smooth steering toward center
        if (currentSidewaysDistance > laneThreshold * 0.35f) // start adjusting a bit early
        {
            Vector3 targetDirection = closestLanePoint.position - transform.position;
            targetDirection.y = 0; // keep rotation horizontal

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

            // Smooth, slower rotation
            float smoothSpeed = autoCenterStrength * 0.2f; // reduce turning speed
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
        }
    }


    void UpdateLaneStatusText()
    {
        if (laneStatusText == null) return;

        float d = currentSidewaysDistance;

        if (d < laneThreshold * 0.35f)
            laneStatusText.text = "<color=green>LDWS: Ready</color>";
        else if (d < laneThreshold)
            laneStatusText.text = "<color=yellow>LDWS: Lane Approach Warning</color>";
        else
            laneStatusText.text = "<color=red>LDWS: Lane Departure Warning!</color>";
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        if (closestLanePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(closestLanePoint.position, 0.25f);
            Gizmos.DrawLine(transform.position, closestLanePoint.position);
        }

        if (lanePoints != null)
        {
            Gizmos.color = Color.green;
            foreach (var p in lanePoints)
            {
                if (p != null)
                    Gizmos.DrawSphere(p.position, 0.12f);
            }
        }
    }
}
