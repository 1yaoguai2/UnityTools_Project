#region

using UnityEngine;

#endregion

namespace RGame.RToDo
{
    /// <summary>
    ///     Central controller managing application lifecycle and UI interactions
    /// </summary>
    public class AppManager : MonoBehaviour
    {
        [Header("Configurations")] [SerializeField]
        private TaskConfigSO mTaskConfig;

        [Header("UI References")] [SerializeField]
        private MenuPanel mMenuPanel;

        [SerializeField] private NormalPanel mNormalPanel;
        [SerializeField] private CompletePanel mCompletePanel;

        [Header("Events")] [SerializeField] private VoidEventChannelSO mSaveEventChannel;
        [SerializeField] private TaskEventChannelSO mTaskCompletedChannel;

        [SerializeField] private SaveSystem mSaveSystem;

        [SerializeField] private IThemeConfig mThemeConfig;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        /// <summary>
        ///     Handles UI panel transitions and event subscriptions
        /// </summary>
        private void OnEnable()
        {
            mNormalPanel.OpenMainMenu += HandleOpenMainMenuRequest;
            mTaskCompletedChannel.AddListener(HandleTaskCompleted);
            mSaveEventChannel.AddListener(mSaveSystem.SaveData);
        }

        /// <summary>
        ///     Cleans up event subscriptions
        /// </summary>
        private void OnDisable()
        {
            mNormalPanel.OpenMainMenu -= HandleOpenMainMenuRequest;
            mTaskCompletedChannel.RemoveListener(HandleTaskCompleted);
            mSaveEventChannel.RemoveListener(mSaveSystem.SaveData);
        }

        /// <summary>
        ///     Ensures data persistence when application quits
        /// </summary>
        private void OnApplicationQuit()
        {
            mSaveSystem.SaveData();
        }

        /// <summary>
        ///     Handles main menu display logic
        /// </summary>
        private void HandleOpenMainMenuRequest()
        {
            mMenuPanel.OnReturnRequested += HandleReturnRequest;
            mMenuPanel.OnCompleteListRequested += HandleCompleteListRequest;

            mMenuPanel.Show();
        }

        /// <summary>
        ///     Handles main menu closure logic
        /// </summary>
        private void HandleReturnRequest()
        {
            mMenuPanel.OnReturnRequested -= HandleReturnRequest;
            mMenuPanel.OnCompleteListRequested -= HandleCompleteListRequest;

            mMenuPanel.Hide();
        }

        /// <summary>
        ///     Handles completed tasks list display
        /// </summary>
        private void HandleCompleteListRequest()
        {
            mCompletePanel.OnReturnRequested += HandleReturnMenuRequest;

            mCompletePanel.Show();
        }

        /// <summary>
        ///     Handles completed tasks list closure
        /// </summary>
        private void HandleReturnMenuRequest()
        {
            mCompletePanel.OnReturnRequested -= HandleReturnMenuRequest;

            mCompletePanel.Hide();
        }

        /// <summary>
        ///     Triggers save operation when task is completed
        /// </summary>
        private void HandleTaskCompleted(TaskData _task)
        {
            mSaveEventChannel.RaiseEvent();
        }
    }
}