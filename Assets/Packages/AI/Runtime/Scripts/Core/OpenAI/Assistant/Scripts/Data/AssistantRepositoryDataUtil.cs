using System.Collections.Generic;
using UnityEngine;

namespace Services.AI
{
    public class AssistantRepositoryDataUtil
    {
        private Dictionary<string, object> _repositoryData;

        public Dictionary<string, object> RepositoryData
        {
            get => _repositoryData;
        }

        public AssistantRepositoryDataUtil()
        {
            _repositoryData = new Dictionary<string, object>();
        }

        public bool DataExist(string key)
        {
            return _repositoryData.ContainsKey(key);
        }

        public void AddDataToRepository(string key, object value)
        {
            if (!DataExist(key))
                _repositoryData.Add(key, value);
            else
                _repositoryData[key] = value;

            UnityEngine.Debug.Log($"Repository Updated with key: {key} and value : {value}");
        }

        public T GetData<T>(string key)
        {
            if (_repositoryData.TryGetValue(key, out object result))
            {
                return (T)result;
            }
            else
            {
                Debug.LogError($"Get Data failed for {key} key does not exist data type");
                return default(T);
            }

        }

    }
}
