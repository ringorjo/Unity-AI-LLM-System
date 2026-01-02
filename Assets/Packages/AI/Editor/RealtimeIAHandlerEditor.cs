using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Services.AI
{
    [CustomEditor(typeof(RealtimeIAHandler))]
    public class RealtimeIAHandlerEditor : OdinEditor
    {
        AIHandlerBase _aiHandlerBase => target as AIHandlerBase;
        private List<MonoBehaviour> _aiConfigMonobehaviour;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

        
            if (GUILayout.Button("Update AIConfig"))
            {
                UpdateAIConfig();
            }
        }

        private async void UpdateAIConfig()
        {
            _aiHandlerBase.ApisOverrideConfigurationList = _aiHandlerBase.transform.GetComponentsInChildren<ISetAIConfig>().ToList();
            _aiConfigMonobehaviour = new List<MonoBehaviour>(_aiHandlerBase.ApisOverrideConfigurationList.Cast<MonoBehaviour>());
            for (int i = 0; i < _aiHandlerBase.ApisOverrideConfigurationList.Count; i++)
            {
                GameObject go = _aiConfigMonobehaviour[i].gameObject;
                Selection.activeObject = go;
                await Task.Delay(100);
                _aiHandlerBase.ApisOverrideConfigurationList[i].OverrideIAConfig(_aiHandlerBase.GetConfigData);
                EditorUtility.SetDirty(go);
                await Task.Delay(100);
            }
            Selection.activeObject = _aiHandlerBase.gameObject;
        }

    }

}
