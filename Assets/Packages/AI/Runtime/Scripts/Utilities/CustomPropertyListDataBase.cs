using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomPropertyListData", menuName = "Xennial Digital/AI/CustomPropertyListData", order = 1)]
public class CustomPropertyListDataBase : ScriptableObject
{
    public List<string> Options = new List<string>();

    public void AddNewOption(string option)
    {
        if (!Options.Contains(option))
        {
            Options.Add(option);
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
    }
}

