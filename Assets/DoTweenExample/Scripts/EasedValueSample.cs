using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EasedValueSample : MonoBehaviour
{
    private Vector3 _startPoint;
    public Vector3 targetPoint;
    public float flyTime;
    public Ease easeType;

    private bool _paused = false;
    [SerializeField] [Range(0, 1)] private float _progress;
    
    void Start()
    {
        _startPoint = transform.position;
    }

    void Update()
    {
        _progress = Mathf.Clamp01(_progress);
        float value = DOVirtual.EasedValue(0, 1, _progress, easeType);
        transform.position = _startPoint + (targetPoint - _startPoint) * value;
    }

    private IEnumerator PlayDoTween()
    {
        yield return null;
    }

    private IEnumerator ReverseTween()
    {
        float time = flyTime * _progress;
        while (time < flyTime)
        {
            yield return null;
        }
    }
    
}
