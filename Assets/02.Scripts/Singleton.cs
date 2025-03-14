using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    //게임이 시작될 때 특정 시점에서 호출 (씬이 로드되기 전에 실행)
    public static void OnBeforeSceneLoadRuntimeMethod()
    {
        
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
            
            // ✅ 최초 인스턴스에서만 씬 전환 구독 추가
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            OnAwake();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public virtual void OnAwake()
    {
    }

    protected abstract void OnSceneLoaded(Scene scene, LoadSceneMode mode);
}