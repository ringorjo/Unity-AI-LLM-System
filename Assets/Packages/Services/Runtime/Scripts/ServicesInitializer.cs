using Sirenix.OdinInspector;
using UnityEngine;

namespace Xennial.Services
{
    [DefaultExecutionOrder(-1000)]
    public class ServicesInitializer : SerializedMonoBehaviour
    {
        [SerializeField]
        private IService[] _services;

        private void Awake()
        {
            for (int i = 0; i < _services.Length; i++)
            {
                _services[i].Register();
            }
        }
    }
}