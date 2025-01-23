using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;
public class LogManager
{
    private const int LOG_COUNT = 20; // 最多临时保存LOG_COUNT条日志
    private const int LOG_FILE_COUNT = 10; // 最多保存的日志文件数


    public static bool EnableLog = true; // 是否启用日志，仅可控制普通级别的日志的启用与关闭，LogError和LogWarn都是始终启用的。
    public static bool EnableSave = true; // 是否允许保存日志，即把日志写入到文件中

    public static string LogFileDir = Application.streamingAssetsPath + "/Log"; // 日志存放目录：streamingAssetsPath目录下的Log
    public static string LogFileName = "";
    public static string Prefix = "> "; // 用于与Unity默认的系统日志做区分。本日志系统输出的日志头部都会带上这个标记。
    public static StreamWriter LogFileWriter = null;

    private const string ErrorColor = "<color=Red>{0}</color>";
    private const string WarningColor = "<color=Yellow>{0}</color>";

    //日志列表，忽略Info时报错回溯使用
    public static List<string> ListLogs = new List<string>();

    //第一次执行打印log
    private static bool FirstLogTag = true;

    public static StringBuilder logStr = new StringBuilder();
    public static StringBuilder warningStr = new StringBuilder();
    public static StringBuilder errorStr = new StringBuilder();
    public static void Init()
    {
        Application.logMessageReceived += OnLogByUnity;
        logStr.Append("[I]");
        warningStr.Append("[W]");
        errorStr.Append("[E]");
    }

    #region 日志

    public static void Log(object message, bool recordStackTrace = false)
    {
        var (fileName, lineNumber) = GetStackTraceInfo();
        //string logStr = "[I]" + GetLogTime() + message;
        logStr.Append(GetLogTime());
        logStr.Append(message);
        AddListLogs(logStr.ToString());
        if (!EnableLog)
            return;
#if UNITY_EDITOR
        Debug.Log(Prefix + logStr, null);
#endif
        LogToFile(logStr.ToString(), recordStackTrace);
        logStr.Length = 3;
    }

    public static void LogWarning(object message)
    {
        var(fileName, lineNumber) = GetStackTraceInfo();
        //string warningStr = "[W]" + GetLogTime() + message;
        warningStr.Append(GetLogTime());
        warningStr.Append(message);
        AddListLogs(warningStr.ToString());
#if UNITY_EDITOR
        Debug.LogWarning(string.Format(WarningColor, Prefix + warningStr), null);
#endif
        LogToFile(warningStr.ToString(), true);
        warningStr.Length = 3;
    }

    public static void LogError(object message)
    {
        var (fileName, lineNumber) = GetStackTraceInfo();
        //string errorStr = "[E]" + GetLogTime() + message;
        errorStr.Append(GetLogTime());
        errorStr.Append(message);
#if UNITY_EDITOR
        Debug.LogError(string.Format(ErrorColor, Prefix + errorStr), null);
#endif
        if (!EnableLog)
        {
            OutputListLogs(LogFileWriter); // 忽略Info时报错，自动将日志记录到文件中方便回溯
        }
        else
        {
            AddListLogs(errorStr.ToString());
        }

        LogToFile(errorStr.ToString(), true);
        errorStr.Length = 3;
    }

    /// <summary>
    /// 输出列表中所有日志
    /// 可能会造成卡顿，谨慎使用。
    /// </summary>
    /// <param name="sw"></param>
    public static void OutputListLogs(StreamWriter sw)
    {
        if (sw == null || ListLogs.Count < 1)
            return;
        sw.WriteLine($"---------------- Log History Start [以下是报错前{ListLogs.Count}条日志]---------------- ");
        foreach (var i in ListLogs)
        {
            sw.WriteLine(i);
        }

        sw.WriteLine($"---------------- Log History  End  [以上是报错前{ListLogs.Count}条日志]---------------- ");
        ListLogs.Clear();
    }

    #endregion

    public static void CloseLog()
    {
        if (LogFileWriter != null)
        {
            try
            {
                LogFileWriter.Flush();
                LogFileWriter.Close();
                LogFileWriter.Dispose();
                LogFileWriter = null;
            }
            catch (Exception)
            {
            }
        }
    }

    public static void CheckClearLog()
    {
        if (!Directory.Exists(LogFileDir))
        {
            return;
        }

        DirectoryInfo direction = new DirectoryInfo(LogFileDir);
        var files = direction.GetFiles("*");
        if (files.Length >= LOG_FILE_COUNT)
        {
            var oldfile = files[0];
            var lastestTime = files[0].CreationTime;
            foreach (var file in files)
            {
                if (lastestTime > file.CreationTime)
                {
                    oldfile = file;
                    lastestTime = file.CreationTime;
                }
            }

            oldfile.Delete();
        }
    }

    private static void OnLogByUnity(string condition, string stackTrace, LogType type)
    {
        // 过滤自己的输出
        if (type == LogType.Log || condition.StartsWith(Prefix))
        {
            return;
        }

        var str = type == LogType.Warning ? "[W]" : "[E]" + GetLogTime() + condition + "\n" + stackTrace;
        if (!EnableLog && type != LogType.Warning)
            OutputListLogs(LogFileWriter); // 忽略Info时报错，自动将日志记录到文件中方便回溯
        else
            AddListLogs(str);
        //LogToFile(str);
    }

    private static void AddListLogs(string str)
    {
        if (ListLogs.Count > LOG_COUNT)
        {
            ListLogs.RemoveAt(0);
        }

        ListLogs.Add(str);
    }

    private static string GetLogTime()
    {
        return $"{DateTime.Now:HH:mm:ss.fff} ";
    }

    /// <summary>
    /// 将日志写入到文件中
    /// </summary>
    /// <param name="message"></param>
    /// <param name="EnableStack"></param>
    private static void LogToFile(string message, bool EnableStack = false)
    {
        if (!EnableSave)
            return;

        if (LogFileWriter == null)
        {
            CheckClearLog();
            LogFileName = DateTime.Now.GetDateTimeFormats('s')[0].ToString();
            LogFileName = LogFileName.Replace("-", "_");
            LogFileName = LogFileName.Replace(":", "_");
            LogFileName = LogFileName.Replace(" ", "");
            LogFileName = LogFileName.Replace("T", "_");
            LogFileName = LogFileName + ".log";
            if (string.IsNullOrEmpty(LogFileDir))
            {
                try
                {
                    if (!Directory.Exists(LogFileDir))
                    {
                        Directory.CreateDirectory(LogFileDir);
                    }
                }
                catch (Exception exception)
                {
#if UNITY_EDITOR
                    Debug.Log(Prefix + "获取 Application.streamingAssetsPath 报错！" + exception.Message, null);
#endif
                    return;
                }
            }

            string path = LogFileDir + "/" + LogFileName;
#if UNITY_EDITOR
            Debug.Log("Log Path :" + LogFileDir + "\nLog Name :" + LogFileName);
#endif
            try
            {
                if (!Directory.Exists(LogFileDir))
                {
                    Directory.CreateDirectory(LogFileDir);
                }

                LogFileWriter = File.AppendText(path);
                LogFileWriter.AutoFlush = true;
            }
            catch (Exception exception2)
            {
                LogFileWriter = null;
#if UNITY_EDITOR
                Debug.Log("LogToCache() " + exception2.Message + exception2.StackTrace, null);
#endif
                return;
            }
        }

        if (LogFileWriter != null)
        {
            try
            {
                if (FirstLogTag)
                {
                    FirstLogTag = false;
                    PhoneSystemInfo(LogFileWriter);
                }

                LogFileWriter.WriteLine(message);
                if (EnableStack)
                {
                    //把无关的log去掉
                    var st = StackTraceUtility.ExtractStackTrace();
#if UNITY_EDITOR
                    for (int i = 0; i < 3; i++)
#else
                        for (int i = 0; i < 2; i++)
#endif
                    {
                        st = st.Remove(0, st.IndexOf('\n') + 1);
                    }

                    LogFileWriter.WriteLine(st);
                }
            }
            catch (Exception)
            {
            }
        }
    }

    private static void PhoneSystemInfo(StreamWriter sw)
    {
        sw.WriteLine(
            "*********************************************************************************************************start");
        sw.WriteLine("By " + SystemInfo.deviceName);
        DateTime now = DateTime.Now;
        sw.WriteLine(string.Concat(new object[]
        {
            now.Year.ToString(), "年", now.Month.ToString(), "月", now.Day, "日  ", now.Hour.ToString(), ":",
            now.Minute.ToString(), ":", now.Second.ToString()
        }));
        sw.WriteLine();
        sw.WriteLine("操作系统:  " + SystemInfo.operatingSystem);
        sw.WriteLine("系统内存大小:  " + SystemInfo.systemMemorySize);
        sw.WriteLine("设备模型:  " + SystemInfo.deviceModel);
        sw.WriteLine("设备唯一标识符:  " + SystemInfo.deviceUniqueIdentifier);
        sw.WriteLine("处理器数量:  " + SystemInfo.processorCount);
        sw.WriteLine("处理器类型:  " + SystemInfo.processorType);
        sw.WriteLine("显卡标识符:  " + SystemInfo.graphicsDeviceID);
        sw.WriteLine("显卡名称:  " + SystemInfo.graphicsDeviceName);
        sw.WriteLine("显卡标识符:  " + SystemInfo.graphicsDeviceVendorID);
        sw.WriteLine("显卡厂商:  " + SystemInfo.graphicsDeviceVendor);
        sw.WriteLine("显卡版本:  " + SystemInfo.graphicsDeviceVersion);
        sw.WriteLine("显存大小:  " + SystemInfo.graphicsMemorySize);
        sw.WriteLine("显卡着色器级别:  " + SystemInfo.graphicsShaderLevel);
        sw.WriteLine("是否图像效果:  " + SystemInfo.supportsImageEffects);
        sw.WriteLine("是否支持内置阴影:  " + SystemInfo.supportsShadows);
        sw.WriteLine(
            "*********************************************************************************************************end");
        sw.WriteLine("LogInfo:");
        sw.WriteLine();
    }

    /// <summary>
    /// 获取当前调用的文件名和行号
    /// </summary>
    private static (string fileName, int lineNumber) GetStackTraceInfo()
    {
        try
        {
            var stackTrace = new StackTrace(true);
            // 遍历堆栈帧查找第一个非日志类的调用
            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                var frame = stackTrace.GetFrame(i);
                if (frame == null) continue;

                var method = frame.GetMethod();
                if (method == null || method.DeclaringType == null) continue;

                // 跳过日志类自身的方法
                if (method.DeclaringType == typeof(CustomLogger)) continue;

                return (frame.GetFileName() ?? "Unknown", frame.GetFileLineNumber());
            }
        }
        catch
        {
            // 如果获取堆栈信息失败，返回默认值
        }

        return ("Unknown", 0);
    }
}