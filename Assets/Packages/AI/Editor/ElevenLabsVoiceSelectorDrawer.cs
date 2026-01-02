using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Services.AI
{
    [CustomPropertyDrawer(typeof(ElevenLabsVoiceSelector))]
    public class ElevenLabsVoiceSelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            List<string> voices = ElevenLabsVoiceDataUtil.GetElevenLabsVoiceData().voices.Select(x => x.name).ToList();
            if (voices == null || voices.Count == 0)
            {
                EditorGUI.LabelField(position, label.text, "No voices available");
                return;
            }
            int selectedIndex = Mathf.Max(0, voices.IndexOf(property.stringValue));
            selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, voices.ToArray());
            if (selectedIndex >= 0 && selectedIndex < voices.Count)
            {
                property.stringValue = voices[selectedIndex];
            }
        }
    }


}

