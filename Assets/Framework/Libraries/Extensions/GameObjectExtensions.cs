/*
* 文件名：GameObjectExtensions.cs
* 作者：依旧
* 版本：1.0
* Unity版本：2021.3.26f1
* 创建日期：2024/09/03 00:25:59
* 版权：© 2024 杭州西雨动画有限公司
* All rights reserved.
*/

using UnityEngine;
/// <summary>
/// 类：GameObjectExtensions
/// 描述：此类的功能和用途...
/// </summary>
public static class GameObjectExtensions
{
    /// <summary>
    /// 查找或创建子对象。如果在父对象中找到具有指定名称的子对象，则返回该子对象；
    /// 否则，创建一个新的子对象并返回。
    /// </summary>
    /// <param name="parent">父对象</param>
    /// <param name="childName">子对象名称</param>
    /// <returns>找到或新创建的子对象</returns>
    public static GameObject FindOrCreateChild(this GameObject parent, string childName)
    {
        Transform child = parent.transform.Find(childName);
        if (child != null)
        {
            return child.gameObject;
        }
        GameObject newChild = new GameObject(childName);
        newChild.transform.SetParent(parent.transform);
        return newChild;
    }

    /// <summary>
    /// 递归设置GameObject及其所有子对象的激活状态。
    /// </summary>
    /// <param name="gameObject">目标对象</param>
    /// <param name="state">激活状态</param>
    public static void SetActiveRecursively(this GameObject gameObject, bool state)
    {
        gameObject.SetActive(state);
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.SetActive(state);
        }
    }

    /// <summary>
    /// 获取子对象中的指定组件类型。如果需要，可以包括未激活的对象。
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="gameObject">目标对象</param>
    /// <param name="includeInactive">是否包括未激活的子对象</param>
    /// <returns>找到的组件</returns>
    public static T GetComponentInChildren<T>(this GameObject gameObject, bool includeInactive = false)
    {
        return gameObject.GetComponentInChildren<T>(includeInactive);
    }

    /// <summary>
    /// 获取指定类型的组件，如果该组件不存在则添加一个新的组件。
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="gameObject">目标对象</param>
    /// <returns>获取或添加的组件</returns>
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }
        return component;
    }

    /// <summary>
    /// 销毁GameObject的所有子对象。
    /// </summary>
    /// <param name="gameObject">目标对象</param>
    public static void DestroyAllChildren(this GameObject gameObject)
    {
        foreach (Transform child in gameObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// 递归查找具有指定名称的子对象。
    /// </summary>
    /// <param name="gameObject">目标对象</param>
    /// <param name="name">子对象名称</param>
    /// <returns>找到的子对象，如果未找到则返回null</returns>
    public static GameObject FindChildByName(this GameObject gameObject, string name)
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.name == name) return child.gameObject;
            GameObject result = child.gameObject.FindChildByName(name);
            if (result != null) return result;
        }
        return null;
    }

    /// <summary>
    /// 设置GameObject及其所有子对象的层级。
    /// </summary>
    /// <param name="gameObject">目标对象</param>
    /// <param name="layer">层级</param>
    public static void SetLayerRecursively(this GameObject gameObject, int layer)
    {
        gameObject.layer = layer;
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.SetLayerRecursively(layer);
        }
    }

    /// <summary>
    /// 检查GameObject是否具有指定名称的子对象。
    /// </summary>
    /// <param name="gameObject">目标对象</param>
    /// <param name="childName">子对象名称</param>
    /// <returns>如果找到子对象则返回true，否则返回false</returns>
    public static bool HasChild(this GameObject gameObject, string childName)
    {
        return gameObject.transform.Find(childName) != null;
    }

    /// <summary>
    /// 复制GameObject及其所有子对象。
    /// </summary>
    /// <param name="gameObject">目标对象</param>
    /// <returns>复制的GameObject</returns>
    public static GameObject Clone(this GameObject gameObject)
    {
        return GameObject.Instantiate(gameObject);
    }

    /// <summary>
    /// 隐藏GameObject及其所有子对象。
    /// </summary>
    /// <param name="gameObject">目标对象</param>
    public static void Hide(this GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 显示GameObject及其所有子对象。
    /// </summary>
    /// <param name="gameObject">目标对象</param>
    public static void Show(this GameObject gameObject)
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 获取GameObject的可见性。
    /// </summary>
    /// <param name="gameObject">目标GameObject</param>
    /// <returns>如果GameObject是可见的，则返回true，否则返回false。</returns>
    public static bool IsVisible(this GameObject gameObject)
    {
        return gameObject.activeSelf;
    }


    /// <summary>
    /// 获取GameObject的所有子对象。
    /// </summary>
    /// <param name="gameObject">目标对象</param>
    /// <returns>子对象数组</returns>
    public static GameObject[] GetAllChildren(this GameObject gameObject)
    {
        GameObject[] children = new GameObject[gameObject.transform.childCount];
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            children[i] = gameObject.transform.GetChild(i).gameObject;
        }
        return children;
    }

    /// <summary>
    /// 在GameObject下添加一个新的子对象。
    /// </summary>
    /// <param name="gameObject">目标对象</param>
    /// <param name="childName">子对象名称</param>
    /// <returns>新添加的子对象</returns>
    public static GameObject AddChild(this GameObject gameObject, string childName)
    {
        GameObject child = new GameObject(childName);
        child.transform.SetParent(gameObject.transform);
        return child;
    }

    /// <summary>
    /// 检查GameObject是否具有指定的组件。
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="gameObject">目标对象</param>
    /// <returns>如果具有组件则返回true，否则返回false</returns>
    public static bool HasComponent<T>(this GameObject gameObject) where T : Component
    {
        return gameObject.GetComponent<T>() != null;
    }

    /// <summary>
    /// 获取GameObject及其所有子对象中的所有指定类型的组件。
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="gameObject">目标对象</param>
    /// <returns>找到的组件数组</returns>
    public static T[] GetComponentsInChildrenRecursively<T>(this GameObject gameObject)
    {
        return gameObject.GetComponentsInChildren<T>(true);
    }

    /// <summary>
    /// 根据名称查找或创建组件。
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="gameObject">目标对象</param>
    /// <param name="name">组件名称</param>
    /// <returns>找到或新创建的组件</returns>
    public static T FindOrCreateComponent<T>(this GameObject gameObject, string name) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }
        return component;
    }

    /// <summary>
    /// 根据名称禁用指定的组件。
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="gameObject">目标对象</param>
    /// <param name="name">组件名称</param>
    public static void DisableComponent<T>(this GameObject gameObject, string name) where T : MonoBehaviour
    {
        T component = gameObject.GetComponent<T>();
        if (component != null)
        {
            component.enabled = false;
        }
    }

    /// <summary>
    /// 根据名称启用指定的组件。
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="gameObject">目标对象</param>
    /// <param name="name">组件名称</param>
    public static void EnableComponent<T>(this GameObject gameObject, string name) where T : MonoBehaviour
    {
        T component = gameObject.GetComponent<T>();
        if (component != null)
        {
            component.enabled = true;
        }
    }

    /// <summary>
    /// 移除GameObject中的指定组件。
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="gameObject">目标对象</param>
    public static void RemoveComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component != null)
        {
            GameObject.Destroy(component);
        }
    }

    /// <summary>
    /// 获取GameObject的完整路径（从根对象到当前对象）。
    /// </summary>
    /// <param name="gameObject">目标对象</param>
    /// <returns>对象的完整路径</returns>
    public static string GetFullPath(this GameObject gameObject)
    {
        return gameObject.transform.GetFullPath();
    }
    /// <summary>
    /// 获取物体的 Pivot 点（Transform.position）
    /// </summary>
    /// <param name="obj">目标物体</param>
    /// <returns>Pivot 点的世界坐标</returns>
    public static Vector3 GetPivotPoint(this GameObject obj)
    {
        return obj.transform.position;
    }

    /// <summary>
    /// 递归计算所有子节点的位置中心点
    /// </summary>
    /// <param name="obj">目标物体</param>
    /// <returns>所有子节点位置的平均值（世界坐标）</returns>
    public static Vector3 GetCenterPoint(this GameObject obj)
    {
        Vector3 center = Vector3.zero; // 初始化中心点
        int count = 0; // 节点数量计数

        // 递归计算所有子节点的位置
        CalculateCenterRecursive(obj.transform, ref center, ref count);

        if (count == 0)
        {
            Debug.LogWarning($"物体 {obj.name} 及其子物体没有有效的 Transform，返回自身位置作为 Center Point。");
            return obj.transform.position; // 没有子物体时，返回自身位置
        }

        return center / count; // 平均值作为中心点
    }

    /// <summary>
    /// 递归计算子物体的位置总和和数量
    /// </summary>
    private static void CalculateCenterRecursive(Transform node, ref Vector3 center, ref int count)
    {
        center += node.position; // 累加当前节点的位置
        count++; // 增加节点数量

        // 递归子节点
        foreach (Transform child in node)
        {
            CalculateCenterRecursive(child, ref center, ref count);
        }
    }

}
