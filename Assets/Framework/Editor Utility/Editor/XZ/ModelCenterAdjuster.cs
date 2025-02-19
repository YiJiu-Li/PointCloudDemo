using UnityEngine;
using UnityEditor;

public class ModelCenterAdjuster : EditorWindow
{
    [MenuItem("Tools/调整模型中心")]
    static void ShowWindow()
    {
        GetWindow<ModelCenterAdjuster>("调整模型中心");
    }

    void OnGUI()
    {
        GUILayout.Label("选择模型并点击“调整中心”以重置其枢轴为几何中心。", EditorStyles.helpBox);
        if (GUILayout.Button("调整中心"))
        {
            AdjustModelPivotToCenter();
        }
    }

    void AdjustModelPivotToCenter()
    {
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject == null)
        {
            EditorUtility.DisplayDialog("错误", "请选择一个模型的 GameObject。", "确定");
            return;
        }

        MeshFilter[] filters = selectedObject.GetComponentsInChildren<MeshFilter>(true);
        if (filters.Length == 0)
        {
            EditorUtility.DisplayDialog("错误", "选中的对象不包含任何 MeshFilter。", "确定");
            return;
        }

        // 合并所有网格的边界
        Bounds combinedBounds = filters[0].sharedMesh.bounds;
        for (int i = 1; i < filters.Length; i++)
        {
            combinedBounds.Encapsulate(filters[i].sharedMesh.bounds);
        }

        Vector3 center = combinedBounds.center;

        // 计算需要移动的偏移量，使枢轴对齐几何中心
        Vector3 offset = center - selectedObject.transform.position;

        // 记录对象状态以支持撤销操作
        Undo.RecordObject(selectedObject, "调整模型枢轴");

        // 移动物体到新的位置
        selectedObject.transform.position += offset;

        // 遍历所有 MeshFilter，修改网格顶点实现 pivot 调整
        foreach (MeshFilter filter in filters)
        {
            Mesh originalMesh = filter.sharedMesh;
            // 创建网格的新实例，确保修改持久化而不会影响原始资源
            Mesh newMesh = Object.Instantiate(originalMesh);
            newMesh.name = originalMesh.name + " (调整后)";

            Vector3[] vertices = newMesh.vertices;

            // 平移顶点，使几何中心成为新的枢轴点
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3(
                    vertices[i].x - offset.x,
                    vertices[i].y - offset.y,
                    vertices[i].z - offset.z
                );
            }

            newMesh.vertices = vertices;
            newMesh.RecalculateBounds();
            newMesh.RecalculateNormals();
            newMesh.RecalculateTangents();

            // 记录 MeshFilter 的操作
            Undo.RecordObject(filter, "调整网格枢轴");
            filter.sharedMesh = newMesh;
        }

        EditorUtility.DisplayDialog("成功", "模型的枢轴已调整至几何中心。", "确定");
    }
}