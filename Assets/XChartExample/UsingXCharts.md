### 使用XCharts

1. 网址

   [Hello from XCharts | XCharts](https://xcharts-team.github.io/)

   [XCharts——Unity上最好用的免费开源图表插件！（一）基本介绍-CSDN博客](https://blog.csdn.net/2201_75516689/article/details/131890984)

   [数据分析报表-数据分析报表模板下载-觅知网](https://www.51miz.com/so-biaoge/240933.html?utm_term=11088890&utm_source=baidu2&bd_vid=13302914347628773953)

2. Unity导入

```
https://github.com/XCharts-Team/XCharts.git#3.0
```

3. 创建图标

   层级窗口，右键创建UI->XCharts->*Chart

4. 图表设计

   1. 主题

      ```
      Transparent Background --透明背景，使用父物体图片作为背景板
      Enable Custom Theme  --启用颜色主题
      Custom Color Palette  --风格颜色调色板
      ```

   2. X 轴

      ```
      Type --横轴数据类型
      Split Number--横轴数量
      Data --横轴数据
      ```

   3. Serie 0：Line

      ```
      Line Type --线类型
      Symbol --顶点符号
      Line Style --线样式
      Data --数据          
      ```

3. 特殊操作

   1. 折线图顶点数据显示

      Series0：Line 功能新增，LabelStyle

   2. 折线图增加背景颜色

      Series0：Line 功能新增，AreaStyle

   3. 饼状图中心空缺

      Series0：Pie Radius 0.1 - 0.28

   4. 饼状图切换玫瑰图形

      Series0：Pie Rose Type -> Radius

   5. 柱状图多种类型的数据在一张表

      新增多个Serie，忽略Serie 1-N的不需要的数据，修改每个Serie的BarGap为-1，使数据居中

   6. 新增数据样式颜色标识

      AddMainComponent -> Legend

4. 相关代码

   ```
   //初始化
   var chart = gameObject.GetComponent<LineChart>();
   if (chart == null)
   {
       chart = gameObject.AddComponent<LineChart>();
       chart.Init();
   }
   ```

   

   ```
   //数据处理
   chart.ClearData()：清空图表数据（不移除Series）
   chart.RemoveData()：清除图表数据（会移除所有Serie）
   chart.AddSerie()：添加Serie
   chart.AddXAxisData()：添加X轴数据
   chart.AddData()：添加Serie数据
   chart.UpdateData()：更新Serie数据
   chart.UpdateXAxisData()：更新X轴数据
   chart.UpdateDataName()：更新Serie数据的名字
   ```

   

