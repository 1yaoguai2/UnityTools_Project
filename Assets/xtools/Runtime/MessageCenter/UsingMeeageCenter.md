## 1. 通道字符串定义

```c#
//静态类中

/// <summary>
/// 设备实时故障
/// </summary>
Public static string ErrorContent => "ErrorContent";
```

## 2. 发送消息

```c#
public static string ErrorContent => "ErrorContent";
public Transform errorEquip;

void OnEnable()
{
    MessageCenter.Send(this, ErrorContent,errorEquip);
}
```

## 3. 订阅消息消息

```c#
//start中订阅
MessageCenter.Subscribe<object, Transform>(this, ErrorContent, UpdateErrorUI);

 private void UpdateErrorUI(object sender, Transform newErrorEquip)
 {
        Debug.Log($"收到故障设备：{newErrorEquip.name}");
 }
```

