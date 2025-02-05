# DoTween插件操作手册

1. 学习链接

​       [DOTween动画插件超详解（保姆级巨细）-CSDN博客](https://blog.csdn.net/xks18232047575/article/details/143219657)

2. 注意事项

   2.1 初始化

   2.2 清除遗留

   2.3 设置timeScale对动画的影响

3. 代码块

   3.1 移动旋转缩放

   ```
   DOMove(),DOMoveY()
   DORotate(),DOLoaclRotate(),DORotateQuaternion()
   DOScale()
   ```

   3.2 数字变化

   ```
    DOTween.To((value) =>
               {
                   numberTxt.text = value.ToString("N0");
               }, 0f, 100000f, 3f).SetEase(Ease.Flash);
   ```

   3.3 颜色变化

   ```
    DOTween.To((value) =>
               {
                   numberTxt.color = Color.Lerp(Color.blue, Color.red, value);
   
               }, 0, 1, 3f).SetEase(Ease.Flash);
               
     //numberTxt.DOColor(Color.red, 3f); //TextMeshPro需要DoTween高级版
   ```

   



