using System.Collections.Generic;
using UnityEngine;

public static class FindAllChild 
{
    public static List<Transform> FindFirstLevelChilds(this Transform transForm,Transform obj)
    {
        List<Transform> firstLevelChilds = new List<Transform>();
        for (int i = 0; i < obj.childCount; i++)
        {
            firstLevelChilds.Add(obj.GetChild(i));
        }
        return firstLevelChilds;
    }

}
