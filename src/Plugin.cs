#region using
using BepInEx;
using BepInEx.Logging;
using Fisobs;
using Fisobs.Core;
using Fisobs.Items;
using IL;
using Menu.Remix;
using Menu.Remix.MixedUI;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MoreSlugcats;
using Noise;
using On;
using RewiredConsts;
using RWCustom;
using Smoke;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEngine;
using Watcher;
using static PhysicalObject;
using static Helper;
#endregion
namespace Translator;

[BepInPlugin(Plugin.GUID, Plugin.NAME, Plugin.VERSION)]
public sealed class Plugin : BaseUnityPlugin
{
	public const string GUID = "Lvye_Translator";
	public const string NAME = "Translator";
	public const string VERSION = "0.1.0";

	public const string version = "0.1.0";
	public const string Name = "Translator";

#if DEBUG
	public static bool DebugMode { get; set; } = true;//  false
	private static bool EnableStartScreen = true;// true
	public static bool EnableLog = true;// false
#else
	public const bool DebugMode = false;
	private const bool EnableStartScreen = true;
	public const bool EnableLog = false;
#endif


	private static bool isEnabled;

	public void Awake()
	{
	}
	void Update()
	{
		Debugger.Update();
	}

	// 已注册的钩子委托实例，OnDisable 时据此准确注销
	private static On.RainWorld.hook_OnModsInit? _onModsInitHook;
	private static On.Menu.Remix.InternalOI_Stats.hook_Initialize? _statsInitHook;
	private static On.Menu.Remix.InternalOI_Stats.hook__PreviewMod? _statsPreviewHook;
	public void OnEnable()
	{
		if (Plugin.isEnabled)
		{
			return;
		}
		Plugin.isEnabled = true;

		Log.SetLog(base.Logger);

		// Put your custom hooks here!-在此放置你自己的钩子
		// 保存委托实例，保证 OnDisable 时能准确注销
		_onModsInitHook = Extras.WrapInit(LoadResources);
		On.RainWorld.OnModsInit += _onModsInitHook;
	}

	public void OnDisable()
	{
		if (!Plugin.isEnabled)
		{
			return;
		}
		Plugin.isEnabled = false;

		// Remove your custom hooks here!-在此取消你的钩子
		if (_onModsInitHook != null)
		{
			On.RainWorld.OnModsInit -= _onModsInitHook;
			_onModsInitHook = null;
		}
		if (_statsInitHook != null)
		{
			On.Menu.Remix.InternalOI_Stats.Initialize -= _statsInitHook;
			_statsInitHook = null;
		}
		if (_statsPreviewHook != null)
		{
			On.Menu.Remix.InternalOI_Stats._PreviewMod -= _statsPreviewHook;
			_statsPreviewHook = null;
		}

		// 允许禁用后重新启用时再次执行 LoadResources
		Extras.ResetInitialized();
	}


	// Load any resources, such as sprites or sounds-加载任何资源 包括图像素材和音效
	private void LoadResources(RainWorld rainWorld)
	{
		// 各初始化步骤相互隔离：任一步失败都不影响其余步骤
		try
		{
			MachineConnector.SetRegisteredOI(GUID, new MyOptions());
		}
		catch (Exception e)
		{
			Log.LogError($"注册设置页失败: {e}");
		}

		try
		{
			TrySyncStringsFile();
		}
		catch (Exception e)
		{
			Log.LogError($"同步字符串文件失败: {e}");
		}

		try
		{
			_statsInitHook = new On.Menu.Remix.InternalOI_Stats.hook_Initialize(this.InternalOI_Stats_InitializeHook);
			On.Menu.Remix.InternalOI_Stats.Initialize += _statsInitHook;

			_statsPreviewHook = new On.Menu.Remix.InternalOI_Stats.hook__PreviewMod(this.InternalOI_Stats__PreviewModHook);
			On.Menu.Remix.InternalOI_Stats._PreviewMod += _statsPreviewHook;
		}
		catch (Exception e)
		{
			Log.LogError($"注册模组信息页钩子失败: {e}");
		}
	}


	public OpSimpleImageButton? renameButton; // 模组信息页的编辑按钮
	public static ModManager.Mod? CurrentPreviewMod { get; private set; }

	// 读取并同步字符串文件
	internal static void TrySyncStringsFile()
	{
		string savePath = MyOptions.GetSavePath();
		string oldPath = MyOptions.GetSaveOldPath();
		string stringsPath = MyOptions.GetStringsPath();

		// 旧版存档迁移：按键合并进新存档（新存档中已有的键优先），完成后删除旧文件，不丢失任一侧内容
		if (File.Exists(oldPath))
		{
			Log.LogDebug($"迁移旧版字符串文件： oldPath:{oldPath}, savePath:{savePath}");
			MergeModStringsFiles(oldPath, savePath);
			File.Delete(oldPath);
		}

		if (File.Exists(savePath))
		{
			Log.LogDebug($"同步字符串文件： savePath:{savePath}, stringsPath:{stringsPath}");
			// 先合并模组随更新带来的新条目（save 中没有的键才补入），避免模组更新被用户存档遮蔽；
			// 用户已删除的键同样不在 save 中，不会被补回
			MergeModStringsFiles(stringsPath, savePath);
			WriteStringsSaveToLanguageFile(savePath, stringsPath);
		}
		else if (File.Exists(stringsPath))
		{
			Log.LogDebug($"反向同步字符串文件： savePath:{savePath}, stringsPath:{stringsPath}");
			WriteModStringsToSaveFile(stringsPath, savePath);
		}

		// 修复 bug1：把同步后的翻译表加载到游戏翻译器内存，确保非中文语言下立即生效，而不必等到下一次启动
		MyOptions.ReloadShortStrings();

		MyOptions.ClearTempFile();
	}


	// 从 "key|value" 行中取出 key；不含 '|' 的行返回 null
	private static string? GetKey(string line)
	{
		if (string.IsNullOrEmpty(line) || !line.Contains('|')) return null;
		string[] parts = line.Split(new char[] { '|' }, 2);
		return parts.Length == 2 ? parts[0] : null;
	}
	// 判断是否为模组翻译键（ModID-name / ModID-description），用于与 UI 译文键区分
	private static bool IsModKey(string? key)
	{
		return key != null && (key.EndsWith("-name") || key.EndsWith("-description"));
	}

	// 将 fromPath 中不存在于 toPath 的键追加到 toPath（保留 toPath 原有顺序与内容，按键去重，重复键以 toPath 为准）
	// 仅把 fromPath 中的模组翻译键合并到 toPath，忽略 UI 译文键，避免语言切换后 UI 被旧语言覆盖
	private static void MergeModStringsFiles(string fromPath, string toPath)
	{
		if (!File.Exists(fromPath)) return;

		string[] fromLines = File.ReadAllLines(fromPath);
		string[] toLines = File.Exists(toPath) ? File.ReadAllLines(toPath) : Array.Empty<string>();

		HashSet<string> seen = new HashSet<string>();
		List<string> merged = new List<string>(toLines.Length + fromLines.Length);

		foreach (string line in toLines)
		{
			string? key = GetKey(line);
			if (key != null && !seen.Add(key)) continue; // 重复键只保留第一条
			merged.Add(line);
		}
		foreach (string line in fromLines)
		{
			string? key = GetKey(line);
			if (key == null || !IsModKey(key) || !seen.Add(key)) continue;
			merged.Add(line);
		}

		File.WriteAllLines(toPath, merged);
	}

	// 将 savePath 中的模组翻译写回当前语言 stringsPath，同时保留 stringsPath 中的 UI 译文行
	private static void WriteStringsSaveToLanguageFile(string savePath, string stringsPath)
	{
		string[] uiLines = File.ReadAllLines(stringsPath);
		string[] saveLines = File.Exists(savePath) ? File.ReadAllLines(savePath) : Array.Empty<string>();

		List<string> result = new List<string>();
		HashSet<string> seen = new HashSet<string>();

		foreach (string line in uiLines)
		{
			string? key = GetKey(line);
			if (key == null || IsModKey(key)) continue; // 保留 UI 译文
			if (!seen.Add(key)) continue;
			result.Add(line);
		}

		foreach (string line in saveLines)
		{
			string? key = GetKey(line);
			if (key == null || !IsModKey(key) || !seen.Add(key)) continue;
			result.Add(line);
		}

		File.WriteAllLines(stringsPath, result);
	}

	// 将 stringsPath 中的模组翻译键写入 savePath，*忽略 UI 译文，避免污染存档*
	private static void WriteModStringsToSaveFile(string stringsPath, string savePath)
	{
		string[] stringsLines = File.ReadAllLines(stringsPath);
		List<string> result = new List<string>();
		HashSet<string> seen = new HashSet<string>();

		foreach (string line in stringsLines)
		{
			string? key = GetKey(line);
			//if (key == null || !IsModKey(key) || !seen.Add(key)) continue;
			if (key == null || !seen.Add(key)) continue;
			result.Add(line);
		}

		File.WriteAllLines(savePath, result);
	}

	

	// 初始化时在模组信息界面添加按钮
	private void InternalOI_Stats_InitializeHook(On.Menu.Remix.InternalOI_Stats.orig_Initialize orig, InternalOI_Stats self)
	{
		orig.Invoke(self);

		Futile.atlasManager.LoadAtlas("assets/ModRenameButton_Icons");
		this.renameButton = new OpSimpleImageButton(new Vector2(520f, 510f), new Vector2(30f, 30f), "ModRenameButton_Icon")// 560f 440f
		{
			description = T("Rename_Button_Desc"),
        };
		this.renameButton.OnClick += RenameButton_OnClick;
		// 索引1 模组列表标签页?
		self.Tabs[1].AddItems(new UIelement[] { this.renameButton });
	}

	// 点击按钮时的处理逻辑
	private void RenameButton_OnClick(UIfocusable trigger)
	{
		if (MyOptions.CheckTempFile())
		{
			if (MyOptions.CheckTempFileFor(CurrentPreviewMod?.id))
			{
				MyOptions.AddToStrings(CurrentPreviewMod?.id);
			}
			else
			{
				// 修复 bug2：临时文件属于其它模组时，不要误应用；为当前预览模组重新打开临时文件
				Log.LogWarning($"临时文件属于模组 {MyOptions.TempFileModId}，当前预览为 {CurrentPreviewMod?.id}。已为当前模组重新打开临时文件，原内容将被覆盖。");
				MyOptions.OpenTempFile(CurrentPreviewMod?.id);
			}
		}
		else
		{
			MyOptions.OpenTempFile(CurrentPreviewMod?.id);
		}
	}

	private void InternalOI_Stats__PreviewModHook(On.Menu.Remix.InternalOI_Stats.orig__PreviewMod orig, InternalOI_Stats self, MenuModList.ModButton button)
	{
		orig.Invoke(self, button);

		CurrentPreviewMod = self.previewMod;
		if (this.renameButton != null)
		{
			//if (Custom.rainWorld.processManager.mySteamManager != null)
			if (true)
			{
				this.renameButton.Show();
			}
			else
			{
				this.renameButton.Hide();
			}
		}
	}


}
