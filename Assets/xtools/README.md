# 开发自定义插件注意事项

Unity官方文档：[Unity - Manual: Project manifest](https://docs.unity3d.com/2022.2/Documentation/Manual/upm-manifestPrj.html)

开发流程1：[Unity 如何制作和发布你的 Package_unitypackage-CSDN博客](https://blog.csdn.net/Jaihk662/article/details/137688906)

开发流程2：[unity自定义插件开发流程，并通过GitUrl导入插件测试--unity学习笔记_unity 插件开发-CSDN博客](https://blog.csdn.net/qq_38399916/article/details/135699274)

发布UPM版本：[Unity教程-使用Package Manager开发和管理自定义插件 - 简书](https://www.jianshu.com/p/2a7a35454f3a)





# 插件程序集

未对插件的Editor和Runtime创建AssemblyDefinition，原因：理不清依赖关系

装配集合的应用：

Unity小技巧 如何使用AssemlyDefinition划分多个程序集 减少编译时间

[Unity小技巧 如何使用AssemlyDefinition划分多个程序集 减少编译时间_哔哩哔哩_bilibili](https://www.bilibili.com/video/BV1Ud4y1w7zC/?spm_id_from=333.1387.favlist.content.click&vd_source=a08df359422d16d82a30f019bf9ebb8c)



# 使用插件后打包注意事项

XTools依赖于Addressables插件，所有资源都需要先打包，然后才能打包项目。

## Addressables相关操作

1. 打开资源管理窗口

   Window->Assets Management->Addressables->Groups

2. 新增资源

   在Addressables Groups窗口下，左侧按钮New -> Packed Assets, 出现新的列

   将场景或者预制体拖拽到Packed Assets 下，右键，Simpify Addressable Names，修改名称

3. 手动打包

   左上角，Play Mode Script，选择Use Existing Build

   左上角，Build->New Build->Default Build Script

   注意：每次修改资产后，都需要操作一次

4. 自动打包

   文件夹Assets->AdressableAssetsData->AddressableAssetSettings文件

   属性界面，Build->Bild Addressables on player Build ->选择content on Player Build



# Unity 关于Addressables相关操作

[【游戏开发探究】Unity Addressables资源管理方式用起来太爽了，资源打包、加载、热更变得如此轻松（Addressable Asset System | 简称AA）_unity aa-CSDN博客](https://blog.csdn.net/linxinfa/article/details/122390621)
