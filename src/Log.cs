#region using
using BepInEx.Logging;
using Kittehface.Build;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
// <DefineConstants>ENDERPEARL</DefineConstants>
#if ENDERPEARL
using EnderPearl;
#elif EXTENSIONLIB
using ExtensionLib;
#elif TRANSLATOR
using Translator;
#else
//
#endif

#endregion


internal class Log : CustomLogger
{
	public static Log Instance
	{
		get
		{
			instance ??= new Log();
			instance.Initialize();
			return instance;
		}
	}
	private static Log? instance;

	public override string ModName => Plugin.Name;
	public override bool EnableLog => Plugin.EnableLog;
	public override LogSeverity CurrentSeverity { get; } = LogSeverity.Development;
	public override bool isDevMod => false;

	public override string BaseDirectory => Path.Combine(UnityEngine.Application.persistentDataPath, ModName, "Logs");
	public override string OutputLogsDirectory => Path.Combine(this.BaseDirectory, "OutputLogs");
	public override string OutputLogFilePath => Path.Combine(this.OutputLogsDirectory, "output_log.txt");


	internal static void LogDevelopment(object Message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
	{
		Instance.BaseLogDevelopment(Message, memberName, filePath, lineNumber);
	}
	internal static void LogDebug(object Message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
	{
		Instance.BaseLogDebug(Message, memberName, filePath, lineNumber);
	}
	internal static void LogInfo(object Message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
	{
		Instance.BaseLogInfo(Message, memberName, filePath, lineNumber);
	}
	internal static void LogMessage(object Message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
	{
		Instance.BaseLogMessage(Message, memberName, filePath, lineNumber);
	}
	internal static void LogWarning(object Message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
	{
		Instance.BaseLogWarning(Message, memberName, filePath, lineNumber);
	}
	internal static void LogError(object Message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
	{
		Instance.BaseLogError(Message, memberName, filePath, lineNumber);
	}
	internal static void LogException(object Message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
	{
		Instance.BaseLogException(Message, memberName, filePath, lineNumber);
	}
	internal static void LogFatal(object Message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
	{
		Instance.BaseLogFatal(Message, memberName, filePath, lineNumber);
	}

}




internal static class _Log
{
	//public static ManualLogSource? log { get; private set; }
	private static ManualLogSource? log;

	public static bool EnableLog => Plugin.EnableLog;// || MyConfig.EnableLog;
	private static LogSeverity CurrentSeverity { get; set; } = LogSeverity.Debug;
	//private static LogLevel CurrentLevel { get; set; } = LogLevel.Debug;

	public static void SetLog(ManualLogSource logger) => log = logger;

	//public static void LogInfo(object obj) => Log_(LogLevel.Info, obj);// [CallerArgumentExpression]
	public static void LogInfo(object obj, bool stackTrace = false, [CallerMemberName] string caller = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) =>
		Log_(LogSeverity.Info, obj, stackTrace, caller, filePath, lineNumber);
	public static void LogWarning(object obj, bool stackTrace = false, [CallerMemberName] string caller = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) =>
		Log_(LogSeverity.Warning, obj, stackTrace, caller, filePath, lineNumber);
	public static void LogError(object obj, bool stackTrace = false, [CallerMemberName] string caller = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) =>
		Log_(LogSeverity.Error, obj, stackTrace, caller, filePath, lineNumber);
	public static void LogFatal(object obj, bool stackTrace = false, [CallerMemberName] string caller = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) =>
		Log_(LogSeverity.Fatal, obj, stackTrace, caller, filePath, lineNumber);
	public static void LogDebug(object obj, bool stackTrace = false, [CallerMemberName] string caller = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) =>
		Log_(LogSeverity.Debug, obj, stackTrace, caller, filePath, lineNumber);
	public static void LogMessage(object obj, bool stackTrace = false, [CallerMemberName] string caller = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) =>
		Log_(LogSeverity.Message, obj, stackTrace, caller, filePath, lineNumber);

	public static void Assert(bool condition, string message = "", bool stackTrace = false, [CallerArgumentExpression("condition")] string expression = "",
		[CallerMemberName] string caller = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
	{
		if (!condition)
			Log_(LogSeverity.Error, $"断言失败:{message}, {expression} is {condition}", stackTrace, caller, filePath, lineNumber);
	}
	#region 参数
	// 1个参数
	public static void LogVar<T1>(T1 v1, [CallerMemberName] string caller = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0,
		[CallerArgumentExpression("v1")] string n1 = "", bool stackTrace = false)
		 => Log_(LogSeverity.Info, $"{n1}:{v1}", stackTrace, caller, filePath, lineNumber);
	// 2个参数
	public static void LogVar<T1, T2>(T1 v1, T2 v2, [CallerMemberName] string caller = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0,
		[CallerArgumentExpression("v1")] string n1 = "", [CallerArgumentExpression("v2")] string n2 = "", bool stackTrace = false)
		 => Log_(LogSeverity.Info, $"{n1}:{v1}, {n2}:{v2}", stackTrace, caller, filePath, lineNumber);
	// 3个参数
	public static void LogVar<T1, T2, T3>(T1 v1, T2 v2, T3 v3, [CallerMemberName] string caller = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0,
		[CallerArgumentExpression("v1")] string n1 = "", [CallerArgumentExpression("v2")] string n2 = "", [CallerArgumentExpression("v3")] string n3 = "", bool stackTrace = false)
		 => Log_(LogSeverity.Info, $"{n1}:{v1}, {n2}:{v2}, {n3}:{v3}", stackTrace, caller, filePath, lineNumber);
	// 4个参数
	public static void LogVar<T1, T2, T3, T4>(T1 v1, T2 v2, T3 v3, T4 v4, [CallerMemberName] string caller = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0,
		[CallerArgumentExpression("v1")] string n1 = "", [CallerArgumentExpression("v2")] string n2 = "", [CallerArgumentExpression("v3")] string n3 = "",
		[CallerArgumentExpression("v4")] string n4 = "", bool stackTrace = false)
		 => Log_(LogSeverity.Info, $"{n1}:{v1}, {n2}:{v2}, {n3}:{v3}, {n4}:{v4}", stackTrace, caller, filePath, lineNumber);
	#endregion

	private static string _buildTime = File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location).ToString("mm:ss");

	public static object StringLog(object obj, bool stackTrace = false, string caller = "", string filePath = "", int lineNumber = 0)
	{
		try
		{
			if (obj is string s)
			{
				if (stackTrace)
				{
					StackTrace StackTrace = new StackTrace();
					StackFrame[] frames = StackTrace.GetFrames();

					//StackFrame?[] frames = new StackFrame?[3];
					//for (int i = 0; i < frames.Length; i++)
					//{
					//	int j = i + direct;

					//	if (j < allFrames.Length)
					//	{
					//		frames[i] = allFrames[j] ?? null;
					//	}
					//}

					MethodBase?[] callers = new MethodBase?[frames.Length];
					for (int i = 0; i < frames.Length; i++)
					{
						callers[i] = frames[i]?.GetMethod() ?? null;
					}

					string[] fileNames = new string[frames.Length];
					for (int i = 0; i < frames.Length; i++)
					{
						fileNames[i] = frames[i]?.GetFileName() ?? "";
					}

					string[] callerNames = new string[frames.Length];
					for (int i = 0; i < frames.Length; i++)
					{
						callerNames[i] = callers[i]?.Name ?? "";
					}

					int[] lineNumbers = new int[frames.Length];
					for (int i = 0; i < frames.Length; i++)
					{
						lineNumbers[i] = frames[i]?.GetFileLineNumber() ?? 0;
					}

					//directFrame = frames?[1]; // caller 直接调用者
					//grandFrame = frames?[2];  // grandCaller 调用者的调用者

					string className = Path.GetFileNameWithoutExtension(filePath);

					string str = s;
					obj = $"[{Plugin.version}][{DateTime.Now:mm:ss}]";
					for (int i = 0; i < frames.Length; i++)
					{
						obj += $"[{fileNames[i]}.{callerNames[i]}():{lineNumbers[i]}] => ";
					}
					obj += $"\n{s}";

					//obj = $"[{Plugin.version}]-[{DateTime.Now:mm:ss}] [{fileNames[2]}.{callerNames[2]}():{lineNumbers[2]}]=>[{className}.{caller}():{lineNumber}] {s}";
				}
				else
				{
					if (caller == "" && filePath == "" && lineNumber == 0)
					{
						obj = $"{Plugin.version}_{DateTime.Now:mm:ss} {s}";
					}
					else
					{
						string className = Path.GetFileNameWithoutExtension(filePath);
						obj = $"{Plugin.version}_{DateTime.Now:mm:ss} [{className}.{caller}:{lineNumber}] {s}";
					}
				}
			}
		}
		catch (Exception ex)
		{
			if (log != null)
			{
				log.LogError(ex);
			}
			else
			{
				Console.WriteLine(ex.Message);
			}
		}
		return obj;
	}

	private static void Log_(LogSeverity level, object obj, bool stackTrace = false, string caller = "", string filePath = "", int lineNumber = 0)
	{
		if (log == null) return;

		if (!ShouldLog(level)) return;

		obj = StringLog(obj, stackTrace, caller, filePath, lineNumber);

		switch (level)
		{
			case LogSeverity.Fatal:
				log.LogFatal(obj);
				break;
			case LogSeverity.Error:
				log.LogError(obj);
				break;
			case LogSeverity.Warning:
				log.LogWarning(obj);
				break;
			case LogSeverity.Info:
				log.LogInfo(obj);
				break;
			case LogSeverity.Debug:
				log.LogDebug(obj);
				break;
			case LogSeverity.Message:
				log.LogMessage(obj);
				break;
			default:
				log.LogInfo(obj);
				break;
		}

	}

	/*private enum LogLevel
	{
		None = 0,
		Message = 0,
		Fatal = 1,
		Error = 2,
		Warning = 3,
		Info = 4,
		Debug = 5
	}*/
	public enum LogSeverity
	{
		None = 0,
		Fatal = 1,
		Error = 2,
		Warning = 3,
		Message = 4,
		Info = 5,
		Debug = 6
	}


	private static bool ShouldLog(LogSeverity Level)
	{
		if (!EnableLog) return false;

		return Level <= CurrentSeverity;
	}

	#region LogLevel
	// Fatal 致命
	// Error 错误
	// Warning 警告
	// Message 消息
	// Info 信息
	// Debug 调试

	/*// 摘要:
	//     The level, or severity of a log entry.
	[Flags]
	public enum LogLevel_
	{
		//
		// 摘要:
		//     No level selected.
		None = 0,
		//
		// 摘要:
		//     A fatal error has occurred, which cannot be recovered from.
		Fatal = 1,
		//
		// 摘要:
		//     An error has occured, but can be recovered from.
		Error = 2,
		//
		// 摘要:
		//     A warning has been produced, but does not necessarily mean that something wrong
		//     has happened.
		Warning = 4,
		//
		// 摘要:
		//     An important message that should be displayed to the user.
		Message = 8,
		//
		// 摘要:
		//     A message of low importance.
		Info = 0x10,
		//
		// 摘要:
		//     A message that would likely only interest a developer.
		Debug = 0x20,
		//
		// 摘要:
		//     All log levels.
		All = 0x3F
	}*/
	#endregion
}

internal static class _LogFile
{
	private static bool LogReset = true;

	private static string _path => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Plugin.Name}_log.txt");

	public static void LogInfo(string s, bool stackTrace = false, [CallerMemberName] string caller = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
	{
		Write(s, stackTrace, caller, filePath, lineNumber);
	}

	public static void Write(string msg, bool stackTrace = false, string caller = "", string filePath = "", int lineNumber = 0)
	{
		if (!_Log.EnableLog)
		{
			return;
		}

		if (LogReset && !Plugin.DebugMode)
		{
			LogReset = false;
			File.WriteAllText(_path, $"======[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{Plugin.version}]====== Log Reset\n");
		}

		msg = (string)_Log.StringLog(msg, stackTrace, caller, filePath, lineNumber);

		/*if (!time)
		{
			File.AppendAllText(path, $"{msg}\n");
			return;
		}*/
		File.AppendAllText(_path, $"{msg}\n");
	}
}
