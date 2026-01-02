using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public static class ScriptablePropertyDataLoaderUtil
{
#if UNITY_EDITOR
    private const string ROOT_SCRIPTABLE_FOLDER = "Assets/Editor/Resources";
    private const string EXTENTION_FILE = ".asset";
    public static CustomPropertyListDataBase GetScriptableData(string fileName)
    {
        EnsureScriptableFolder();
        string fileExtention = $"{fileName}{EXTENTION_FILE}";
        string path = Path.Combine(ROOT_SCRIPTABLE_FOLDER, fileExtention);
        CustomPropertyListDataBase data = AssetDatabase.LoadAssetAtPath<CustomPropertyListDataBase>(path);
        if (data == null)
        {
            Debug.Log($"Scriptable with name: {fileName} not found creating a new one");
            data = ScriptableObject.CreateInstance<CustomPropertyListDataBase>();
            AssetDatabase.CreateAsset(data, path);
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        return data;
    }

    public static void AddNewOption(string option, ref CustomPropertyListDataBase destination)
    {
        destination?.AddNewOption(option);
    }

    private static void EnsureScriptableFolder()
    {
        if (!Directory.Exists(ROOT_SCRIPTABLE_FOLDER))
        {
            Directory.CreateDirectory(ROOT_SCRIPTABLE_FOLDER);
        }
    }
#endif

}

