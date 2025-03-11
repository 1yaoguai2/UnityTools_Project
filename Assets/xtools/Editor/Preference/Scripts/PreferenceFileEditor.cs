using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;
using Debug = UnityEngine.Debug;

namespace XTools
{
    [CustomEditor(typeof(ReadmeScriptableObject))]
    [InitializeOnLoad]
    public class PreferenceFileEditor : Editor
    {

        static string kShowedReadmeSessionStateName = "ReadmeScriptableObject.showedReadme";

        static float kSpace = 16f;

        private static readonly MethodInfo s_LoadWindowLayoutMethod;
        private static readonly MethodInfo s_SaveWindowLayoutMethod;
        private static string assetPath = Application.dataPath + "/Preference/Editor/Layout.wlt";

        static PreferenceFileEditor()
        {
            EditorApplication.delayCall += SelectReadmeAutomatically;
            EditorApplication.quitting += SaveReadme;
            var typeWindowLayout = Type.GetType("UnityEditor.WindowLayout,UnityEditor");
            if (typeWindowLayout != null)
            {
                s_LoadWindowLayoutMethod = typeWindowLayout.GetMethod(("LoadWindowLayout"),
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null,
                    new Type[] { typeof(string), typeof(bool) }, null);
                s_SaveWindowLayoutMethod = typeWindowLayout.GetMethod(("SaveWindowLayout"),
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null,
                    new Type[] { typeof(string) }, null);
            }
        }

        static void SelectReadmeAutomatically()
        {
            if (!SessionState.GetBool(kShowedReadmeSessionStateName, false))
            {
                var readme = SelectReadme();
                SessionState.SetBool(kShowedReadmeSessionStateName, true);

                if (readme)
                {
                    //利用电脑标识符，判断是否是新电脑打开
                    string currentComputerID = SystemInfo.deviceUniqueIdentifier;
                    if (string.IsNullOrEmpty(readme.computerID) || !currentComputerID.Equals(readme.computerID))
                    {
                        Debug.Log($"新电脑标识符{currentComputerID}:{readme.computerID}");
                        readme.computerID = currentComputerID;
                        LoadLayout();
                    }
                }
            }
        }

        static void SaveReadme()
        {
            var readme = SelectReadme();
            SessionState.SetBool(kShowedReadmeSessionStateName, true);

            if (readme)
            {
                //必须要写这个，要不然的化，光下面的那个无法保存ScriptableObject
                EditorUtility.SetDirty((ScriptableObject)readme);
                AssetDatabase.SaveAssets();
            }
        }


        static void LoadLayout()
        {
            LoadLayoutFromAsset();
        }

        /// <summary>
        /// 保存Layout到资源文件
        /// </summary>
        [MenuItem("Tools/Preference/Save Window Layout")]
        public static void SaveLayoutToAsset()
        {
            if (s_SaveWindowLayoutMethod == null)
                return;
            var path = Path.Combine(Directory.GetCurrentDirectory(), assetPath);
            s_SaveWindowLayoutMethod.Invoke(null, new object[] { path });
            Debug.Log($"当前布局保存到{assetPath}成功！");
        }

        /// <summary>
        /// 加载.wlt文件
        /// </summary>
        [MenuItem("Tools/Preference/Load Window Layout")]
        public static void LoadLayoutFromAsset()
        {
#if UNITY_2023_1_OR_NEWER
        LoadLayoutFromAsset6();
#else
            if (s_LoadWindowLayoutMethod == null)
                return;
            if (!File.Exists(assetPath))
            {
                //可能是菜单栏Tutorial忘记保存布局，生成布局文件
                Debug.LogError($"加载布局文件出错：找不到{assetPath}文件");
                return;
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), assetPath);
            s_LoadWindowLayoutMethod.Invoke(null, new object[] { path, false });
            Debug.Log($"加载布局文件{assetPath}成功！");
#endif
        }

        /// <summary>
        /// unity6.0版本专用
        /// </summary>
        private static void LoadLayoutFromAsset6()
        {
            try
            {
                if (!File.Exists(assetPath))
                {
                    new Exception($"6.0版本，加载布局文件出错：找不到{assetPath}文件");
                }

                //EditorWindowLayout.LoadLayout(assetPath);
                Debug.Log($"加载布局文件{assetPath}成功！");
            }
            catch (Exception e)
            {
                Debug.LogError($"6.0版本加载布局文件出错：{e.Message}");
            }
        }



        /// <summary>
        /// 选中Readme资产
        /// </summary>
        /// <returns></returns>
        [MenuItem("Tools/Preference/Show Tutorial Instructions")]
        static ReadmeScriptableObject SelectReadme()
        {
            var ids = AssetDatabase.FindAssets("Readme t:ReadmeScriptableObject");
            if (ids.Length == 1)
            {
                var readmeObject = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(ids[0]));

                Selection.objects = new UnityEngine.Object[] { readmeObject };

                return (ReadmeScriptableObject)readmeObject;
            }
            else
            {
                Debug.Log("Couldn't find a ReadmeScriptableObject");
                return null;
            }
        }

        /// <summary>
        /// 创建Readme资产
        /// </summary>
        /// <returns></returns>
        [MenuItem("Tools/Preference/CreateReadmeFile")]
        public static ReadmeScriptableObject CreateReadmeFile()
        {
            var ids = AssetDatabase.FindAssets("Readme t:ReadmeScriptableObject");
            if (ids.Length == 1)
            {
                var readmeObject = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(ids[0]));

                Selection.objects = new UnityEngine.Object[] { readmeObject };
                Debug.LogError("Readme,已经存在");
                return (ReadmeScriptableObject)readmeObject;
            }
            else
            {
                ReadmeScriptableObject file = CreateInstance<ReadmeScriptableObject>();
                file.sections = new ReadmeScriptableObject.Section[0];
                AssetDatabase.CreateAsset(file, "Assets/Readme.asset");
                AssetDatabase.Refresh();
                return file;
            }

        }

        protected override void OnHeaderGUI()
        {
            var readme = (ReadmeScriptableObject)target;
            Init();

            var iconWidth = Mathf.Min(EditorGUIUtility.currentViewWidth / 3f - 20f, 128f);

            GUILayout.BeginHorizontal("In BigTitle");
            {
                GUILayout.Label(readme.icon, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth));
                GUILayout.Label(readme.title, TitleStyle);
            }
            GUILayout.EndHorizontal();
        }

        public override void OnInspectorGUI()
        {
            var readme = (ReadmeScriptableObject)target;
            Init();

            foreach (var section in readme.sections)
            {
                if (!string.IsNullOrEmpty(section.heading))
                {
                    GUILayout.Label(section.heading, HeadingStyle);
                }

                if (!string.IsNullOrEmpty(section.text))
                {
                    GUILayout.Label(section.text, BodyStyle);
                }

                if (!string.IsNullOrEmpty(section.linkText))
                {
                    if (LinkLabel(new GUIContent(section.linkText)))
                    {
                        Application.OpenURL(section.url);
                    }
                }

                GUILayout.Space(kSpace);
            }
        }


        bool m_Initialized;

        GUIStyle LinkStyle
        {
            get { return m_LinkStyle; }
        }

        [SerializeField]
        GUIStyle m_LinkStyle;

        GUIStyle TitleStyle
        {
            get { return m_TitleStyle; }
        }

        [SerializeField]
        GUIStyle m_TitleStyle;

        GUIStyle HeadingStyle
        {
            get { return m_HeadingStyle; }
        }

        [SerializeField]
        GUIStyle m_HeadingStyle;

        GUIStyle BodyStyle
        {
            get { return m_BodyStyle; }
        }

        [SerializeField]
        GUIStyle m_BodyStyle;

        void Init()
        {
            if (m_Initialized)
                return;
            m_BodyStyle = new GUIStyle(EditorStyles.label);
            m_BodyStyle.wordWrap = true;
            m_BodyStyle.fontSize = 14;

            m_TitleStyle = new GUIStyle(m_BodyStyle);
            m_TitleStyle.fontSize = 26;

            m_HeadingStyle = new GUIStyle(m_BodyStyle);
            m_HeadingStyle.fontSize = 18;

            m_LinkStyle = new GUIStyle(m_BodyStyle);
            m_LinkStyle.wordWrap = false;
            // Match selection color which works nicely for both light and dark skins
            m_LinkStyle.normal.textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xDA / 255f, 1f);
            m_LinkStyle.stretchWidth = false;

            m_Initialized = true;
        }

        bool LinkLabel(GUIContent label, params GUILayoutOption[] options)
        {
            var position = GUILayoutUtility.GetRect(label, LinkStyle, options);

            Handles.BeginGUI();
            Handles.color = LinkStyle.normal.textColor;
            Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
            Handles.color = Color.white;
            Handles.EndGUI();

            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);

            return GUI.Button(position, label, LinkStyle);
        }

    }
}

