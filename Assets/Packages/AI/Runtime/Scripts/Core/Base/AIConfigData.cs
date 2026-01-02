using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using UnityEngine;
namespace Services.AI
{
    [CreateAssetMenu(fileName = "AIConfigData", menuName = "Xennial Digital/AI/AIConfigData", order = 1)]
    public class AIConfigData : SerializedScriptableObject
    {
        public string Url;
        public bool UseToken;
        [ShowIf(nameof(UseToken))]
        public string Token;
        public string AIModel;
        public string AIProvider;
        public string AIModelName;
        public string Version;
        [TextArea(4, 30)]
        public string SystemPrompt;
        [SerializeField]
        public bool UseFunctionCallTemplate;
        [SerializeField, ShowIf(nameof(UseFunctionCallTemplate))]
        public FunctionCallData FunctionCallData;
        [SerializeField]
        private Dictionary<string, string> _extraData;
        [Title("Update Settings")]
        public bool UpdateFromUrl;
        [ShowIf(nameof(UpdateFromUrl))]
        public string FileUrl;
        private string _localPath;

        public string GetExtraDataByKey(string key)
        {
            if (_extraData.TryGetValue(key, out var result))
            {
                return result;
            }
            return string.Empty;
        }

        public void SetValueOnExtraData(string key, string value)
        {
            if (ExistKey(key))
            {
                _extraData[key] = value;
            }
        }

        public bool ExistKey(string key)
        {
            return _extraData.ContainsKey(key);
        }

        [Button, ShowIf(nameof(UpdateFromUrl))]
        public async void UpdatePromptFromUrl(Action OnDonwloadComplete)
        {
            _localPath = Path.Combine(Application.persistentDataPath, "AISystemPromp.txt");
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string fileContent = await client.GetStringAsync(FileUrl);
                    File.WriteAllText(_localPath, fileContent);
                    if (File.Exists(_localPath))
                    {
                        SystemPrompt = File.ReadAllText(_localPath);
                        OnDonwloadComplete?.Invoke();
                    }

                }
                catch (HttpRequestException e)
                {
                    Debug.LogError("Download file error: " + e.Message);
                    OnDonwloadComplete?.Invoke();
                }
            }
        }
    }
}
