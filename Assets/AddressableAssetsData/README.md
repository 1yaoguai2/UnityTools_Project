# Unity 关于Addressables相关操作

[【游戏开发探究】Unity Addressables资源管理方式用起来太爽了，资源打包、加载、热更变得如此轻松（Addressable Asset System | 简称AA）_unity aa-CSDN博客](https://blog.csdn.net/linxinfa/article/details/122390621)

## Addressables相关操作

XTools依赖于Addressables插件，所有资源都需要先打包，然后才能打包项目。

1. 打开资源管理窗口

   Window->Assets Management->Addressables->Groups

2. 新增资源

   在Addressables Groups窗口下，左侧按钮New -> Packed Assets, 出现新的列

   将场景或者预制体拖拽到Packed Assets 下，右键，Simpify Addressable Names，自动修改资产地址为名称

3. 手动打包

   左上角，Play Mode Script，选择Use Existing Build

   左上角，Build->New Build->Default Build Script

   注意：每次修改资产后，都需要操作一次

4. 自动打包

   文件夹Assets->AdressableAssetsData->AddressableAssetSettings文件

   属性界面，Build->Bild Addressables on player Build ->选择content on Player Build

5. 标签功能

   资源分为包内资源（静态资源），包外资源（动态资源），都可以添加多个Label，可以统一加载某种Label，不建议多个Lable

6. Addressables使用Event Viewer窗口，查看资源释放

   在AddressableAssetsSettings配置文件中，General->Send Profiler Events(勾选)

7. 资源颗粒化

   Assets->AddressableAsstesData->AssetGroups->Schemas->EwmoteGroup配置文件下

   BundelMode->Pack Separately.



## Addressables 资源加载

```c#
//通过地址加载资源
Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Cube.prefab").Completed += (handle) =>
        {
            // 预设物体
            GameObject prefabObj = handle.Result;
            // 实例化
            GameObject cubeObj = Instantiate(prefabObj);
        };

```

```c#
//一步到位
Addressables.InstantiateAsync("Assets/Prefabs/Cube.prefab").Completed += (handle) =>
{
    // 已实例化的物体
    GameObject cubeObj = handle.Result;
};

```

```C#
// Asset弱引用,
public AssetReference spherePrefabRef;

    void Start()
    {
        spherePrefabRef.LoadAssetAsync<GameObject>().Completed += (obj) =>
        {
            // 预设
            GameObject spherePrefab = obj.Result;
            // 实例化
            GameObject sphereObj = Instantiate(spherePrefab);
        };
        
        //一步
        spherePrefabRef.InstantiateAsync().Completed += (obj) =>
        {
          // 已实例化的物体
          GameObject sphereObj = obj.Result;
        };
    }


```

```C#
//通过Label加载所有资源 
public AssetLabelReference textureLabel;

    void Start()
    {
        Addressables.LoadAssetsAsync<Texture2D>(textureLabel, (texture) =>
        {
            // 没加载完一个资源，就回调一次
            Debug.Log("加载了一个资源： " + texture.name);
        });
        
        //一口气加载完,再回调
               var handle1 = Addressables.LoadAssetAsync<List<GameUISO>>(assetsLabel);
          handle1.Completed += (obj) =>
        {
            List<GameUISO> gameUisos = obj.Result;
        };
    }
```

```c#
 // 释放资源,可以通过Addressable Event Viewer查看释放结果
   Addressables.Release(handle);
```

```C#
//使用协程加载资源,最后释放  
private IEnumerator InitUI()
    {
        Dictionary<string, GameUISO> uISO = new Dictionary<string, GameUISO>();
        //该场景下所有UI资产
        var handle = Addressables.LoadAssetAsync<GameUISO>(assetsLabel);
        handle.Completed += (gameUISo) =>
        {
            Debug.Log("GameUISO名称 " + gameUISo.Result.name);
            uISO.Add(gameUISo.Result.name, gameUISo.Result);
        };
        while (!handle.IsDone) yield return null;
        UIManager.Instance.InitDics(uISO);
        InitOpenUI();
        Addressables.Release(handle);
    }
```

```c#
   //卡住主线程加载资源
   var handle = panelSo.uiReference.LoadAssetAsync<GameObject>();
   handle.WaitForCompletion(); //卡住主线程
   currentPanelObj = handle.Result;
   prefabDic.Add(panelName, currentPanelObj);
   Addressables.Release(handle);  //释放
```

```C#
//不使用回调,使用异步,注WebGL不支持
private async void InstantiateCube()
	{
		// 虽然这里使用了Task，但并没有使用多线程
		GameObject prefabObj = await Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Cube.prefab").Task;
		// 实例化
		GameObject cubeObj = Instantiate(prefabObj);
		
		// 也可直接使用InstantiateAsync方法
		// GameObject cubeObj = await Addressables.InstantiateAsync("Assets/Prefabs/Cube.prefab").Task;
	}
```



