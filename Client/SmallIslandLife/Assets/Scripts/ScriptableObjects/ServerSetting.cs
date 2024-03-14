using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

public class ServerSetting : ScriptableObject
{
    private const string SETTING_FILE_DIRECTORY = "Assets/Resources";
    private const string SETTING_FILE_PATH = "Assets/Resources/ServerSetting.asset";

    private static ServerSetting instance;
    public static ServerSetting Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            instance = Resources.Load<ServerSetting>("ServerSetting");

            #if UNITY_EDITOR

            if (instance == null)
            {
                if (!AssetDatabase.IsValidFolder(SETTING_FILE_DIRECTORY))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }

                instance = AssetDatabase.LoadAssetAtPath<ServerSetting>(SETTING_FILE_PATH);

                if (instance == null)
                {
                    instance = CreateInstance<ServerSetting>();

                    AssetDatabase.CreateAsset(instance, SETTING_FILE_PATH);
                }
            }

            #endif

            return instance;
        }
    }

    [System.Serializable]
    public struct ServerConfig
    {
        public int id;
        public string name;
    }

    public ServerConfig[] serverConfigs;
}
