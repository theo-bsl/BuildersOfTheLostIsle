using UnityEngine;

namespace Tools.Singleton
{
    [DefaultExecutionOrder(-100)]
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _instance;
    
        [Header("Singleton Settings")]
        [SerializeField] protected bool _needDontDestroyOnLoad = false;

        public static T Instance
        {
            get
            {
                if (_instance) 
                    return _instance;
            
                Debug.LogError("New " + typeof(T) + " Instantiated");
                return new GameObject("New " + typeof(T)).AddComponent<T>();

            }
        }

        private void Awake()
        {
            if (_instance != null)
            {
                Debug.LogError(typeof(T) + " Already exist");
                Destroy(this.gameObject);
                return;
            }

            if (_needDontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);

            _instance = this as T;
        }
    }
}