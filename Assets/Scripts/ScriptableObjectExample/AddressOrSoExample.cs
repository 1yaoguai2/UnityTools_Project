using System;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AddressOrSoExample : MonoBehaviour
{
    public GameSceneSO level1Scene;
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
        level1Scene.sceneReference.LoadSceneAsync(LoadSceneMode.Single,true);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(100, 100, 100, 50), "开启测试"))
        {
            loadSceneEvent.RaisedEvent();
        }
    }
}
