using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LanePointGenerator))]
public class LanePointGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LanePointGenerator gen = (LanePointGenerator)target;

        if (GUILayout.Button("Generate Lane Points"))
        {
            gen.GeneratePoints();
        }
    }
}
