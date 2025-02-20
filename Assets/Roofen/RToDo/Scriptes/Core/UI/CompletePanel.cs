#region

using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace RGame.RToDo
{
    /// <summary>
    ///     UI panel for displaying and managing completed tasks
    /// </summary>
    public class CompletePanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup mCanvasGroup;
        [SerializeField] private TaskConfigSO mTaskConfig;
        [SerializeField] private TaskItemUI mTaskItemPrefab;
        [SerializeField] private Transform mTaskContainer;
        [SerializeField] private TaskListUI mTaskList;
        public UnityAction OnReturnRequested;

        private void Start()
        {
            InitializeTasks();
        }

        /// <summary>
        ///     Subscribes to task completion events
        /// </summary>
        private void OnEnable()
        {
            mTaskConfig.OnTaskCompleted += OnTaskComplete;
        }

        /// <summary>
        ///     Unsubscribes from task completion events
        /// </summary>
        private void OnDisable()
        {
            mTaskConfig.OnTaskCompleted -= OnTaskComplete;
        }

        /// <summary>
        ///     Fades in the panel and enables interaction
        /// </summary>
        public void Show()
        {
            mCanvasGroup.DOFade(1, 0.3f).OnComplete(() =>
            {
                mCanvasGroup.interactable = true;
                mCanvasGroup.blocksRaycasts = true;
            });
        }

        /// <summary>
        ///     Fades out the panel and disables interaction
        /// </summary>
        public void Hide()
        {
            mCanvasGroup.DOFade(0, 0.3f);
            mCanvasGroup.interactable = false;
            mCanvasGroup.blocksRaycasts = false;
        }

        /// <summary>
        ///     Initializes completed tasks list on start
        /// </summary>
        private void InitializeTasks()
        {
            for (var i = 0; i < mTaskConfig.CompletedTasks.Count; i++) CreateTaskItem(mTaskConfig.CompletedTasks[i]);
        }

        /// <summary>
        ///     Handles new task completion by creating a task item
        /// </summary>
        private void OnTaskComplete(TaskData _task)
        {
            CreateTaskItem(_task);
        }

        /// <summary>
        ///     Instantiates and initializes a new task item UI
        /// </summary>
        private void CreateTaskItem(TaskData _task)
        {
            var taskItem = Instantiate(mTaskItemPrefab, mTaskContainer);
            taskItem.Initialize(_task.Description, _task.Deadline, () => CompleteTask(_task, taskItem));
            mTaskList.AddItem(taskItem);
        }

        /// <summary>
        ///     Marks task as incomplete and removes it from completed list
        /// </summary>
        private void CompleteTask(TaskData _task, TaskItemUI _taskItem)
        {
            mTaskConfig.SetTaskNotComplete(_task);
            _taskItem.PlayCompleteAnimation(() => { mTaskList.RemoveItem(_taskItem); });
        }

        /// <summary>
        ///     Handles return button click event
        /// </summary>
        public void OnReturnButtonClick()
        {
            OnReturnRequested.Invoke();
        }
    }
}