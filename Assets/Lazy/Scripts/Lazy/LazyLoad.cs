using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazyLoad : MonoBehaviour
{
    /// <summary>
    /// 子物体两个游戏对象在Start中使用AddComponent新增各自形状的BoxCollider组件，并且获取对方的BoxCollider
    /// 就会出现错误，因为必定有一方先执行，新增然后获取对方的组件，然而对方的Start还未执行，未增加组件
    /// </summary>
    BoxCollider _collider;
    /// <summary>
    /// 使用属性的Get达成资源的lazyLoad
    /// 优点：使用才加载，且只加载一次，不使用则永不加载
    /// 去除资源对unity生命周期的依赖
    /// </summary>
    public BoxCollider Collider
    {
        get
        {
            if(_collider == null)
            {
                _collider = GetComponent<BoxCollider>();
            }
            return _collider;
        }
    }

    void Start()
    {
        CustomLogger.Log(Collider.size.ToString());
    }

    void Update()
    {
        
    }
}
