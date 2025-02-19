/*
* 文件名：Trigger.cs
* 作者：依旧
* 版本：#VERSION#
* Unity版本：2021.3.26f1
* 创建日期：2024/11/18 10:58:07
* 版权：© 2024 杭州西雨动画有限公司
* All rights reserved.
*/

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 类：Trigger
/// 描述：用于管理触发器事件，包括玩家进入、停留、退出的回调。
/// </summary>
[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public class Trigger : MonoBehaviour
{
    public event Action<Collider> OnPlayerEnter; // 玩家进入回调
    public event Action<Collider> OnPlayerStay;  // 玩家停留回调
    public event Action<Collider> OnPlayerExit;  // 玩家退出回调
    private void Awake()
    {
        // 获取触发器组件，并确保其是Trigger类型
        Collider triggerCollider = GetComponent<Collider>();
        if (!triggerCollider.isTrigger)
        {
            Debug.LogWarning("Collider is not set as a Trigger. Enabling isTrigger.");
            triggerCollider.isTrigger = true;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Debug.Log("进入触发区域");
            OnPlayerEnter?.Invoke(other);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerStay?.Invoke(other); // 玩家停留时触发
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Debug.Log("离开触发区域");
            OnPlayerExit?.Invoke(other); // 触发退出回调
        }
    }
}