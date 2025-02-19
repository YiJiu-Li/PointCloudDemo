using System;
using System.Collections.Generic;
using UnityEngine;

// 在文件顶部添加委托定义
public delegate void MessageCallBack();
public delegate void MessageCallBack<T>(T arg);
public delegate void MessageCallBack<T1, T2>(T1 arg1, T2 arg2);
public delegate void MessageCallBack<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);

public class MessageDispatcher
{
    // 使用线程安全的字典
    private static readonly Dictionary<object, Delegate> MessageEvents = new Dictionary<object, Delegate>();
    private static readonly object lockObject = new object();

    #region 消息广播
    public static void SendMessageData(object key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        Delegate d;
        lock (lockObject)
        {
            if (!MessageEvents.TryGetValue(key, out d)) return;
        }

        if (d is MessageCallBack callback)
        {
            try
            {
                callback();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"广播事件执行错误: {key}, {ex}");
            }
        }
        else
        {
            UnityEngine.Debug.LogError($"广播事件类型错误: 事件{key}对应的委托类型不匹配");
        }
    }

    public static void SendMessageData<T>(object key, T arg)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        Delegate d;
        lock (lockObject)
        {
            if (!MessageEvents.TryGetValue(key, out d)) return;
        }

        if (d is MessageCallBack<T> callback)
        {
            try
            {
                callback(arg);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"广播事件执行错误: {key}, {ex}");
            }
        }
        else
        {
            UnityEngine.Debug.LogError($"广播事件类型错误: 事件{key}对应的委托类型不匹配");
        }
    }

    public static void SendMessageData<T1, T2>(object key, T1 arg1, T2 arg2)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        Delegate d;
        lock (lockObject)
        {
            if (!MessageEvents.TryGetValue(key, out d)) return;
        }

        if (d is MessageCallBack<T1, T2> callback)
        {
            try
            {
                callback(arg1, arg2);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"广播事件执行错误: {key}, {ex}");
            }
        }
        else
        {
            UnityEngine.Debug.LogError($"广播事件类型错误: 事件{key}对应的委托类型不匹配");
        }
    }

    public static void SendMessageData<T1, T2, T3>(object key, T1 arg1, T2 arg2, T3 arg3)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        Delegate d;
        lock (lockObject)
        {
            if (!MessageEvents.TryGetValue(key, out d)) return;
        }

        if (d is MessageCallBack<T1, T2, T3> callback)
        {
            try
            {
                callback(arg1, arg2, arg3);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"广播事件执行错误: {key}, {ex}");
            }
        }
        else
        {
            UnityEngine.Debug.LogError($"广播事件类型错误: 事件{key}对应的委托类型不匹配");
        }
    }
    #endregion

    #region 消息监听
    public static void AddListener(object key, MessageCallBack callback)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (callback == null) throw new ArgumentNullException(nameof(callback));

        lock (lockObject)
        {
            if (!MessageEvents.TryGetValue(key, out Delegate d))
            {
                MessageEvents[key] = callback;
                return;
            }

            if (d != null && d.GetType() != callback.GetType())
            {
                throw new InvalidOperationException($"委托类型不匹配: 当前类型 {d.GetType()}, 新类型 {callback.GetType()}");
            }

            MessageEvents[key] = Delegate.Combine(d, callback);
        }
    }

    public static void AddListener<T>(object key, MessageCallBack<T> callback)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (callback == null) throw new ArgumentNullException(nameof(callback));

        lock (lockObject)
        {
            if (!MessageEvents.TryGetValue(key, out Delegate d))
            {
                MessageEvents[key] = callback;
                return;
            }

            if (d != null && d.GetType() != callback.GetType())
            {
                throw new InvalidOperationException($"委托类型不匹配: 当前类型 {d.GetType()}, 新类型 {callback.GetType()}");
            }

            MessageEvents[key] = Delegate.Combine(d, callback);
        }
    }

    public static void AddListener<T1, T2>(object key, MessageCallBack<T1, T2> callback)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (callback == null) throw new ArgumentNullException(nameof(callback));

        lock (lockObject)
        {
            if (!MessageEvents.TryGetValue(key, out Delegate d))
            {
                MessageEvents[key] = callback;
                return;
            }

            if (d != null && d.GetType() != callback.GetType())
            {
                throw new InvalidOperationException($"委托类型不匹配: 当前类型 {d.GetType()}, 新类型 {callback.GetType()}");
            }

            MessageEvents[key] = Delegate.Combine(d, callback);
        }
    }

    public static void AddListener<T1, T2, T3>(object key, MessageCallBack<T1, T2, T3> callback)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (callback == null) throw new ArgumentNullException(nameof(callback));

        lock (lockObject)
        {
            if (!MessageEvents.TryGetValue(key, out Delegate d))
            {
                MessageEvents[key] = callback;
                return;
            }

            if (d != null && d.GetType() != callback.GetType())
            {
                throw new InvalidOperationException($"委托类型不匹配: 当前类型 {d.GetType()}, 新类型 {callback.GetType()}");
            }

            MessageEvents[key] = Delegate.Combine(d, callback);
        }
    }
    #endregion

    #region 消息监听移除
    public static void RemoveListener(object key, MessageCallBack callback)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (callback == null) throw new ArgumentNullException(nameof(callback));

        lock (lockObject)
        {
            if (!MessageEvents.TryGetValue(key, out Delegate d))
            {
                UnityEngine.Debug.LogWarning($"移除监听失败: 未找到事件 {key}");
                return;
            }

            if (d.GetType() != callback.GetType())
            {
                throw new InvalidOperationException($"委托类型不匹配: 当前类型 {d.GetType()}, 移除类型 {callback.GetType()}");
            }

            d = Delegate.Remove(d, callback);
            if (d != null)
                MessageEvents[key] = d;
            else
                MessageEvents.Remove(key);
        }
    }

    public static void RemoveListener<T>(object key, MessageCallBack<T> callback)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (callback == null) throw new ArgumentNullException(nameof(callback));

        lock (lockObject)
        {
            if (!MessageEvents.TryGetValue(key, out Delegate d))
            {
                UnityEngine.Debug.LogWarning($"移除监听失败: 未找到事件 {key}");
                return;
            }

            if (d.GetType() != callback.GetType())
            {
                throw new InvalidOperationException($"委托类型不匹配: 当前类型 {d.GetType()}, 移除类型 {callback.GetType()}");
            }

            d = Delegate.Remove(d, callback);
            if (d != null)
                MessageEvents[key] = d;
            else
                MessageEvents.Remove(key);
        }
    }

    public static void RemoveListener<T1, T2>(object key, MessageCallBack<T1, T2> callback)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (callback == null) throw new ArgumentNullException(nameof(callback));

        lock (lockObject)
        {
            if (!MessageEvents.TryGetValue(key, out Delegate d))
            {
                UnityEngine.Debug.LogWarning($"移除监听失败: 未找到事件 {key}");
                return;
            }

            if (d.GetType() != callback.GetType())
            {
                throw new InvalidOperationException($"委托类型不匹配: 当前类型 {d.GetType()}, 移除类型 {callback.GetType()}");
            }

            d = Delegate.Remove(d, callback);
            if (d != null)
                MessageEvents[key] = d;
            else
                MessageEvents.Remove(key);
        }
    }

    public static void RemoveListener<T1, T2, T3>(object key, MessageCallBack<T1, T2, T3> callback)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (callback == null) throw new ArgumentNullException(nameof(callback));

        lock (lockObject)
        {
            if (!MessageEvents.TryGetValue(key, out Delegate d))
            {
                UnityEngine.Debug.LogWarning($"移除监听失败: 未找到事件 {key}");
                return;
            }

            if (d.GetType() != callback.GetType())
            {
                throw new InvalidOperationException($"委托类型不匹配: 当前类型 {d.GetType()}, 移除类型 {callback.GetType()}");
            }

            d = Delegate.Remove(d, callback);
            if (d != null)
                MessageEvents[key] = d;
            else
                MessageEvents.Remove(key);
        }
    }
    #endregion

    public static void ClearAllListeners()
    {
        lock (lockObject)
        {
            MessageEvents.Clear();
        }
    }
}