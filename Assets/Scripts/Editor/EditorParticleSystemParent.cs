using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(ParticleSystemParent))]
public class EditorParticleSystemParent : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Apply"))
        {
            (serializedObject.targetObject as ParticleSystemParent).ApplyChanges();
        }
    }
}
