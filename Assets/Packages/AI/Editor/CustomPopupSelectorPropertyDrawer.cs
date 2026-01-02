using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Xennial.AI
{
    [CustomPropertyDrawer(typeof(CustomPropertyPopupSelector))]
    public class CustomPopupSelectorPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var stringOptions = (CustomPropertyPopupSelector)attribute;
            List<string> options = stringOptions.Options;
            if (options == null || options.Count == 0)
            {
                EditorGUI.LabelField(position, label.text, "No data available");
                return;
            }
            int selectedIndex = Mathf.Max(0, options.IndexOf(property.stringValue));
            selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, options.ToArray());
            if (selectedIndex >= 0 && selectedIndex < options.Count)
            {
                property.stringValue = options[selectedIndex];
            }
        }
    }


}

