using UnityEngine;

public class LanePointGizmo : MonoBehaviour
{
    public Color pointColor = Color.green;
    public float pointSize = 0.3f;

    private void OnDrawGizmos()
    {
        Gizmos.color = pointColor;

        // Draw every point as a sphere
        foreach (Transform t in transform)
        {
            Gizmos.DrawSphere(t.position, pointSize);
        }

        // Draw lines connecting points
        Transform prev = null;
        foreach (Transform t in transform)
        {
            if (prev != null)
                Gizmos.DrawLine(prev.position, t.position);

            prev = t;
        }

        // Close the loop
        if (transform.childCount > 1)
        {
            Gizmos.DrawLine(transform.GetChild(transform.childCount - 1).position,
                            transform.GetChild(0).position);
        }
    }
}
