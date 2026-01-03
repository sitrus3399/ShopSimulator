using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            DoOnAwake();
        }
        else
        {
            DoOnDestroy();
            Destroy(gameObject);
        }
    }

    protected virtual void DoOnDestroy()
    {
        
    }

    protected virtual void DoOnAwake()
    {
       
    }
}

public class SingletonDontDestroy<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
