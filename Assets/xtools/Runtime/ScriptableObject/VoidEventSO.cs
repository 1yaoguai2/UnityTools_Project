using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "VoidEventSO", menuName = "Event/VoidEvent")]
public class VoidEventSO : ScriptableObject
{
    public UnityAction OnRaiseEvent;

    public void RaisedEvent()
    {
        OnRaiseEvent?.Invoke();
    }
}
