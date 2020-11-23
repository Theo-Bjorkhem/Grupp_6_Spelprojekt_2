using UnityEditor;
using UnityEngine;

public class PrefabInstantiatorWindow : EditorWindow
{
    SerializedObject mySelf;
    bool mySelectionListUnfolded = true;

    [SerializeField] GameObject myPrefab;
    [SerializeField] bool myEnumerate = true;
    [SerializeField] bool myPreserveParent = true;

    SerializedProperty myEnumerateProperty;
    SerializedProperty myPrefabProperty;
    SerializedProperty myPreserveParentProperty;

    [MenuItem("Window/PrefabInstantiator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(PrefabInstantiatorWindow));
    }

    void OnEnable()
    {
        mySelf = new SerializedObject(this);
        myPrefabProperty = mySelf.FindProperty("myPrefab");
        myEnumerateProperty = mySelf.FindProperty("myEnumerate");
        myPreserveParentProperty = mySelf.FindProperty("myPreserveParent");
    }

    void InstantiateOnGroup(GameObject aPrefab, Transform[] someTransforms)
    {
        for (int i = 0; i < someTransforms.Length; i++)
        {
            GameObject prefab = InstantiateWithTransform(aPrefab, someTransforms[i]);
            if (myEnumerate && i > 0)
            {
                prefab.name += " (" + i + ")";
            }
        }
    }

    void DestroyGameObjects(GameObject[] someObjectsToDestroy)
    {
        foreach (GameObject obj in someObjectsToDestroy)
        {
            DestroyImmediate(obj);
        }
    }

    GameObject InstantiateWithTransform(GameObject aPrefab, Transform aTransform)
    {
        GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(aPrefab);
        prefab.transform.localScale = aTransform.lossyScale;
        if (myPreserveParent)
        {
            prefab.transform.SetParent(aTransform.parent);
        }
        prefab.transform.SetPositionAndRotation(aTransform.position, aTransform.rotation);
        return prefab;
    }

    void OnGUI()
    {
        EditorGUILayout.PropertyField(myPrefabProperty);
        EditorGUILayout.PropertyField(myPreserveParentProperty);
        EditorGUILayout.PropertyField(myEnumerateProperty);

        mySelf.ApplyModifiedProperties();

        if (GUILayout.Button("Instantiate"))
        {
            InstantiateOnGroup(myPrefab, Selection.transforms);
        }

        if (GUILayout.Button("Instantiate & Delete"))
        {
            InstantiateOnGroup(myPrefab, Selection.transforms);
            DestroyGameObjects(Selection.gameObjects);
        }

        mySelectionListUnfolded = EditorGUILayout.BeginFoldoutHeaderGroup(mySelectionListUnfolded, "Selected objects");
        if (mySelectionListUnfolded)
        {
            foreach (GameObject selectedObject in Selection.gameObjects)
            {
                EditorGUILayout.LabelField(selectedObject.name);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
}
