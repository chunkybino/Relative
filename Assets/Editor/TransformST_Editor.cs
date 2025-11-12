using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TransformST))]
public class TransformST_Editor : Editor
{
    TransformST transform;

    public override void OnInspectorGUI()
    {
        transform = (TransformST)target;

        base.OnInspectorGUI();

        EditorGUILayout.FloatField("Gamma", transform.gamma);
        EditorGUILayout.FloatField("CMult", transform.Cmult);

        Vector3 vel = EditorGUILayout.Vector3Field("Velocity", transform.velocity);
        Vector3 velProp = EditorGUILayout.Vector3Field("Velocity Proper", transform.velocityProper);

        if (vel != transform.velocity)
        {
            transform.velocity = vel;
        }
        if (velProp != transform.velocityProper)
        {
            transform.velocityProper = velProp;
        }
    }
}
