using UnityEngine;

/// <summary>
/// This is a helper class to implement the Singleton pattern without needing to rewrite Singleton logic.
/// Use this class to turn any existing class into a singleton quickly and easily.
/// Instructions:
/// Inherit from Singleton instead of MonoBehaviour and use the name of your class as the type.
/// Example:
/// public class GameManager : Singleton<GameManager>
/// </summary>
/// <typeparam name="T"></typeparam>

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    instance = obj.AddComponent<T>();
                }
            }
            
            return instance;
        }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
