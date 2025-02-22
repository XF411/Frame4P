using UnityEngine;


namespace Fream4P.Core 
{
    /// <summary>
    /// 简易的单例模式实现
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        public static T Instance { get; private set; }

        public static T CreateInstance()
        {
            if (Instance != null)
                return Instance;

            var go = new GameObject(typeof(T).Name);
            Instance = go.AddComponent<T>();
            DontDestroyOnLoad(go);
            return Instance;
        }

        public static void DestroyInstance()
        {
            if (!Instance)
            {
                Debug.LogError($"Singleton[{typeof(T).Name}] has been disposed, or not yet be created");
                return;
            }
            GameObject.Destroy(Instance.gameObject);
            Instance = null;
        }
    }
}
