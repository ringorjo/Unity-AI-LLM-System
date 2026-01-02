using System.Collections.Generic;
using UnityEngine;

public class CustomPropertyPopupSelector : PropertyAttribute
{
    public List<string> Options = new List<string>();

    public CustomPropertyPopupSelector(string dataLocation)
    {
#if UNITY_EDITOR
        CustomPropertyListDataBase customPropertyListDataBase = ScriptablePropertyDataLoaderUtil.GetScriptableData(dataLocation);
        if (customPropertyListDataBase != null)
        {
            Options.AddRange(customPropertyListDataBase.Options);
            return;
        }
        Debug.LogError($"Is not Posible to get the {dataLocation}.asset to load OpcionData ");
#endif

    }
}

