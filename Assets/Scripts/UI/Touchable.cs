#if UNITY_EDITOR

[UnityEditor.CustomEditor(typeof(Touchable))]
public class TouchableEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI() { }
}

#endif

public class Touchable : UnityEngine.UI.Graphic
{
    protected override void UpdateGeometry() { }
}
