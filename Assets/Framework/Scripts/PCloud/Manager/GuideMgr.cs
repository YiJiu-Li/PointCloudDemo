/*
* 文件名：GuideMgr.cs
* 作者：依旧
* 版本：#VERSION#
* Unity版本：2021.3.26f1
* 创建日期：2025/01/13 16:43:04
* 版权：© 2025 杭州西雨动画有限公司
* All rights reserved.
*/

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using YZJ;

namespace HQG
{
    /// <summary>
    /// 类：GuideMgr
    /// 描述：此类的功能和用途...
    /// </summary>
    public class GuideMgr : UnitySingleton<GuideMgr>
    {
        public GameObject GuideRoot;
        public RawImage tipRoot;
        private Animator GuideAdmin;
        public bool isGuideEnd;
        public CancellationTokenSource cancellationTokenSource;
        private new void Awake()
        {
            base.Awake();
            GuideAdmin = GuideRoot.GetComponent<Animator>();
            cancellationTokenSource = new CancellationTokenSource();
        }
        void Start()
        {
            GuideRoot.gameObject.Show();
            AudioMgr.Instance.PlayGuideFromResources("Start");
            AudioMgr.Instance.PlayBGMFromResourcesAsync("BGM");
            AudioMgr.Instance.SetVolume(AudioMgr.BGM_TYPE, 0.1f);
            this.DelaySeconds(10, () =>
            {
                GuideAdmin.SetTrigger("Close");
                this.DelaySeconds(1, () =>
                {
                    isGuideEnd = true;
                }, cancellationTokenSource.Token).Forget();
            }, cancellationTokenSource.Token).Forget();
        }

        public void CanceGuide()
        {
            AudioMgr.Instance.StopAudio(AudioMgr.GUIDE_TYPE);
            GuideRoot.gameObject.Hide();
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
        }

    }
}