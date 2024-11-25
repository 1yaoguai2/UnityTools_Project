﻿using Cat.Animation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Animation;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
#pragma warning disable 0649
namespace Cat.Sample
{
    class Sample_Animation : MonoBehaviour
    {

        public Transform target;
        public Transform source;

        // Start is called before the first frame update
        void Start()
        {
            UALAnimation uAL = new UALAnimation(
                1,
                1,
                10,
                source.position,
                target.position,
                LineAxial.Y,
                1,
                1);

            source.PlayAnimation(uAL);
            uAL.OnPlayStart += UAL_OnPlayStart;
            uAL.OnPlayUpdate += UAL_OnPlayUpdate;
            uAL.OnPlayComplete += UAL_OnPlayComplete;
        }
        
        private void UAL_OnPlayStart(CatAnimation anim)
        {
            Debug.Log($"开始播，理论耗时{anim.ElapsedTime.TotalSeconds}");
        }

        private void UAL_OnPlayComplete(CatAnimation anim)
        {
            
            Debug.Log("播放完成,物理耗时" + anim.RealElapsedTime.TotalSeconds);
        }

        private void UAL_OnPlayUpdate(CatAnimation animation)
        {
            //Debug.Log("正在播");
        }


        public LineAxial LineAxial;
        public LinePlane LinePlane;
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadSceneAsync(1);
                //GameObject.Destroy(source.gameObject);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {

            }
        }
        [ContextMenu("ss")]
        public void Test()
        {
            //source.transform.position = Vector3.zero;

            UALAnimation anim = new UALAnimation(0.5f, 0.5f, 2.67f, source.position, target.position, LineAxial.Y, source.rotation);
            //LineAnimation anim = new LineAnimation(source.position, target.position, 2, LinePlane, source.rotation, 1f);
            source.PlayAnimation(anim);
            anim.OnPlayStart += UAL_OnPlayStart;
            anim.OnPlayUpdate += UAL_OnPlayUpdate;
            anim.OnPlayComplete += UAL_OnPlayComplete;

            //UALAnimation uAL = new UALAnimation(1, 1, 10, source.position, target.position, LineAxial, source.rotation);

            //source.PlayAnimation(uAL);
            //uAL.OnPlayStart += UAL_OnPlayStart;
            //uAL.OnPlayUpdate += UAL_OnPlayUpdate;
            //uAL.OnPlayComplete += UAL_OnPlayComplete;
        }
    }
}

