#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// 或者使用unity自带的GameObject菜单中的Center On Children
/// </summary>
public class CenterParentTool : EditorWindow
{
    [MenuItem("Tools/对象操作/Center Parent to Children")]
    private static void CenterParentToChildren()
    {
        // 获取所有选中的物体
        Transform[] selectedTransforms = Selection.transforms;

        if (selectedTransforms == null || selectedTransforms.Length == 0)
        {
            EditorUtility.DisplayDialog("父物体居中", "请先选择至少一个父物体", "确定");
            return;
        }

        // 处理每个选中的父物体
        foreach (Transform parentTransform in selectedTransforms)
        {
            if (parentTransform.childCount == 0)
            {
                Debug.LogWarning($"物体 '{parentTransform.name}' 没有子物体，已跳过处理");
                continue;
            }

            // 计算所有子物体的中心点
            Vector3 centerPoint = Vector3.zero;
            int childCount = parentTransform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                centerPoint += parentTransform.GetChild(i).position;
            }

            centerPoint /= childCount;

            // 记录当前状态用于撤销
            Undo.RecordObject(parentTransform, "Center Parent to Children");

            // 移动父物体到中心点
            Vector3 offset = centerPoint - parentTransform.position;
            parentTransform.position = centerPoint;

            // 调整所有子物体的位置以保持它们的世界坐标不变
            for (int i = 0; i < childCount; i++)
            {
                Transform child = parentTransform.GetChild(i);
                Undo.RecordObject(child, "Center Parent to Children");
                child.position -= offset;
            }
        }
    }
}
#endif