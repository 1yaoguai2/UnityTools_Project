using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class EasedValueSample : MonoBehaviour
{
    private Vector3 _startPoint;
    public Vector3 targetPoint;
    public float flyTime;
    public Ease easeType;

    [SerializeField]
    [Range(0,1)] private float _progress;
    
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

    private void OnGUI()
    {
        if (GUI.Button(new Rect(100, 0, 100, 30), "播放"))
        {
            StopTween();
            StartCoroutine(PlayTween());
        }
        if (GUI.Button(new Rect(100, 50, 100, 30), "暂停"))
        {
            StopTween();
        }
        if (GUI.Button(new Rect(100, 100, 100, 30), "反转"))
        {
            StopTween();
            StartCoroutine(ReverseTween());
        }
    }

    private IEnumerator PlayTween()
    {
        while (_progress < 1)
        {
            _progress += flyTime * Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator ReverseTween()
    {
        while (_progress > 0)
        {
            _progress -= flyTime * Time.deltaTime;
            yield return null;
        }
    }

    private void StopTween()
    {
        StopAllCoroutines();
    }
    
}
