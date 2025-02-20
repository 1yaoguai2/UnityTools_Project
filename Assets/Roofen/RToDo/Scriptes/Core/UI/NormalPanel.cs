#region

using System;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace RGame.RToDo
{
    /// <summary>
    ///     Normal panel manages the main task interface where users can create, view and complete tasks
    ///     Handles the interaction between task data (TaskConfigSO) and visual representation (TaskItemUI)
    /// </summary>
    public class NormalPanel : MonoBehaviour
    {
        [SerializeField] private TaskConfigSO mTaskConfig;
        [SerializeField] private TaskItemUI mTaskItemPrefab;
        [SerializeField] private TaskInputUI mTaskInput;
        [SerializeField] private Transform mTaskContainer;
        [SerializeField] private TaskListUI mTaskList;
        public UnityAction OpenMainMenu;

        private void Start()
        {
            InitializeTasks();
        }

        private void OnEnable()
        {
            mTaskInput.OnTaskSubmitted += HandleNewTask;
            mTaskConfig.OnTaskSetNotComplete += OnSetNotComplete;
        }

        private void OnDisable()
        {
            mTaskInput.OnTaskSubmitted -= HandleNewTask;
            mTaskConfig.OnTaskSetNotComplete -= OnSetNotComplete;
        }

        /// <summary>
        ///     Initializes the panel by loading and displaying all incomplete tasks
        /// </summary>
        private void InitializeTasks()
        {
            Debug.Log(mTaskConfig.NotCompleteTasks.Count);

            for (var i = 0; i < mTaskConfig.NotCompleteTasks.Count; i++) CreateTaskItem(mTaskConfig.NotCompleteTasks[i]);
        }

        /// <summary>
        ///     Creates a new task with given description and deadline, then displays it
        /// </summary>
        private void HandleNewTask(string _description, DateTime _deadline)
        {
            var task = mTaskConfig.AddTask(_description, _deadline);
            CreateTaskItem(task);
        }

        /// <summary>
        ///     Handles the event when a completed task is marked as incomplete
        /// </summary>
        private void OnSetNotComplete(TaskData _task)
        {
            CreateTaskItem(_task);
        }

        /// <summary>
        ///     Creates and initializes a visual task item from task data
        /// </summary>
        private void CreateTaskItem(TaskData _task)
        {
            var taskItem = Instantiate(mTaskItemPrefab, mTaskContainer);
            taskItem.Initialize(_task.Description, _task.Deadline, () => CompleteTask(_task, taskItem));
            mTaskList.AddItem(taskItem);
        }

        /// <summary>
        ///     Marks a task as complete and handles its visual removal
        /// </summary>
        private void CompleteTask(TaskData _task, TaskItemUI _taskItem)
        {
            mTaskConfig.SetTaskComplete(_task);

            _taskItem.PlayCompleteAnimation(() => { mTaskList.RemoveItem(_taskItem); });
        }

        public void OnOpenMainMenuButton()
        {
            OpenMainMenu?.Invoke();
        }
    }
}