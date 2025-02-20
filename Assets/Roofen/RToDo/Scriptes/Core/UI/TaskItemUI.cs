#region

using System;
using System.Globalization;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
// 添加此命名空间

#endregion

namespace RGame.RToDo
{
    /// <summary>
    ///     Represents a single task item in the to-do list UI.
    /// </summary>
    public class TaskItemUI : MonoBehaviour
    {
        [SerializeField] private Image mIconImage;
        [SerializeField] private TextMeshProUGUI mDescriptionText;
        [SerializeField] private TextMeshProUGUI mDeadlineText;
        [SerializeField] private Button mCompleteButton;
        [SerializeField] private Sprite mCompleteSprite;

        private Action mOnComplete;
        private Sprite mOriginalSprite;
        private RectTransform mRectTransform;

        [HideInInspector] public DateTime MyDateTime;

        private void OnDestroy()
        {
            mCompleteButton.onClick.RemoveAllListeners();
            DOTween.Kill(mRectTransform);
            DOTween.Kill(mIconImage.transform);
        }

        /// <summary>
        ///     Initializes the task item with a description, deadline, and a callback for when the task is marked complete.
        /// </summary>
        public void Initialize(string _description, DateTime _deadline, Action _onComplete)
        {
            mRectTransform = GetComponent<RectTransform>();

            mDescriptionText.text = _description;
            UpdateDeadlineText(_deadline);
            mOnComplete = _onComplete;
            mCompleteButton.onClick.AddListener(() => mOnComplete?.Invoke());
        }

        /// <summary>
        ///     Updates the deadline text in the format "Tuesday, April 4".
        /// </summary>
        private void UpdateDeadlineText(DateTime _deadline)
        {
            // Specify English culture to ensure the date is formatted in English
            var weekDay = _deadline.ToString("dddd", CultureInfo.InvariantCulture);
            var month = _deadline.ToString("MMMM", CultureInfo.InvariantCulture);
            mDeadlineText.text = $"{weekDay}, {month} {_deadline.Day}";
        }

        /// <summary>
        ///     Plays a completion animation and invokes the provided Unity action once complete.
        /// </summary>
        public void PlayCompleteAnimation(UnityAction _completeAction)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(mIconImage.transform.DOScale(0f, 0.3f))
                .AppendCallback(() => mIconImage.sprite = mCompleteSprite)
                .Append(mIconImage.transform.DOScale(1f, 0.3f))
                .OnComplete(_completeAction.Invoke);
        }
    }
}