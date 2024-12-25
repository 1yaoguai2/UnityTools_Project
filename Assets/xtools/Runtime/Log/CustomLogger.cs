using UnityEngine;
using System.Diagnostics;
using System;
using System.Text;
using System.IO;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// 自定义日志工具类，提供日志记录和文件管理功能
/// </summary>
public static class CustomLogger
{
    private const int BUFFER_SIZE = 256;  // StringBuilder的初始和重置容量
    private const int INITIAL_QUEUE_CAPACITY = 1000;  // 队列初始容量
    private const int MAX_QUEUE_SIZE = 10000;  // 队列最大容量，防止内存溢出

    private static readonly StringBuilder s_StringBuilder = new StringBuilder(BUFFER_SIZE);
    private static string LogFilePath;    // 当前日志文件路径
    private static readonly object LogLock = new object();  // 线程同步锁
    private static readonly string LogDirectory = Path.Combine(Application.streamingAssetsPath, "Logs");
    private static Queue<string> LogQueue = new Queue<string>(INITIAL_QUEUE_CAPACITY);
    private static string timestamp;
    private static string logMessage;

    //电脑参数
    private static string deviceModel;
    private static string operatingSystem;
    private static string version;

    //日志堆栈信息
    private static string fileName;
    private static string lineNumber;
    private static LogType logType;


    /// <summary>
    /// 重置StringBuilder的状态和容量为初始值
    /// </summary>
    private static void ResetStringBuilder()
    {
        s_StringBuilder.Clear();
        s_StringBuilder.Capacity = BUFFER_SIZE;
    }

    /// <summary>
    /// 在应用启动时初始化系统信息并注册退出事件
    /// </summary>
    [RuntimeInitializeOnLoadMethod]
    private static void RegisterQuitHandler()
    {
        //Application.logMessageReceivedThreaded += OnLogMessageReceived;
        GetComputerInfo();
        Application.quitting += () =>
        {
            FlushLogsToFile(LogQueue);
            //Application.logMessageReceivedThreaded -= OnLogMessageReceived;
        };
    }

    /// <summary>
    /// 缓存当前设备和系统信息
    /// </summary>
    private static void GetComputerInfo()
    {
        deviceModel = SystemInfo.deviceModel;
        operatingSystem = SystemInfo.operatingSystem;
        version = Application.version;
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

    /// <summary>
    /// 收到日志消息
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="stacktrace"></param>
    /// <param name="type"></param>
    private static void OnLogMessageReceived(string condition, string stacktrace, LogType type)
    {
        //stacktrace如何使用堆栈字符串获取有用信息
        //fileName？
        //lineNumber？
        logType = type;
    }

    /// <summary>
    /// 记录日志信息
    /// </summary>
    /// <param name="message">日志内容</param>
    /// <param name="context">Unity对象上下文</param>
    /// <param name="logType">日志类型</param>
    public static void LogWithContext(string message, Object context = null, LogType logType = LogType.Log)
    {
        try
        {
            timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var (fileName, lineNumber) = GetStackTraceInfo();

#if UNITY_EDITOR

            s_StringBuilder
                .Append('[').Append(timestamp).Append("] ")
                .Append(message).Append('\n')
                .Append("File: ").Append(fileName).Append('\n')
                .Append("Line: ").Append(lineNumber);

            string fullMessage = s_StringBuilder.ToString();

            switch (logType)
            {
                case LogType.Error:
                    Debug.LogError(fullMessage, context);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(fullMessage, context);
                    break;
                default:
                    Debug.Log(fullMessage, context);
                    break;
            }
#else
            ResetStringBuilder();

            s_StringBuilder
                .Append('[').Append(timestamp).Append(']')
                .Append('[').Append(logType.ToString().ToUpper()).Append(']')
                .Append(message).Append('\n')
                .Append("File: ").Append(fileName).Append('\n')
                .Append("Line: ").Append(lineNumber);

#endif
            logMessage = s_StringBuilder.ToString();


            lock (LogLock)
            {
                // 检查队列大小
                if (LogQueue.Count >= MAX_QUEUE_SIZE)
                {
                    var oldQueue = LogQueue;
                    LogQueue = new Queue<string>(INITIAL_QUEUE_CAPACITY);

                    var warningMsg = $"[{DateTime.Now:HH:mm:ss.fff}][WARNING] Log queue exceeded limit, older logs were save and cleared";
                    oldQueue.Enqueue(warningMsg);

                    QueueLogsToSave(oldQueue);
                }

                LogQueue.Enqueue(logMessage);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Logger Error: {e.Message}\nOriginal message: {message}", context);
        }
        finally
        {
            ResetStringBuilder();
        }
    }

    /// <summary>
    /// 记录错误日志
    /// </summary>
    public static void LogError(string message, Object context = null)
    {
        LogWithContext(message, context, LogType.Error);
    }

    /// <summary>
    /// 记录警告日志
    /// </summary>
    public static void LogWarning(string message, Object context = null)
    {
        LogWithContext(message, context, LogType.Warning);
    }

    /// <summary>
    /// 记录普通日志
    /// </summary>
    public static void Log(string message, Object context = null)
    {
        LogWithContext(message, context, LogType.Log);
    }

    /// <summary>
    /// 将日志队列写入文件并清空队列
    /// </summary>
    private static void FlushLogsToFile(Queue<string> overflowQueue)
    {
        try
        {
            if (overflowQueue.Count > 0)
            {
                try
                {
                    var now = DateTime.Now;
                    var infoStringBuilder = new StringBuilder();
                    Directory.CreateDirectory(LogDirectory);
                    LogFilePath = Path.Combine(LogDirectory, $"log_{now:yyyy-MM-dd_HH}.txt");

                    infoStringBuilder
                        .AppendLine()
                        .Append("=== Log Start at ").Append(now.ToString("yyyy-MM-dd HH:mm:ss")).AppendLine(" ===")
                        .Append("Device: ").AppendLine(deviceModel)
                        .Append("OS: ").AppendLine(operatingSystem)
                        .Append("Game Version: ").AppendLine(version)
                        .AppendLine("=====================================")
                        .AppendLine();
                    using var fileStream = new FileStream(LogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                    using var streamWriter = new StreamWriter(fileStream);
                    streamWriter.Write(infoStringBuilder.ToString());

                    while (overflowQueue.Count > 0)
                    {
                        streamWriter.WriteLine(overflowQueue.Dequeue());
                    }
                    // 写入文件尾
                    streamWriter.WriteLine("\n=== Log End ===");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to flush logs: {e.Message}");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write overflow logs: {e.Message}");
        }
    }

    /// <summary>
    /// 使用线程池异步保存日志队列
    /// </summary>
    private static void QueueLogsToSave(Queue<string> oldQueue)
    {
        // 使用线程池而不是创建新线程
        ThreadPool.QueueUserWorkItem(_ =>
        {
            try
            {
                FlushLogsToFile(oldQueue);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save logs in thread pool: {e.Message}");
            }
        });
    }

}