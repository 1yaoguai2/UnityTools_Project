using Cat.Animation;
using System.Collections;
using System.Collections.Generic;
using Animation;
using UnityEngine;

public class TestLineAnimation : MonoBehaviour
{
    public Transform Tr_A;
    public Transform Tr_B;

    // Start is called before the first frame update
    void Start()
    {
        var anim = new LineAnimation(
            Tr_A.transform.position,
            Tr_B.transform.position,
            speed: 1,
            proportion: 1,
            ratio: 10);
        Tr_A.PlayAnimation(anim);
        anim.OnPlayComplete += Anim_OnPlayComplete;
    }

    private void Anim_OnPlayComplete(CatAnimation obj)
    {
        LineAnimation line = obj as LineAnimation;
        Debug.Log($"ElapsedTime:{line.ElapsedTime}");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
