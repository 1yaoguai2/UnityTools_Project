#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class ChangeName : EditorWindow
{
    private string findLetter = "";
    private string targetLetter = "";

    [MenuItem("Tools/对象操作/批量修替换名称字母")]
    static void ChangeGameObjectName()
    {
        // 创建窗口
        ChangeName window = (ChangeName)EditorWindow.GetWindow(typeof(ChangeName));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("输入查找字母和目标字母", EditorStyles.boldLabel);
        findLetter = EditorGUILayout.TextField("查找字母:", findLetter);
        targetLetter = EditorGUILayout.TextField("目标字母:", targetLetter);

        if (GUILayout.Button("确定"))
        {
            // 获取当前选中的物体
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject != null)
            {
                // 遍历选中物体的子物体
                foreach (Transform child in selectedObject.transform)
                {
                    // 如果子物体名称中包含查找字母，则替换
                    if (child.name.Contains(findLetter))
                    {
                        string newName = child.name.Replace(findLetter, targetLetter);
                        child.name = newName;
                    }
                }
            }
            else
            {
                EditorUtility.DisplayDialog("提示", "请先选择一个物体", "确定");
            }
        }
    }
}

public class SetPositionWindow : EditorWindow
{
    private float xDis = 0f;
    private float zDis = 0f;

    [MenuItem("Tools/对象操作/批量设置子物体位置")]
    static void SetGameObjectPosition()
    {
        // 创建窗口
        SetPositionWindow window = (SetPositionWindow)EditorWindow.GetWindow(typeof(SetPositionWindow));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("输入X和Z轴间距", EditorStyles.boldLabel);
        xDis = EditorGUILayout.FloatField("X轴间距:", xDis);
        zDis = EditorGUILayout.FloatField("Z轴间距:", zDis);

        if (GUILayout.Button("确定"))
        {
            // 获取当前选中的物体
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject != null)
            {
                string parentFirstLetter = selectedObject.name.Substring(0, 1);
                Vector3 basePosition = selectedObject.transform.GetChild(0).position;

                // 遍历选中物体的子物体
                foreach (Transform child in selectedObject.transform)
                {
                    // 如果子物体名称首字母与选中物体名称首字母相同
                    if (child.name.Length > 0 && child.name.Substring(0, 1) == parentFirstLetter)
                    {
                        if (child.name.Length >= 5)
                        {
                            try
                            {
                                // 获取第2-3个字符转换为int
                                int row = int.Parse(child.name.Substring(1, 2)) - 1;
                                // 获取第4-5个字符转换为int
                                int col = int.Parse(child.name.Substring(3, 2)) - 1;

                                // 计算新位置
                                Vector3 newPosition = basePosition;
                                newPosition.x += row * xDis;
                                newPosition.z += col * zDis;
                                child.position = newPosition;
                            }
                            catch (System.Exception)
                            {
                                Debug.LogWarning($"无法解析子物体 {child.name} 的名称为坐标");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"子物体 {child.name} 的名称长度不足，需要至少5个字符");
                        }
                    }
                }
            }
            else
            {
                EditorUtility.DisplayDialog("提示", "请先选择一个物体", "确定");
            }
        }
    }
}
#endif