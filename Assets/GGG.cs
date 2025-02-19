/*
* 文件名：GGG.cs
* 作者：依旧
* Unity版本：2021.3.26f1
* 创建日期：2025/02/14 14:24:27
* 版权：© 2025 DefaultCompany
* All rights reserved.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HQG
{
    /// <summary>
    /// 类：GGG
    /// 描述：此类的功能和用途...
    /// </summary>
    public class GGG : MonoBehaviour
    {
        //Awake
        void Awake()
        {
            MessageDispatcher.AddListener<int, int>("Awake", (gg1, gg2) => { Util.Log("Awake" + gg1 + gg2, LogColor.Red); });
        }
        //Start 回收内存
        void Start()
        {

        }
        //待办 初始化
        void Initialize()
        {

        }
        //OnDestroy 回收内存
        void OnDestroy()
        {

        }

        private void Update()
        {
            MessageDispatcher.SendMessageData("Awake", 1, 1);
        }
    }
}