using UnityEngine;

public class PathFollower : MonoBehaviour
{
    [Header("Path Settings")]
    public Transform[] pathPoints; // Drag your waypoints here in Inspector
    public float speed = 10f;
    public bool loopPath = true;

    private int currentPoint = 0;
    private float progress = 0f;

    void Update()
    {
        if (pathPoints == null || pathPoints.Length < 2) return;

        // Get current and next waypoint
        Transform start = pathPoints[currentPoint];
        Transform end = pathPoints[(currentPoint + 1) % pathPoints.Length];

        // Calculate progress along this segment
        float distance = Vector3.Distance(start.position, end.position);
        progress += (speed * Time.deltaTime) / distance;

        // Move to next waypoint when we reach the end
        if (progress >= 1f)
        {
            progress = 0f;
            currentPoint++;

            // Loop or stop at end
            if (currentPoint >= pathPoints.Length - 1)
            {
                if (loopPath)
                    currentPoint = 0;
                else
                    currentPoint = pathPoints.Length - 2; // Stay at last segment
            }
        }

        // Smoothly move and rotate along path
        transform.position = Vector3.Lerp(start.position, end.position, progress);
        transform.rotation = Quaternion.Lerp(start.rotation, end.rotation, progress);
    }
}