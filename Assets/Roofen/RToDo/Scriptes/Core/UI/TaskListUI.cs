#region

using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

#endregion

namespace RGame.RToDo
{
    /// <summary>
    ///     Manages the task list UI, including adding, removing, and rearranging task items.
    /// </summary>
    public class TaskListUI : MonoBehaviour
    {
        [SerializeField] private RectTransform mContent;
        [SerializeField] private float mItemSpacing = 10f;
        [SerializeField] private float mAnimationDuration = 0.5f;

        private readonly List<TaskItemUI> mItems = new();
        private readonly Queue<TaskItemUI> mPendingItems = new();
        private bool mIsAnimating;
        private float mOriginalX;

        /// <summary>
        ///     Cleans up resources when the object is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            mPendingItems.Clear();
            mItems.Clear();
            DOTween.KillAll();
        }

        /// <summary>
        ///     Adds a task item to the list and plays its entry animation.
        /// </summary>
        public void AddItem(TaskItemUI _item)
        {
            _item.transform.SetParent(mContent, false);
            var itemRect = _item.GetComponent<RectTransform>();
            var yPos = CalculateYPosition(mItems.Count, itemRect);
            if (mItems.Count == 0)
                mOriginalX = itemRect.anchoredPosition.x;
            itemRect.anchoredPosition = new Vector2(mOriginalX + itemRect.rect.width + 100,
                yPos - itemRect.rect.height / 2);
            mItems.Add(_item);
            mPendingItems.Enqueue(_item);
            UpdateContentHeight();
            if (!mIsAnimating)
                PlayNextEnterAnimation();
        }

        /// <summary>
        ///     Removes a task item from the list with an exit animation.
        /// </summary>
        public void RemoveItem(TaskItemUI _item)
        {
            _item.GetComponent<RectTransform>()
                .DOAnchorPosX(1500f, mAnimationDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    mItems.Remove(_item);
                    RearrangeItems();
                    UpdateContentHeight();
                    Destroy(_item.gameObject);
                });
        }

        /// <summary>
        ///     Calculates the Y position for a task item based on its index.
        /// </summary>
        private float CalculateYPosition(int _index, RectTransform _itemRect)
        {
            return -(_index * (_itemRect.rect.height + mItemSpacing) + 5);
        }

        /// <summary>
        ///     Plays the entry animation for the next task item in the queue.
        /// </summary>
        private void PlayNextEnterAnimation()
        {
            if (mPendingItems.Count == 0)
            {
                mIsAnimating = false;
                return;
            }

            mIsAnimating = true;
            var item = mPendingItems.Dequeue();
            var rect = item.GetComponent<RectTransform>();
            rect.DOAnchorPosX(mOriginalX, mAnimationDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(PlayNextEnterAnimation);
        }

        /// <summary>
        ///     Rearranges the task items in the list after a modification.
        /// </summary>
        private void RearrangeItems()
        {
            for (var i = 0; i < mItems.Count; i++)
            {
                var rect = mItems[i].GetComponent<RectTransform>();
                var targetY = CalculateYPosition(i, rect);
                rect.DOAnchorPosY(targetY - rect.rect.height / 2, 0.3f)
                    .SetEase(Ease.OutQuad);
            }
        }

        /// <summary>
        ///     Updates the height of the content container based on the number of task items.
        /// </summary>
        private void UpdateContentHeight()
        {
            if (mItems.Count == 0) return;
            var firstItemRect = mItems[0].GetComponent<RectTransform>();
            var totalHeight = mItems.Count * (firstItemRect.rect.height + mItemSpacing);
            mContent.sizeDelta = new Vector2(mContent.sizeDelta.x, totalHeight + 5);
        }
    }
}