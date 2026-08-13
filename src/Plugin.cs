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
		Log.SetLog(base.Logger);
		Log.LogInfo("Awake");

		On.RainWorld.OnModsEnabled += RainWorld_OnModsEnabled;
		On.RainWorld.OnModsDisabled += RainWorld_OnModsDisabled;
	}

	void Update()
	{
		Debugger.Update();
	}

	public void OnEnable()
	{
		if (Plugin.isEnabled)
		{
			return;
		}
		Plugin.isEnabled = true;

		Log.SetLog(base.Logger);
		Log.LogInfo("OnEnable");

		// Put your custom hooks here!-在此放置你自己的钩子
		On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);
	}

	public void OnDisable()
	{
		if (!Plugin.isEnabled)
		{
			return;
		}
		Plugin.isEnabled = false;

		// Remove your custom hooks here!-在此取消你的钩子
		On.RainWorld.OnModsInit -= Extras.WrapInit(LoadResources);
	}


	// Load any resources, such as sprites or sounds-加载任何资源 包括图像素材和音效
	private void LoadResources(RainWorld rainWorld)
	{
		MachineConnector.SetRegisteredOI(GUID, new MyOptions());

		TrySyncStringsFile();
		On.Menu.Remix.InternalOI_Stats.Initialize += new On.Menu.Remix.InternalOI_Stats.hook_Initialize(this.InternalOI_Stats_InitializeHook);
		On.Menu.Remix.InternalOI_Stats._PreviewMod += new On.Menu.Remix.InternalOI_Stats.hook__PreviewMod(this.InternalOI_Stats__PreviewModHook);
	}

	#region Mod 的生命周期
	// ── Mod-enable lifecycle 启用 Mod 的生命周期 ────────────────────────────────────────────

	/// <summary>
	/// Called when any mods are enabled.  We register our ExtEnum type here so
	/// 当启用任何模组时调用此方法。我们在此处注册我们的 ExtEnum 类型，以便
	/// it participates in the game's dynamic enum system.
	/// 它能参与游戏的动态枚举系统。
	/// </summary>
	private void RainWorld_OnModsEnabled(On.RainWorld.orig_OnModsEnabled orig,
		RainWorld self, ModManager.Mod[] newlyEnabledMods)
	{
		orig(self, newlyEnabledMods);
		//EnderPearlFisob.RegisterValues();
	}

	/// <summary>
	/// Called when mods are disabled.  If *our* mod is among the disabled ones,
	/// 当模块被禁用时调用。如果*我们的*模块在被禁用的模块中，
	/// unregister the ExtEnum type so we don't leak enum values.
	/// 注销 ExtEnum 类型的注册，以免枚举值泄漏。
	/// </summary>
	private void RainWorld_OnModsDisabled(On.RainWorld.orig_OnModsDisabled orig,
		RainWorld self, ModManager.Mod[] newlyDisabledMods)
	{
		orig(self, newlyDisabledMods);
		foreach (var mod in newlyDisabledMods)
		{
			if (mod.id == GUID)
			{
				//EnderPearlFisob.UnregisterValues();
				break;
			}
		}
	}
	#endregion

	public const string Menu_tip_tip = "## 删除此行后向此文件中写入你想替换的文本，第一行为名称，其他行将作为简介（仅输入名称则不对简介进行替换），完成后请关闭此文件后点击确认替换。||你可以通过关闭此模组设置中的文件使用提示选项来使下次此文件中不会出现这行话。";
	public const string Menu_tip_nameW = "## 警告：已有为此模组设置的名称存在！确认替换将会覆盖上次修改。";
	public const string Menu_tip_dicW = "## 警告：已有为此模组设置的简介存在！确认替换将会覆盖上次修改。";

	public OpSimpleImageButton? renameButton;

	public static ModManager.Mod? CurrentPreviewMod { get; private set; }
	//private ModManager.Mod? Mod;

	// 读取并同步字符串文件
	internal static void TrySyncStringsFile()
	{
		string savePath = MyOptions.GetSavePath();
		string oldPath = MyOptions.GetSaveOldPath();
		string stringsPath = MyOptions.GetStringsPath();

		if (File.Exists(oldPath))
		{
			MyOptions.BakFile(savePath);
			File.AppendAllLines(savePath, File.ReadAllLines(oldPath));
			File.Delete(oldPath);
		}

		if (File.Exists(savePath))
		{
			Log.LogDebug($"同步字符串文件： savePath:{savePath}, stringsPath:{stringsPath}");
			File.WriteAllLines(stringsPath, File.ReadAllLines(savePath));
		}
		else if (File.Exists(stringsPath))
		{
			Log.LogDebug($"同步字符串文件： savePath:{savePath}, stringsPath:{stringsPath}");
			MyOptions.BakFile(savePath);
			File.WriteAllLines(savePath, File.ReadAllLines(stringsPath));
		}
	}

	// 初始化时在模组信息界面添加按钮
	private void InternalOI_Stats_InitializeHook(On.Menu.Remix.InternalOI_Stats.orig_Initialize orig, InternalOI_Stats self)
	{
		orig.Invoke(self);

		Futile.atlasManager.LoadAtlas("assets/ModRenameButton_Icons");
		this.renameButton = new OpSimpleImageButton(new Vector2(520f, 510f), new Vector2(30f, 30f), "ModRenameButton_Icon")// 560f, 510f|520f, 440f
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
