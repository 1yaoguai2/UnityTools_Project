using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "VectorEventSO",menuName = "Event/VectorEvent")]
public class VectorEventSO : ScriptableObject
{
   public UnityAction<(float,float)> OnRaiseEvent;

   public void RaisedEvent((float,float) value)
   {
      OnRaiseEvent?.Invoke(value);
   }
}
