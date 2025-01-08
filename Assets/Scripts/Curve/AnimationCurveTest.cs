using System.Collections;
using UnityEngine;

public class AnimationCurveTest : MonoBehaviour
{
    public AnimationCurve curve;
    private Vector3 startPosition;
    public Vector3 endPosition;
    public float totalTime;
    public float maxHeight;
    [Header("使用渐变属性")] public bool isUseRang;
    [Range(0, 1)] public float targetTime;


    private void Start()
    {
        startPosition = transform.position;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StopAllCoroutines();
            StartCoroutine(FlyWithCurve());
        }

        if (isUseRang)
        {
            Vector3 dir = (endPosition - startPosition).normalized;
            float distance = Vector3.Distance(endPosition, startPosition);
            float sampleRate = curve.Evaluate(targetTime);
            transform.position = startPosition + dir * (distance * targetTime) + (sampleRate * maxHeight) * Vector3.up;
        }
    }

    private IEnumerator FlyWithCurve()
    {
        float time = 0;
        Vector3 dir = (endPosition - startPosition).normalized;
        float distance = Vector3.Distance(endPosition, startPosition);
        while (time < totalTime)
        {
            float normalizedTime = time / totalTime;
            float sampleRate = curve.Evaluate(normalizedTime);
            transform.position = startPosition + dir * (distance * normalizedTime) +
                                 (sampleRate * maxHeight) * Vector3.up;
            time += Time.deltaTime;
            yield return null;
        }
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(100, 100, 100, 30), "按Q播放动画"))
        {
            StopAllCoroutines();
            StartCoroutine(FlyWithCurve());
        }
    }
}