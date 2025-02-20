#region

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace RGame.RToDo
{
    /// <summary>
    ///     ScriptableObject that manages todo tasks storage and persistence
    /// </summary>
    [CreateAssetMenu(fileName = "TaskDatabase", menuName = "RGame/RToDo/TaskConfig")]
    public class TaskConfigSO : ScriptableObject
    {
        [SerializeField] private List<TaskData> mTasks = new();
        private readonly List<TaskData> mCompletedTasks = new();

        private readonly List<TaskData> mNotCompleteTasks = new();
        public UnityAction<TaskData> OnTaskAdded;

        public UnityAction<TaskData> OnTaskCompleted;
        public UnityAction<TaskData> OnTaskSetNotComplete;

        public IReadOnlyList<TaskData> NotCompleteTasks => mNotCompleteTasks;
        public IReadOnlyList<TaskData> CompletedTasks => mCompletedTasks;

        private void OnDisable()
        {
            mNotCompleteTasks.Clear();
            mCompletedTasks.Clear();
            mTasks.Clear();
        }

        private void Init()
        {
            for (var i = 0; i < mTasks.Count; i++)
                if (mTasks[i].IsCompleted)
                    mCompletedTasks.Add(mTasks[i]);
                else
                    mNotCompleteTasks.Add(mTasks[i]);
        }

        /// <summary>
        ///     Creates and adds a new task to the database
        /// </summary>
        public TaskData AddTask(string _description, DateTime _deadline)
        {
            var task = new TaskData
            {
                Description = _description,
                Deadline = _deadline,
                IsCompleted = false
            };

            mTasks.Add(task);
            mNotCompleteTasks.Add(task);
            var index = mTasks.Count - 1;
            OnTaskAdded?.Invoke(task);
            return task;
        }

        /// <summary>
        ///     Marks a task as completed and updates task lists
        /// </summary>
        public void SetTaskComplete(TaskData _task)
        {
            _task.IsCompleted = true;
            mNotCompleteTasks.Remove(_task);
            mCompletedTasks.Add(_task);
            OnTaskCompleted?.Invoke(_task);
        }

        /// <summary>
        ///     Reverts a task to not-completed status and updates task lists
        /// </summary>
        public void SetTaskNotComplete(TaskData _task)
        {
            _task.IsCompleted = false;
            mNotCompleteTasks.Add(_task);
            mCompletedTasks.Remove(_task);
            OnTaskSetNotComplete?.Invoke(_task);
        }

        /// <summary>
        ///     Initializes task database from JSON-formatted string
        /// </summary>
        public void LoadFromJson(string _jsonData)
        {
            var data = JsonUtility.FromJson<ToDoJsonData>(_jsonData);

            foreach (var task in data.mTasks)
                mTasks.Add(new TaskData
                {
                    Description = task.Description,
                    Deadline = DateTime.Parse(task.Deadline),
                    IsCompleted = task.IsCompleted
                });

            Init();
        }

        /// <summary>
        ///     Serializes current tasks into JSON format
        /// </summary>
        public string SaveToJson()
        {
            var tasks = new List<TaskJsonData>();

            foreach (var task in mTasks)
                tasks.Add(new TaskJsonData
                {
                    Description = task.Description,
                    Deadline = task.Deadline.ToString("o"),
                    IsCompleted = task.IsCompleted
                });

            var data = new ToDoJsonData { mTasks = tasks };
            return JsonUtility.ToJson(data, true);
        }
    }
}