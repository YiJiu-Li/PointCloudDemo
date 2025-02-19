/*
* 文件名：CameraLookTrigger.cs
* 作者：依旧
* 版本：#VERSION#
* Unity版本：2021.3.26f1
* 创建日期：2024/11/20 11:23:59
* 版权：© 2024 杭州西雨动画有限公司
* All rights reserved.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HQG
{
    /// <summary>
    /// 类：CameraLookTrigger
    /// 描述：此类的功能和用途...
    /// </summary>
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class CameraDynamicLookTrigger : MonoBehaviour
    {
        [Header("摄像机")]
        public Transform mCamera; // 摄像机
        [Header("动态点位列表")]
        public List<Transform> targetPoints; // 动态点位列表
        [Header("注视持续时间")]
        public float lookDuration = 3f; // 注视持续时间
        [Header("注视的角度范围")]
        public float detectionAngle = 5f; // 注视的角度范围
        [Header("最大检测距离")]
        public float detectionDistance = 10f; // 最大检测距离
        [Header("调试线段长度")]
        public float debugLineLength = 5f; // 调试线段长度
        [Header("调试线段颜色")]
        public Color debugLineColor = Color.red; // 调试线段颜色
        private bool isLookingTriggerTarget = false;
        // 注视事件，传递目标点位 Transform
        public Action<LookEntity> OnTargetLookedAt;
        private Transform currentTarget; // 当前注视的点
        private float lookTimer = 0f; // 当前注视点的计时器

        void Update()
        {
            if (mCamera == null || targetPoints == null || targetPoints.Count == 0) return;

            bool isLookingAtAnyTarget = false;

            // 循环检测摄像机是否注视每个点
            foreach (Transform targetPoint in targetPoints)
            {
                if (IsLookingAtTarget(targetPoint.position))
                {
                    // 调试线段：从摄像机画到目标点
                    Debug.DrawLine(mCamera.transform.position, targetPoint.position, debugLineColor);

                    if (currentTarget != targetPoint)
                    {
                        // 切换注视目标
                        currentTarget = targetPoint;
                        lookTimer = 0f; // 重置计时器
                    }

                    // 增加计时器
                    lookTimer += Time.deltaTime;

                    if (lookTimer >= lookDuration && !isLookingTriggerTarget)
                    {
                        TriggerCallback(currentTarget);
                    }

                    isLookingAtAnyTarget = true;
                    break; // 如果检测到注视点，停止后续检测
                }
            }

            if (!isLookingAtAnyTarget)
            {
                // 如果没有注视任何目标，重置状态
                currentTarget = null;
                lookTimer = 0f;
                isLookingTriggerTarget = false;
                if (oldTarget != null)
                {
                    oldTarget.GetComponent<LookEntity>().CloseAudio(); // 调用事件回调
                }
            }
        }

        // 判断摄像机是否注视目标点
        private bool IsLookingAtTarget(Vector3 targetPoint)
        {
            // 计算方向与距离
            Vector3 directionToTarget = targetPoint - mCamera.transform.position;
            float distanceToTarget = directionToTarget.magnitude;
            float angle = Vector3.Angle(mCamera.transform.forward, directionToTarget);

            // 调试：显示当前检测的角度和距离
            // Debug.Log($"Checking target: {targetPoint.name}, Angle: {angle}, Distance: {distanceToTarget}");

            // 判断距离和角度是否满足条件
            return angle < detectionAngle && distanceToTarget <= detectionDistance;
        }

        private Transform oldTarget;
        // 触发目标点的回调
        private void TriggerCallback(Transform target)
        {
            if (oldTarget != null)
            {
                oldTarget.GetComponent<LookEntity>().CloseAudio(); // 调用事件回调
            }
        }
    }
}