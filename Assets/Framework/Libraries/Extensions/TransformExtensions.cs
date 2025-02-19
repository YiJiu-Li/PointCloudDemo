/*
* 文件名：TransformExtensions.cs
* 作者：依旧
* 版本：1.0
* Unity版本：2021.3.26f1
* 创建日期：2024/09/03 00:26:43
* 版权：© 2024 杭州西雨动画有限公司
* All rights reserved.
*/

using UnityEngine;
/// <summary>
/// 类：TransformExtensions
/// 描述：此类的功能和用途...
/// </summary>
public static class TransformExtensions
{
    /// <summary>
    /// 设置Transform的本地位置。
    /// </summary>
    /// <param name="transform">目标Transform</param>
    /// <param name="position">本地位置</param>
    public static void SetLocalPosition(this Transform transform, Vector3 position)
    {
        transform.localPosition = position;
    }

    /// <summary>
    /// 设置Transform的世界位置。
    /// </summary>
    /// <param name="transform">目标Transform</param>
    /// <param name="position">世界位置</param>
    public static void SetWorldPosition(this Transform transform, Vector3 position)
    {
        transform.position = position;
    }

    /// <summary>
    /// 设置Transform的本地旋转。
    /// </summary>
    /// <param name="transform">目标Transform</param>
    /// <param name="rotation">本地旋转</param>
    public static void SetLocalRotation(this Transform transform, Quaternion rotation)
    {
        transform.localRotation = rotation;
    }

    /// <summary>
    /// 设置Transform的世界旋转。
    /// </summary>
    /// <param name="transform">目标Transform</param>
    /// <param name="rotation">世界旋转</param>
    public static void SetWorldRotation(this Transform transform, Quaternion rotation)
    {
        transform.rotation = rotation;
    }

    /// <summary>
    /// 设置Transform的本地缩放。
    /// </summary>
    /// <param name="transform">目标Transform</param>
    /// <param name="scale">本地缩放</param>
    public static void SetLocalScale(this Transform transform, Vector3 scale)
    {
        transform.localScale = scale;
    }

    /// <summary>
    /// 重置Transform的本地位置、旋转和缩放。
    /// </summary>
    /// <param name="transform">目标Transform</param>
    public static void ResetLocalTransform(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    /// <summary>
    /// 将Transform的本地位置归零。
    /// </summary>
    /// <param name="transform">目标Transform</param>
    public static void ResetLocalPosition(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// 将Transform的本地旋转归零。
    /// </summary>
    /// <param name="transform">目标Transform</param>
    public static void ResetLocalRotation(this Transform transform)
    {
        transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// 将Transform的本地缩放归一。
    /// </summary>
    /// <param name="transform">目标Transform</param>
    public static void ResetLocalScale(this Transform transform)
    {
        transform.localScale = Vector3.one;
    }

    /// <summary>
    /// 查找具有指定名称的子对象。
    /// </summary>
    /// <param name="transform">目标Transform</param>
    /// <param name="name">子对象名称</param>
    /// <returns>找到的子Transform，如果未找到则返回null</returns>
    public static Transform FindChildByName(this Transform transform, string name)
    {
        foreach (Transform child in transform)
        {
            if (child.name == name) return child;
            Transform result = child.FindChildByName(name);
            if (result != null) return result;
        }
        return null;
    }

    /// <summary>
    /// 递归设置Transform及其所有子对象的层级。
    /// </summary>
    /// <param name="transform">目标Transform</param>
    /// <param name="layer">层级</param>
    public static void SetLayerRecursively(this Transform transform, int layer)
    {
        transform.gameObject.layer = layer;
        foreach (Transform child in transform)
        {
            child.SetLayerRecursively(layer);
        }
    }

    /// <summary>
    /// 获取Transform的完整路径（从根对象到当前对象）。
    /// </summary>
    /// <param name="transform">目标Transform</param>
    /// <returns>Transform的完整路径</returns>
    public static string GetFullPath(this Transform transform)
    {
        string path = "/" + transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = "/" + transform.name + path;
        }
        return path;
    }

    /// <summary>
    /// 将Transform的子对象排序（根据名称）。
    /// </summary>
    /// <param name="transform">目标Transform</param>
    public static void SortChildrenByName(this Transform transform)
    {
        var children = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }

        System.Array.Sort(children, (a, b) => string.Compare(a.name, b.name));

        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetSiblingIndex(i);
        }
    }

    /// <summary>
    /// 获取Transform的所有子对象。
    /// </summary>
    /// <param name="transform">目标Transform</param>
    /// <returns>子Transform数组</returns>
    public static Transform[] GetAllChildren(this Transform transform)
    {
        Transform[] children = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }
        return children;
    }

    /// <summary>
    /// 设置Transform的父对象。
    /// </summary>
    /// <param name="transform">目标Transform</param>
    /// <param name="parent">新父对象</param>
    public static void SetParent(this Transform transform, Transform parent)
    {
        transform.SetParent(parent);
    }

    /// <summary>
    /// 获取Transform到另一个Transform的距离。
    /// </summary>
    /// <param name="transform">目标Transform</param>
    /// <param name="other">另一个Transform</param>
    /// <returns>两个Transform之间的距离</returns>
    public static float DistanceTo(this Transform transform, Transform other)
    {
        return Vector3.Distance(transform.position, other.position);
    }

    /// <summary>
    /// 将Transform移动到指定位置。
    /// </summary>
    /// <param name="transform">目标Transform</param>
    /// <param name="position">目标位置</param>
    /// <param name="speed">移动速度</param>
    public static void MoveTowards(this Transform transform, Vector3 position, float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, position, speed * Time.deltaTime);
    }

    /// <summary>
    /// 将Transform旋转到指定角度。
    /// </summary>
    /// <param name="transform">目标Transform</param>
    /// <param name="rotation">目标角度</param>
    /// <param name="speed">旋转速度</param>
    public static void RotateTowards(this Transform transform, Quaternion rotation, float speed)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, speed * Time.deltaTime);
    }

    /// <summary>
    /// 使Transform朝向目标对象。
    /// </summary>
    /// <param name="transform">目标Transform</param>
    /// <param name="target">目标对象</param>
    public static void LookAtTarget(this Transform transform, Transform target, Vector3 worldUp)
    {
        // transform.LookAt(target,worldUp);
        // 获取目标的位置
        Vector3 targetPosition = target.position;

        // 计算目标与物体之间的方向
        Vector3 direction = targetPosition - transform.position;

        // 将方向的 Y 分量设为 0，以只控制 Y 轴的旋转
        direction.y = 0;

        // 计算目标的旋转
        if (direction != Vector3.zero) // 确保方向不是零向量
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = rotation;
        }
    }

    /// <summary>
    /// 检查Transform是否是另一个Transform的子对象。
    /// </summary>
    /// <param name="transform">目标Transform</param>
    /// <param name="parent">父Transform</param>
    /// <returns>如果是子对象则返回true，否则返回false</returns>
    public static bool IsChildOf(this Transform transform, Transform parent)
    {
        return transform.IsChildOf(parent);
    }
}
