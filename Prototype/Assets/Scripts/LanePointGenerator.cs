using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LanePointGenerator : MonoBehaviour
{
    public float spacing = 2f;
    public float laneOffset = 1.5f;

    public Transform lanePointsParent;

    void Update()
    {
        if (!Application.isEditor || Application.isPlaying)
            return;

        GeneratePoints();
    }

    public void GeneratePoints()
    {
        ClearExisting();

        if (lanePointsParent == null)
        {
            lanePointsParent = new GameObject("LanePoints").transform;
            lanePointsParent.parent = transform;
        }

        MeshFilter mf = GetComponent<MeshFilter>();
        if (!mf || !mf.sharedMesh) return;

        Mesh mesh = mf.sharedMesh;

        float roadLength = mesh.bounds.size.z * transform.localScale.z;

        for (float z = 0; z < roadLength; z += spacing)
        {
            Vector3 worldPos = transform.TransformPoint(new Vector3(0, 0, z));

            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            g.name = "LanePoint_" + z;
            g.transform.position = worldPos;
            g.transform.localScale = Vector3.one * 0.15f;

            DestroyImmediate(g.GetComponent<Collider>());

            g.transform.parent = lanePointsParent;
        }
    }

    private void ClearExisting()
    {
        if (lanePointsParent != null)
            DestroyImmediate(lanePointsParent.gameObject);
    }
}
