using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace YZJ
{
    public static class AutoReferencer
    {
        // 由编辑器调用的方法，遍历目标对象并调用其上的 "CalledByEditor" 方法
        public static void CalledByEditor(IEnumerable<Object> targets)
        {
            foreach (Object item in targets)
            {
                Transform tf = item as Transform;
                MonoBehaviour[] monos = tf.GetComponents<MonoBehaviour>();
                Undo.RecordObjects(monos, "CalledByEditor");
                for (int i = 0; i < monos.Length; i++)
                {
                    // 找到脚本上的 "CalledByEditor" 方法并调用
                    MethodInfo methodInfo = monos[i].GetType()
                                                    .GetMethod("CalledByEditor", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (methodInfo == null)
                    {
                        continue;
                    }
                    Debug.Log(monos[i].name + " Method CalledByEditor Invoke");
                    methodInfo.Invoke(monos[i], null);
                }
            }
        }

        // 查找引用的方法，遍历目标对象并调用其上的 "FindReferences" 方法
        public static void FindReferences(IEnumerable<Object> targets)
        {
            foreach (Object item in targets)
            {
                Transform tf = item as Transform;
                MonoBehaviour[] monos = tf.GetComponents<MonoBehaviour>();
                Undo.RecordObjects(monos, "FindReferences");
                for (int i = 0; i < monos.Length; i++)
                {
                    // 按照变量名自动找引用
                    FindReferences(monos[i]);
                }
            }
        }

        // 查找引用的具体实现，遍历 MonoBehaviour 的字段并自动填充引用
        private static void FindReferences(MonoBehaviour mono)
        {
            foreach (FieldInfo field in mono.GetType().GetFields()) // 遍历类的变量
            {
                object objValue = field.GetValue(mono);
                Type fieldType = field.FieldType;

                #region 自动找数组功能

                // 如果是数组
                if (fieldType.IsArray)
                {
                    // 判断是不是空的数组或者数组元素有填充了，就跳过
                    object[] objs = objValue as object[];
                    if (objs == null || objs.Length > 0)
                    {
                        continue;
                    }

                    // 处理 GameObject 数组
                    Array filledArray;
                    Type elementType = fieldType.GetElementType();
                    if (elementType == typeof(GameObject))
                    {
                        Transform[] tfs = mono.GetComponentsInChildren<Transform>();
                        Transform[] tfHits = Array.FindAll(tfs, item => item.name.StartsWith(field.Name, StringComparison.OrdinalIgnoreCase));
                        int nLength = tfHits.Length;
                        GameObject[] gos = new GameObject[nLength];
                        for (int i = 0; i < nLength; i++)
                        {
                            gos[i] = tfHits[i].gameObject;
                        }

                        filledArray = Array.CreateInstance(elementType, nLength);
                        Array.Copy(gos, filledArray, nLength);
                        field.SetValue(mono, filledArray);
                        continue;
                    }

                    // 处理其他类型的数组
                    Component[] coms = mono.GetComponentsInChildren(elementType);
                    Component[] comHits = Array.FindAll(coms, item => item.name.StartsWith(field.Name, StringComparison.OrdinalIgnoreCase));
                    if (elementType != null)
                    {
                        filledArray = Array.CreateInstance(elementType, comHits.Length);
                        Array.Copy(comHits, filledArray, comHits.Length);
                        field.SetValue(mono, filledArray);
                    }

                    continue;
                }
                #endregion

                // 判断字段值是否为空
                if (objValue != null)
                {
                    Object uObject = objValue as Object;
                    if (uObject)
                    {
                        continue;
                    }
                }

                // 查找自身的变量
                if (field.Name.Contains("Self"))
                {
                    if (fieldType == typeof(GameObject))
                    {
                        field.SetValue(mono, mono.gameObject);
                        continue;
                    }

                    Component com = mono.GetComponent(fieldType);
                    if (com)
                    {
                        field.SetValue(mono, com);
                        continue;
                    }
                }

                // 迭代遍历子物体看看有没有同名的
                FieldInfo info = field;
                // Transform tf = mono.transform.FindChildRecursion(info.Name, StringComparison.OrdinalIgnoreCase);
                // if (tf == null)
                // {
                //     continue;
                // }

                // 赋值操作
                // if (fieldType == typeof(GameObject))
                // {
                //     field.SetValue(mono, tf.gameObject);
                // }
                // else
                // {
                //     field.SetValue(mono, tf.GetComponent(fieldType));
                // }
            }
        }
    }
}
