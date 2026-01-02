using UnityEngine;
namespace Services.AI
{
    public static class AIConfigUtils
    {
        private const string PATH_FILE = "AIConfigData";
        private static AIConfigData _config = null;
        public static AIConfigData GetConfig()
        {
            if (_config == null)
            {
                _config = Resources.Load<AIConfigData>(PATH_FILE);
                if(!Application.isPlaying)
                {
                    if (_config == null)
                    {
                        Debug.LogError($"The File {PATH_FILE} no exist in resources");
                    }
                }
               
            }
            return _config;
        }



    }
}
