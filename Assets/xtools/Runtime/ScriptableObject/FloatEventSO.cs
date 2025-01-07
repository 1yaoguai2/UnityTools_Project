using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "FloatEventSO", menuName = "Event/FloatEvent")]
public class FloatEventSO : ScriptableObject
{
    public UnityAction<float> OnRaiseEvent;

    public void RaisedEvent(float t)
    {
        OnRaiseEvent?.Invoke(t);
    }
}
