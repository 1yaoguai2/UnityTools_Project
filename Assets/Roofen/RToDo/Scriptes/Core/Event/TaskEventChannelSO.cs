#region

using UnityEngine;
using UnityEngine.Events;

#endregion

namespace RGame.RToDo
{
    [CreateAssetMenu(menuName = "RGame/RToDo/Events/Task Event Channel")]
    public class TaskEventChannelSO : ScriptableObject
    {
        private UnityAction<TaskData> OnEventRaised;

        public void RaiseEvent(TaskData _task)
        {
            OnEventRaised?.Invoke(_task);
        }

        public void AddListener(UnityAction<TaskData> _listener)
        {
            OnEventRaised += _listener;
        }

        public void RemoveListener(UnityAction<TaskData> _listener)
        {
            OnEventRaised -= _listener;
        }
    }
}