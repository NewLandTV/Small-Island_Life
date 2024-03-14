using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ServerSetting))]
public class ServerSettingEditor : Editor
{
    [MenuItem("Assets/Settings/Open Server Setting")]
    public static void OpenInspector()
    {
        Selection.activeObject = ServerSetting.Instance;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }
    }
}
