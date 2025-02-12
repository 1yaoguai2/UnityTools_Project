using System.Collections;
using UnityEngine;
using XCharts.Runtime;

public class LineChartController : MonoBehaviour
{
    private LineChart _lineChart;

    private void Awake()
    {
        _lineChart = GetComponent<LineChart>();
        if(_lineChart is not null)
            _lineChart.Init();
    }

    private void Start()
    {
        StartCoroutine(UpdateLineChartData());
    }

    private IEnumerator UpdateLineChartData()
    {
        var x = 0f;
        while (true)
        {
            for (int i = 0; i < 6; i++)
            {
                x = Random.Range(0, 100);
                _lineChart.UpdateData(0, i, x);
                yield return new WaitForSeconds(1f);
            }
            yield return new WaitForSeconds(3f);
        }
    }
}
