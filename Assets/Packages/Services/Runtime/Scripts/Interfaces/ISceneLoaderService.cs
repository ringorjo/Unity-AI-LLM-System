using System;
using UnityEngine.SceneManagement;

namespace Xennial.Services
{
    public interface ISceneLoaderService : IService
    {
        public event Action SceneLoadRequested;
        public event Action<int> SceneLoaded;
        public event Action<int> SceneUnloaded;

        public void LoadScene(int sceneBuildIndex);
        public void LoadScene(int sceneBuildIndex, LoadSceneMode loadSceneMode);
        public void LoadScene(int sceneBuildIndex, LoadSceneMode loadSceneMode, bool allClients);
        public void LoadScene(string sceneName);
        public void LoadScene(string sceneName, LoadSceneMode loadSceneMode);
        public void LoadScene(string sceneName, LoadSceneMode loadSceneMode, bool allClients);
    }
}