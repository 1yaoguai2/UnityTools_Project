using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExampleUsePool : MonoBehaviour
{
    public List<GameObject> objects = new List<GameObject>();
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-5f, 5f),
                Random.Range(-3f, 3f),
                Random.Range(-5f, 5f)
            );

            Vector3 randomRotation = new Vector3(
                Random.Range(0,360f),
                Random.Range(0,360f),
                Random.Range(0,360f)
            );

            var randomQuaternion = Quaternion.Euler(randomRotation);

            var obj = ObjectPool.Instance.SpawnFromPool("Cube", randomPosition, randomQuaternion);
            objects.Add(obj);

        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            objects[0].SetActive(false);
            objects.RemoveAt(0);
        }
    }
}
