using System.Collections.Generic;

/// <summary>
/// 示例类，用来初始化
/// key UI预制体名称
/// Value UI在Resources下的地址，例如Prefabs/UI/Menu
/// </summary>
public class UIConst
{
    public Dictionary<string, string> uIPrefabPathsDic = new Dictionary<string, string>();
    //Assets/Resources/Prefabs/UI文件夹下有下面几个UI预制体
    //预制体要求，自带Canvas，常用Canvas继承BaseScalePanel或者BasePellucidityPanel（清晰度）
    //不常用Canvas继承BasePanel，关闭的时候直接删除
    public const string MapCanvas = "MapCanvas";
    public const string MenuCanvas = "MenuCanvas";
    public const string MainCanvas = "MainCanvas";
    public const string TaskCanvas = "TaskCanvas";
    public const string ConfirmCanvas = "ConfirmCanvas";
    // public const string TimeCanvas = "TimeCanvas";
    public const string ConveyCanvas = "ConveyCanvas";
    public const string CraneCanvas = "CraneCanvas";
    public const string TimeCanvas = "TimeCanvas";

}