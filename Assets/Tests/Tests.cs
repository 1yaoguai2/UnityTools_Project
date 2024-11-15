using UnityEngine;
using XTools.EditorTools;

public class Tests : MonoBehaviour
{
    public string[] infoList;
    void Start()
    {
        Debug.Log(infoList.ToOneString());
    }

}
