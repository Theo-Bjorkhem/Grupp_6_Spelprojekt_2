using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SnapHelper : MonoBehaviour
{
    [MenuItem("SP/Snapping/Snap to whole number XZ")]
    static void SnapSelectionToWholeNumberXZ()
    {
        Undo.RecordObjects(Selection.transforms, "Snap to whole number XZ");
        foreach (Transform transform in Selection.transforms)
        {
            Vector3 position = transform.position;

            position.x = Mathf.Round(position.x);
            position.z = Mathf.Round(position.z);

            transform.position = position;
        }
    }

    [MenuItem("SP/Apply transform to children")]
    static void ApplyTransformToChildren()
    {
        Transform parent = Selection.activeTransform;
        List<Transform> children = new List<Transform>(parent.childCount);

        foreach (Transform child in parent)
        {
            children.Add(child);
        }

        Undo.RecordObject(parent, "Apply transform to children (parent)");
        Undo.RecordObjects(children.ToArray(), "Apply transform to children (children");

        Vector3 parentPos = parent.localPosition;
        parent.localPosition = Vector3.zero;

        foreach(Transform child in children)
        {
            child.localPosition += parentPos;
        }
    }
}
