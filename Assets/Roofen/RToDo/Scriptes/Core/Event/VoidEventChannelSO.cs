#region

using UnityEngine;
using UnityEngine.Events;

#endregion

namespace RGame
{
    /// <summary>
    ///     This class is used for Events that have no arguments (Example: Exit game event)
    /// </summary>
    [CreateAssetMenu(menuName = "RGame/RToDo/Events/Void Event Channel")]
    public class VoidEventChannelSO : ScriptableObject
    {
        public UnityAction OnEventRaised;

        public void RaiseEvent()
        {
            if (OnEventRaised != null)
                OnEventRaised.Invoke();
        }

        public void AddListener(UnityAction _listener)
        {
            OnEventRaised += _listener;
        }

        public void RemoveListener(UnityAction _listener)
        {
            OnEventRaised -= _listener;
        }
    }
}