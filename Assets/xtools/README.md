# 开发自定义插件注意事项

Unity官方文档：[Unity - Manual: Project manifest](https://docs.unity3d.com/2022.2/Documentation/Manual/upm-manifestPrj.html)

开发流程1：[Unity 如何制作和发布你的 Package_unitypackage-CSDN博客](https://blog.csdn.net/Jaihk662/article/details/137688906)

开发流程2：[unity自定义插件开发流程，并通过GitUrl导入插件测试--unity学习笔记_unity 插件开发-CSDN博客](https://blog.csdn.net/qq_38399916/article/details/135699274)

发布UPM版本：[Unity教程-使用Package Manager开发和管理自定义插件 - 简书](https://www.jianshu.com/p/2a7a35454f3a)



# 更新插件步骤

1. 启动git-bash，转到目标地址

2. git status，检查文件变化
3. git add .
4. git commit -m "优化"
5. git push -f https://github.com/1yaoguai2/XTools.git upm
6. 合并到Main分支



# 插件程序集

未对插件的Editor和Test创建AssemblyDefinition，原因：理不清依赖关系

对Runtime创建了程序集，依赖

Unity.TextMeshPro
Unity.Addressables
Unity.ResourceManager

装配集合的应用：

Unity小技巧 如何使用AssemlyDefinition划分多个程序集 减少编译时间

[Unity小技巧 如何使用AssemlyDefinition划分多个程序集 减少编译时间_哔哩哔哩_bilibili](https://www.bilibili.com/video/BV1Ud4y1w7zC/?spm_id_from=333.1387.favlist.content.click&vd_source=a08df359422d16d82a30f019bf9ebb8c)



# 注意事项

1. 需要.meta文件，注意GUID冲突问题
