#region

using System;
using UnityEngine;

#endregion

namespace RGame.RToDo
{
    /// <summary>
    ///     Manages task reminders and sound effects for todo operations
    /// </summary>
    public class ToDoManager : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private TaskConfigSO mTaskConfig;

        [SerializeField] private VoidEventChannelSO mSaveEventChannel;

        [Header("Audio")] [SerializeField] private AudioSource mAudioSource;
        [SerializeField] private AudioClip mCompleteSFX;
        [SerializeField] private AudioClip mHourlyReminder;
        private int mActiveTaskCount;

        private int mLastHour = -1;

        /// <summary>
        ///     Continuously checks for hourly reminders
        /// </summary>
        private void Update()
        {
            CheckHourlyReminder();
        }

        /// <summary>
        ///     Subscribes to task status change events
        /// </summary>
        private void OnEnable()
        {
            mTaskConfig.OnTaskAdded += AddNotCompleteCount;
            mTaskConfig.OnTaskCompleted += MidNotCompleteCount;
            mTaskConfig.OnTaskSetNotComplete += AddNotCompleteCount;
        }

        /// <summary>
        ///     Unsubscribes from task status change events
        /// </summary>
        private void OnDisable()
        {
            mTaskConfig.OnTaskAdded -= AddNotCompleteCount;
            mTaskConfig.OnTaskCompleted -= MidNotCompleteCount;
            mTaskConfig.OnTaskSetNotComplete -= AddNotCompleteCount;
        }

        /// <summary>
        ///     Plays hourly reminder sound when tasks exist at full hour
        /// </summary>
        private void CheckHourlyReminder()
        {
            if (mActiveTaskCount == 0) return;

            var now = DateTime.Now;
            if (now.Minute == 0 && now.Second == 0 && now.Hour != mLastHour)
            {
                mAudioSource.PlayOneShot(mHourlyReminder);
                mLastHour = now.Hour;
            }
        }

        /// <summary>
        ///     Handles active task count increase and plays sound
        /// </summary>
        private void AddNotCompleteCount(TaskData _x)
        {
            mActiveTaskCount++;
            mAudioSource.PlayOneShot(mCompleteSFX);
            mSaveEventChannel.OnEventRaised();
        }

        /// <summary>
        ///     Handles active task count decrease and plays sound
        /// </summary>
        private void MidNotCompleteCount(TaskData _x)
        {
            mActiveTaskCount--;
            mAudioSource.PlayOneShot(mCompleteSFX);
            mSaveEventChannel.OnEventRaised();
        }
    }
}