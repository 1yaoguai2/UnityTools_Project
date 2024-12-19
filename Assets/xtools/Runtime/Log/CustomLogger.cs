using UnityEngine;
using System.Diagnostics;
using System;
using System.Text;
using System.IO;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

/// <summary>
/// 自定义日志工具类，提供日志记录和文件管理功能
/// </summary>
public static class CustomLogger
{
    // 基础配置常量和字段
    private const int BUFFER_SIZE = 256;  // StringBuilder的初始和重置容量
    private static readonly StringBuilder StringBuilder = new StringBuilder(BUFFER_SIZE);
    private static string LogFilePath;    // 当前日志文件路径
    private static readonly object LogLock = new object();  // 线程同步锁
    private static bool IsInitialized;    // 日志系统初始化标志
    private static readonly string LogDirectory = Path.Combine(Application.streamingAssetsPath, "Logs");
    private static DateTime LastInitTime;  // 上次初始化时间
    private static readonly TimeSpan InitInterval = TimeSpan.FromHours(1);  // 日志文件更新间隔

    /// <summary>
    /// 重置StringBuilder的状态和容量
    /// </summary>
    private static void ResetStringBuilder()
    {
        StringBuilder.Clear();
        StringBuilder.Capacity = BUFFER_SIZE;
    }

    /// <summary>
    /// 初始化或更新日志系统
    /// </summary>
    /// <remarks>
    /// - 创建日志目录和文件
    /// - 每小时创建新的日志文件
    /// - 写入系统信息头
    /// </remarks>
    private static void InitializeLogger()
    {
        if (IsInitialized)
        {
            // 检查是否需要创建新的日志文件（每小时）
            var now = DateTime.Now;
            if (now - LastInitTime < InitInterval) return;

            LastInitTime = now;
        }

        try
        {
            // 确保日志目录存在
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }

            // 创建日志文件名（包含日期和小时）
            var now = DateTime.Now;
            string fileName = $"game_log_{now:yyyy-MM-dd_HH}h.txt";
            string newLogPath = Path.Combine(LogDirectory, fileName);

            // 如果是同一个文件，不需要重新初始化
            if (newLogPath.Equals(LogFilePath)) return;

            LogFilePath = newLogPath;

            // 使用StringBuilder构建header以减少字符串连接
            StringBuilder.Clear();
            StringBuilder
                .AppendLine()
                .Append("=== Log Start at ").Append(now.ToString("yyyy-MM-dd HH:mm:ss")).AppendLine(" ===")
                .Append("Device: ").AppendLine(SystemInfo.deviceModel)
                .Append("OS: ").AppendLine(SystemInfo.operatingSystem)
                .Append("Game Version: ").AppendLine(Application.version)
                .AppendLine("=====================================")
                .AppendLine();

            // 使用追加模式写入header
            using (var fileStream = new FileStream(LogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.Write(StringBuilder.ToString());
            }

            IsInitialized = true;
            LastInitTime = now;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize logger: {e.Message}");
        }
        finally
        {
            ResetStringBuilder();
        }
    }

    /// <summary>
    /// 将日志消息写入文件
    /// </summary>
    /// <param name="message">日志消息内容</param>
    /// <param name="logType">日志类型（Error/Warning/Log）</param>
    /// <remarks>
    /// 使用线程锁确保线程安全的文件写入
    /// </remarks>
    private static void WriteToFile(string message, LogType logType)
    {
        if (string.IsNullOrEmpty(LogFilePath)) return;

        try
        {
            lock (LogLock)
            {
                // 检查是否需要创建新的日志文件
                InitializeLogger();

                // 直接写入消息，因为消息已经包含了时间戳和日志类型
                using (var fileStream = new FileStream(LogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.WriteLine(message);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write to log file: {e.Message}");
        }
    }

    /// <summary>
    /// 记录带上下文的日志信息
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="context">Unity对象上下文，通常是MonoBehaviour实例</param>
    /// <param name="logType">日志类型</param>
    /// <remarks>
    /// 同时在Console窗口显示并写入日志文件
    /// </remarks>
    public static void LogWithContext(string message, Object context = null, LogType logType = LogType.Log)
    {
        // 确保日志文件初始化
        if (!IsInitialized)
        {
            InitializeLogger();
        }

        try
        {
            ResetStringBuilder();

            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string stackTraceInfo = string.Empty;

#if UNITY_EDITOR
            // 在编辑器模式下获取堆栈信息
            var stackTrace = new StackTrace(true);
            var frame = stackTrace.GetFrame(1);
            var fileName = frame.GetFileName();
            var lineNumber = frame.GetFileLineNumber();

            StringBuilder
                .Append('[').Append(timestamp).Append("] ")
                .Append(message).Append('\n')
                .Append("File: ").Append(fileName).Append('\n')
                .Append("Line: ").Append(lineNumber);

            string fullMessage = StringBuilder.ToString();

            // 在Console窗口显示
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

            // 为文件日志添加堆栈信息
            stackTraceInfo = $"\nFile: {fileName}\nLine: {lineNumber}";
#endif

            // 构建日志文件消息
            ResetStringBuilder();
            StringBuilder
                .Append('[').Append(timestamp).Append(']')
                .Append('[').Append(logType.ToString().ToUpper()).Append("] ")
                .Append(message)
                .Append(stackTraceInfo); // 添加堆栈信息（如果在编辑器模式下）

            WriteToFile(StringBuilder.ToString(), logType);
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
    /// 清理指定天数之前的日志文件
    /// </summary>
    /// <param name="daysToKeep">要保留的天数，默认7天</param>
    /// <remarks>
    /// 根据文件创建时间删除过期的日志文件
    /// </remarks>
    public static void CleanOldLogs(int daysToKeep = 7)
    {
        try
        {
            string logDirectory = Path.Combine(Application.persistentDataPath, "Logs");
            if (!Directory.Exists(logDirectory)) return;

            var directory = new DirectoryInfo(logDirectory);
            var files = directory.GetFiles("game_log_*.txt");
            var cutoffDate = DateTime.Now.AddDays(-daysToKeep);

            foreach (var file in files)
            {
                if (file.CreationTime < cutoffDate)
                {
                    file.Delete();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to clean old logs: {e.Message}");
        }
    }

    /// <summary>
    /// 获取当前日志文件的完整路径
    /// </summary>
    /// <returns>日志文件路径</returns>
    public static string GetCurrentLogPath()
    {
        return LogFilePath;
    }

    /// <summary>
    /// 记录错误级别的日志（仅在编辑器模式可用）
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="context">相关的Unity对象</param>
    public static void LogError(string message, Object context = null)
    {
        LogWithContext(message, context, LogType.Error);
    }

    /// <summary>
    /// 记录警告级别的日志（仅在编辑器模式可用）
    /// </summary>
    /// <param name="message">警告消息</param>
    /// <param name="context">相关的Unity对象</param>
    public static void LogWarning(string message, Object context = null)
    {
        LogWithContext(message, context, LogType.Warning);
    }

    /// <summary>
    /// 记录普通信息级别的日志（仅在编辑器模式可用）
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="context">相关的Unity对象</param>
    public static void Log(string message, Object context = null)
    {
        LogWithContext(message, context, LogType.Log);
    }
}