using System;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class AddressOrSoExample : MonoBehaviour
{
    public GameSceneSO targetScene;
    public VoidEventSO loadSceneEvent;

    private void OnEnable()
    {
        loadSceneEvent.OnRaiseEvent += LoadScene;
    }

    private void OnDisable()
    {
        loadSceneEvent.OnRaiseEvent -= LoadScene;
    }

    private void LoadScene()
    {
        targetScene.sceneReference.LoadSceneAsync(LoadSceneMode.Single,true);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(100, 100, 100, 50), "开启测试"))
        {
            loadSceneEvent.RaisedEvent();
        }
    }
}
