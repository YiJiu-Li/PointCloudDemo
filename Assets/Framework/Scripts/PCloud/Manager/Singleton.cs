/*
* 文件名：Singleton.cs
* 作者：依旧
* 版本：1.0
* Unity版本：2021.3.26f1
* 创建日期：2024/09/02 16:56:02
* 版权：© 2024 杭州西雨动画有限公司
* All rights reserved.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 类：Singleton
/// 描述：用于非MonoBehaviour类的单例模式基类。
/// </summary>
public abstract class Singleton<T> where T : class, new()
{
    private static readonly object _lock = new object();
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                }
            }
            return _instance;
        }
    }

    // 修改非MonoBehaviour单例的Initialize方法，使其可以接受参数
    public virtual void Initialize(params object[] args) { }

    protected virtual void OnDestroy() { }
}

/// <summary>
/// 类：UnitySingleton
/// 描述：用于MonoBehaviour类的单例模式基类。
/// </summary>
public class UnitySingleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    private static readonly object _lock = new object();

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<T>();
                        if (_instance == null)
                        {
                            GameObject obj = new GameObject(typeof(T).Name);
                            _instance = obj.AddComponent<T>();
                        }
                    }
                }
            }
            return _instance;
        }
    }
    // 修改MonoBehaviour单例的Initialize方法，使其可以接受参数
    public virtual void Initialize(params object[] args) { }
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            // DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    protected virtual void OnDestroy() { }
    protected virtual void OnDisable() { }

    public void Log(string msg, LogColor color = LogColor.Green)
    {
        Util.Log(msg, color);
    }
}