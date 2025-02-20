#region

using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

#endregion

namespace RGame.RToDo
{
    /// <summary>
    ///     Handles persistence operations for task data using JSON format
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        private const string SAVE_FILENAME = "tasks.json";
        [SerializeField] private TaskConfigSO mTaskConfig;
        private string mSaveFilePath;

        /// <summary>
        ///     Initializes save system and loads existing data
        /// </summary>
        private void Awake()
        {
            InitializeSavePath();
            LoadData();
        }

        /// <summary>
        ///     Creates necessary directories and initial save file if missing
        /// </summary>
        private void InitializeSavePath()
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
                Directory.CreateDirectory(Application.streamingAssetsPath);

            mSaveFilePath = Path.Combine(Application.streamingAssetsPath, SAVE_FILENAME);

            if (!File.Exists(mSaveFilePath))
            {
                var emptyJson = JsonUtility.ToJson(new ToDoJsonData(), true);
                File.WriteAllText(mSaveFilePath, emptyJson);
            }
        }

        /// <summary>
        ///     Loads task data with platform-specific loading method
        /// </summary>
        private void LoadData()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            StartCoroutine(LoadDataFromStreamingAssets());
#else
            var jsonData = File.ReadAllText(mSaveFilePath);
            mTaskConfig.LoadFromJson(jsonData);
#endif
        }

        /// <summary>
        ///     Handles Android-specific file loading through UnityWebRequest
        /// </summary>
        private IEnumerator LoadDataFromStreamingAssets()
        {
            var filePath = "file://" + mSaveFilePath;
            using var request = UnityWebRequest.Get(filePath);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var jsonData = request.downloadHandler.text;
                mTaskConfig.LoadFromJson(jsonData);
            }
            else
            {
                Debug.LogError($"Failed to load tasks: {request.error}");
            }
        }

        /// <summary>
        ///     Serializes current tasks to JSON and writes to file
        /// </summary>
        public void SaveData()
        {
            try
            {
                var jsonData = mTaskConfig.SaveToJson();
                File.WriteAllText(mSaveFilePath, jsonData);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save tasks: {e.Message}");
            }
        }
    }
}