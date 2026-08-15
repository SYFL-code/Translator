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
#endregion
namespace Translator;

[BepInPlugin(Plugin.GUID, Plugin.NAME, Plugin.VERSION)]
public sealed class Plugin : BaseUnityPlugin
{
	public const string GUID = "Lvye_Translator";
	public const string NAME = "Translator";
	public const string VERSION = "0.1.0";

	public const string version = "0.1.1";
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

	public const string Menu_tip_tip1 = "# 此文件中写入你想替换的文本，第一行为名称，其他行将作为简介；某一行留空表示删除该行的翻译并回退为原文，完成后请保存关闭此文件，点击确认替换。";
    public const string Menu_tip_tip2 = "# 以 '#' 开头的行是注释，会被忽略";
    public const string Menu_tip_tip3 = "# 你可以通过关闭此模组设置中的文件使用提示选项来使下次此文件中不会出现这三行话。";
    public const string Menu_tip_nameW = "# 警告：已有为此模组设置的名称存在！确认替换将会覆盖上次修改。";
	public const string Menu_tip_dicW = "# 警告：已有为此模组设置的简介存在！确认替换将会覆盖上次修改。";

	public OpSimpleImageButton? renameButton;
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
			MergeStringsFiles(oldPath, savePath);
			File.Delete(oldPath);
		}

		if (File.Exists(savePath))
		{
			Log.LogDebug($"同步字符串文件： savePath:{savePath}, stringsPath:{stringsPath}");
			// 先合并模组随更新带来的新条目（save 中没有的键才补入），避免模组更新被用户存档遮蔽；
			// 用户已删除的键同样不在 save 中，不会被补回
			MergeStringsFiles(stringsPath, savePath);
			File.WriteAllLines(stringsPath, File.ReadAllLines(savePath));
		}
		else if (File.Exists(stringsPath))
		{
			Log.LogDebug($"反向同步字符串文件： savePath:{savePath}, stringsPath:{stringsPath}");
			MyOptions.BackupFile(savePath);
			File.WriteAllLines(savePath, File.ReadAllLines(stringsPath));
		}
	}

	// 将 fromPath 中不存在于 toPath 的键追加到 toPath（保留 toPath 原有顺序与内容，按键去重，重复键以 toPath 为准）
	private static void MergeStringsFiles(string fromPath, string toPath)
	{
		if (!File.Exists(fromPath)) return;

		string[] fromLines = File.ReadAllLines(fromPath);
		string[] toLines = File.Exists(toPath) ? File.ReadAllLines(toPath) : Array.Empty<string>();

		HashSet<string> seen = new HashSet<string>();
		List<string> merged = new List<string>(toLines.Length + fromLines.Length);

		foreach (string line in toLines)
		{
			string? key = GetStringsKey(line);
			if (key != null && !seen.Add(key)) continue; // 重复键只保留第一条
			merged.Add(line);
		}
		foreach (string line in fromLines)
		{
			string? key = GetStringsKey(line);
			if (key == null || !seen.Add(key)) continue; // 只补入缺失的键
			merged.Add(line);
		}

		File.WriteAllLines(toPath, merged);
	}

	// 从 "key|value" 行中取出 key；不含 '|' 的行返回 null
	private static string? GetStringsKey(string line)
	{
		if (string.IsNullOrEmpty(line) || !line.Contains('|')) return null;
		string[] parts = line.Split(new char[] { '|' }, 2);
		return parts.Length == 2 ? parts[0] : null;
	}

	// 初始化时在模组信息界面添加按钮
	private void InternalOI_Stats_InitializeHook(On.Menu.Remix.InternalOI_Stats.orig_Initialize orig, InternalOI_Stats self)
	{
		orig.Invoke(self);

		Futile.atlasManager.LoadAtlas("assets/ModRenameButton_Icons");
		this.renameButton = new OpSimpleImageButton(new Vector2(520f, 510f), new Vector2(30f, 30f), "ModRenameButton_Icon")// 560f 440f
        {
			description = "修改模组名称及简介"
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
			MyOptions.AddToStrings(CurrentPreviewMod?.id);
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
			if (Custom.rainWorld.processManager.mySteamManager != null || true)
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
