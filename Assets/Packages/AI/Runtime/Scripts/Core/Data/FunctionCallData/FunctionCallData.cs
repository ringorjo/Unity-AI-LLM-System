using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Services.AI
{
    [CreateAssetMenu(fileName = "FunctionCallDataTemplate", menuName = "Xennial Digital/AI/FunctionCallDataTemplate", order = 1)]
    public class FunctionCallData : SerializedScriptableObject
    {
        public string JsonFormat;
        public List<Tools> tools;


        public string GetJson()
        {
            return JsonConvert.SerializeObject(tools);
        }

        [Button("Generate Function Data from JSON")]
        public void GenerateFunction()
        {
            Tools tool = JsonConvert.DeserializeObject<Tools>(JsonFormat);
            tool.type = "function";
            tools = new List<Tools>{
                tool
            };
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
}
