using Menu.Remix;
using Menu.Remix.MixedUI;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static Helper;
using static UnityEngine.UI.Image;

namespace Translator
{
	internal class MyOptions : OptionInterface
	{
		public static MyOptions? Instance;

		public static RainWorld RainWorld => Custom.rainWorld;
		public static InGameTranslator inGameTranslator => RainWorld.inGameTranslator;
		public static InGameTranslator Trans => inGameTranslator;
		public static string lang => LocalizationTranslator.LangShort(Trans.currentLanguage);

		public Configurable<string> idc;
		//public Configurable<bool> startc;
		//public Configurable<bool> openc;
		public Configurable<bool> enabletur;

		public MyOptions()
		{
			Instance = this;

			// 配置4: 目标模组 ID 输入框绑定的配置值
			this.idc = this.config.Bind<string>("LYR_ChModList_idc", "", new ConfigurableInfo("targetID", null, "", Array.Empty<object>()));

			// 配置2: 启动时自动检查（当前未使用，预留功能）
			//this.startc = this.config.Bind<bool>("LYR_ChModList_start", false);

			// 配置3: 打开临时文件相关选项（当前未使用，预留功能）
			//this.openc = this.config.Bind<bool>("LYR_ChModList_opentempfile", false);

			// 配置1: 是否启用临时文件格式提示（默认开启）
			enabletur = this.config.Bind<bool>("LYR_ChModList_enable", true);
		}

		private float by1 = 570f;
		private float by2 = 570f;
		private float by3 = 570f;

		public int tipClearTimer = 0;

		// 修复 bug2：记录当前临时文件属于哪个模组，避免“确认替换”时把 A 的内容写进 B
		public static string? TempFileModId { get; private set; }

		public OpTextBox? id; // 模组 ID 输入框
		public OpSimpleButton? opentempfile; // 打开临时文件按钮
		public OpSimpleButton? startAdd; // 确认替换按钮
		private OpLabel? tips; // 动态提示标签，用于显示操作反馈信息
		// 批量翻译相关控件
		private OpSimpleButton? exportAllButton;
		private OpSimpleButton? importAllButton;
		private OpCheckBox? enableBox; // 复选框，用于启用/禁用文件格式提示

		public override void Initialize()
		{
			this.by1 = (this.by2 = (this.by3 = 570f));

			base.Initialize();

			this.Tabs = new OpTab[]
			{
				new OpTab(this, T("Option"))
			};
			// 标题
			OpLabel title = new OpLabel(20f, this.GetnextY(0f, 0, "null"), T("Op_Translator"), true);

			// 动态提示标签
			this.tips = new OpLabel(280f, this.GetnextY(0f, 0, "null"), "", false);

			// 模组 ID 输入框
			this.id = new OpTextBox(this.idc, new Vector2(30f, this.GetnextY(40f, 0, "null")), 400f);
			OpLabel idInputTip = new OpLabel(450f, this.GetnextY(0f, 0, "null"), T("Op_ID_Input_Tip"), false);

			// 操作按钮
			this.opentempfile = new OpSimpleButton(new Vector2(30f, this.GetnextY(30f, 0, "null")), new Vector2(200f, 30f), T("Op_Add_Trans"))
			{
				description = T("Op_Add_Trans_Desc")
			};
			this.startAdd = new OpSimpleButton(new Vector2(30f, this.GetnextY(35f, 0, "null")), new Vector2(200f, 30f), T("Op_Confirm_Replace"))
			{
				description = T("Op_Confirm_Replace_Desc")
			};

			// 批量翻译按钮
			this.exportAllButton = new OpSimpleButton(new Vector2(30f, this.GetnextY(70f, 0, "null")), new Vector2(200f, 30f), T("Op_Batch_Trans"))
			{
				description = T("Op_Batch_Trans_Desc")
			};
			this.importAllButton = new OpSimpleButton(new Vector2(30f, this.GetnextY(35f, 0, "null")), new Vector2(200f, 30f), T("Op_Apply_All"))
			{
				description = T("Op_Apply_All_Desc")
			};
			OpLabel batchTip = new OpLabel(240f, this.exportAllButton.pos.y, T("Op_Batch_Tip"), false);

			// 配置复选框
			this.enableBox = new OpCheckBox(this.enabletur, new Vector2(30f, 40f));
			OpLabel enablefiletip = new OpLabel(55f, 43f, T("Op_Enable_File_Tip"), false);

			// 绑定事件
			this.opentempfile.OnClick += this.Opentempfile_OnClick;
			this.startAdd.OnClick += this.StartAdd_OnClick;
			this.exportAllButton.OnClick += this.ExportAllButton_OnClick;
			this.importAllButton.OnClick += this.ImportAllButton_OnClick;

			// 将所有元素添加到标签页
			this.Tabs[0].AddItems(new UIelement[]
			{
				title, this.tips, this.id, idInputTip,
				this.opentempfile, this.startAdd,
				this.exportAllButton, this.importAllButton, batchTip,
				this.enableBox, enablefiletip
			});
		}

		private float GetnextY(float distance, int page = 0, string spc = "null")
		{
			float fix = 0f;
			if (spc == "b") fix = 3f;       // 小间距补偿
			if (spc == "s") fix = 6f;       // 中间距补偿

			switch (page)
			{
				case 0: this.by1 -= distance; return this.by1 + fix;
				case 1: this.by2 -= distance; return this.by2 + fix;
				default: this.by3 -= distance; return this.by3 + fix;
			}
		}

		public override void Update()
		{
			if (this.tipClearTimer > 0)
			{
				this.tipClearTimer--;
			}
			if (this.tipClearTimer == 0)
			{
				this.tips!.text = "";
			}
			base.Update();
		}

		#region 文件
		private static string? _cachedModRoot;
		public static string GetPath()
		{
			if (_cachedModRoot != null) return _cachedModRoot;

			// 获取当前 DLL 所在目录
			string? dllDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			// 从 DLL 目录逐级向上查找 modinfo.json
			string? dir = dllDir;
			while (dir != null)
			{
				if (File.Exists(Path.Combine(dir, "modinfo.json")))
				{
					_cachedModRoot = dir;
					Log.LogInfo($"模组根目录已缓存: {dir}");
					return dir;
				}
				dir = Path.GetDirectoryName(dir);
			}
			Log.LogError("无法找到 modinfo.json，请确认模组结构完整。");

			//try
			//{
			//	string stringsRelPath = $"text/text_{LocalizationTranslator.LangShort(Trans.currentLanguage)}/strings.txt";
			//	string resolvedPath = ResolveFilePathFromMod(stringsRelPath, Plugin.GUID, false);

			//	if (File.Exists(resolvedPath))
			//	{
			//		// 从已确认存在的 strings.txt 向上推导根目录
			//		// text/text_xx/strings.txt → 向上3层到模组根目录
			//		string? root = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(resolvedPath)));
			//		if (!string.IsNullOrEmpty(root) && Directory.Exists(root))
			//		{
			//			_cachedModRoot = root;
			//			Log.LogInfo($"模组根目录已缓存: {root}");
			//			return root;
			//		}
			//	}
			//}
			//catch (Exception ex)
			//{
			//	Log.LogError($"解析失败: {ex.Message}");
			//}
			Log.LogError($"解析失败。DLL位置: {dllDir}, GUID: {Plugin.GUID}");
			throw new FileNotFoundException("解析失败");

			// 获取当前 DLL 所在目录: plugins/
			//string pluginDir = Path.GetDirectoryName(typeof(MyOptions).Assembly.Location);
			// 向上一级回到模组根目录
			//string modRoot = Directory.GetParent(pluginDir).FullName;
			//return modRoot;
		}
		public static string GetStringsPath()
		{
			string langDir = Path.Combine(MyOptions.GetPath(), "text", "text_" + lang);
			string path = Path.Combine(langDir, "strings.txt");

			// 修复 bug1：所有语言都应该有对应的 text_<语言> 目录，先创建目录，避免 File.Create 抛 DirectoryNotFoundException
			Directory.CreateDirectory(langDir);

			if (!File.Exists(path))
			{
				//File.Create(path);
				using (File.Create(path)) { } // 立即释放句柄
			}
			return path;
		}
		public static string GetTranslatorPath(string? language = null)
		{
			string langDir = Path.Combine(MyOptions.GetPath(), "text", "text_" + (language ?? lang));
			string path = Path.Combine(langDir, "translator.json");

			Directory.CreateDirectory(langDir);
			return path;
		}
		public static string GetTempPath()
		{
			string path = Path.Combine(MyOptions.GetPath(), "temp.txt");

			if (!File.Exists(path))
			{
				//File.Create(path);
				using (File.Create(path)) { } // 立即释放句柄
			}
			return path;
		}
		public static string[] GetStrings()
		{
			return File.ReadAllLines(GetStringsPath());

			//if (!File.Exists(fullPath))
			//{
			//	File.Create(fullPath);
			//}
			//return from i in File.ReadAllLines(fullPath)
			//	   where !string.IsNullOrWhiteSpace(i) && !i.TrimStart(Array.Empty<char>()).StartsWith("||")
			//	   select i;
		}

		// 将当前语言 strings.txt 中的全部键值同步到游戏翻译器内存，保证运行时修改/补齐后立即生效
		public static void ReloadShortStrings()
		{
			List<string> keys = new();
			List<string> values = new();

			foreach (string line in GetStrings())
			{
				if (string.IsNullOrEmpty(line) || !line.Contains('|')) continue;

				string[] parts = line.Split(new char[] { '|' }, 2);
				if (parts.Length != 2) continue;

				keys.Add(parts[0]);
				values.Add(parts[1]);
			}

			if (keys.Count > 0)
			{
				try
				{
					SetToShortStrings(keys.ToArray(), values.ToArray());
				}
				catch (Exception e)
				{
					Log.LogWarning($"ReloadShortStrings 同步到翻译器失败: {e}");
				}
			}
		}

		public static string[] GetTemp()
		{
			return File.ReadAllLines(GetTempPath());

			//string path = "text/text_" + LocalizationTranslator.LangShort(MyOptions.rainWorld.inGameTranslator.currentLanguage) + "/temp.txt";
			//string fullPath = MyOptions.ResolveFilePathFromMod(path, "Lvye_AnotherNameForMods", false);
			//bool flag = !File.Exists(fullPath);
			//if (flag)
			//{
			//	File.Create(fullPath);
			//}
			//return from i in File.ReadAllLines(fullPath)
			//	   where !string.IsNullOrWhiteSpace(i) && !i.TrimStart(Array.Empty<char>()).StartsWith("||")
			//	   select i;
		}
		public static void ClearTempFile()
		{
			// 拼接目标文件路径
			//string path = "text/text_" + LocalizationTranslator.LangShort(MyOptions.rainWorld.inGameTranslator.currentLanguage) + "/temp.txt";
			//string fullPath = AssetManager.ResolveFilePath(path);

			File.WriteAllText(GetTempPath(), "");
		}
		public static string GetSavePath()
		{
			string path = Path.Combine(Application.persistentDataPath, "ModConfigs", $"ModTranslatorSave_{lang}.txt");
			string? dir = Path.GetDirectoryName(path);
			if (!string.IsNullOrEmpty(dir))
			{
				Directory.CreateDirectory(dir);
			}
			return path;
		}
		public static string GetSaveOldPath()
		{
			return Path.Combine(Application.persistentDataPath, "ly.ModRename_stringsSave.txt");
		}
		public static string GetAllMods()
		{
			string path = Path.Combine(MyOptions.GetPath(), "ModRename_allMods.txt");

			if (!File.Exists(path))
			{
				using (File.Create(path)) { }
			}
			return path;
			//return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ModRename_allMods.txt");
		}
		public static string GetGameRoot()
		{
			return AppDomain.CurrentDomain.BaseDirectory;
		}

		static int maxBackups = 20;
		public static void BackupFile(string path)
		{
			if (!File.Exists(path)) return;

			string backupDir = Path.Combine(MyOptions.GetPath(), "backup");
			if (!Directory.Exists(backupDir))
				Directory.CreateDirectory(backupDir);

			string fileName = Path.GetFileNameWithoutExtension(path);
			string ext = Path.GetExtension(path);
			string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
			//string backupName = $"{fileName}_{timestamp}{ext}.bak";
			string backupName = $"{fileName}_{timestamp}{ext}";
			string backupPath = Path.Combine(backupDir, backupName);

			// 复制到备份目录（保留原文件）
			File.Copy(path, backupPath, overwrite: true);

			// 清理旧备份
			CleanupOldBackups(backupDir, fileName, maxBackups);
		}
		private static void CleanupOldBackups(string backupDir, string fileName, int maxBackups)
		{
			//var backups = Directory.GetFiles(backupDir, $"{fileName}_*.bak")

			string ext = Path.GetExtension(fileName);  // 获取原始扩展名，如 ".txt"
			var backups = Directory.GetFiles(backupDir, $"{fileName}_*{ext}")
				.Select(f => new FileInfo(f))
				.OrderByDescending(f => f.CreationTime)
				.Skip(maxBackups)
				.ToList();

			foreach (var file in backups)
			{
				file.Delete();
			}
		}
		#endregion

		#region 绑定事件
		// 点击打开临时文件
		private void Opentempfile_OnClick(UIfocusable trigger)
		{
			try
			{
				string id = this.id?.value ?? "";

				Log.LogDebug($"Opentempfile_OnClick: id = {id}");

				if (!string.IsNullOrEmpty(id))
				{
					//Log.LogDebug($"Installed mods: {string.Join(", ", ModManager.InstalledMods.Select(m => m.id))}");
					//Log.LogDebug($"Contains: {ModManager.InstalledMods.Contains(ModManager.GetModById(id))}");

					// 检查模组是否已安装（首次点击给出警告，二次点击强制继续）
					if (!ModManager.InstalledMods.Contains(ModManager.GetModById(id)) && TempFileModId != id)
					{
						TempFileModId = id; // 记录当前临时文件属于哪个模组
						Log.LogDebug($"模组 {id} 未安装，提示用户确认。");

						this.tipClearTimer = 120;
						this.tips!.text = T("Tip_Uninstalled_Mod", id);
					}
					else
					{
						Log.LogDebug($"模组 {id} 已安装，尝试打开临时文件。");

						this.tipClearTimer = 20;
						this.tips!.text = T("Tip_Opening_File");

						OpenTempFile(id);
					}
				}
			}
			catch (Exception ex)
			{
				Log.LogError(ex);
			}
		}
		// "确认替换"按钮点击事件
		// 读取临时文件内容并写入持久化字符串表
		private void StartAdd_OnClick(UIfocusable trigger)
		{
			// 验证临时文件内容是否有效
			if (!MyOptions.CheckTempFile())
			{
				this.tipClearTimer = 120; // 提示显示约2秒（60fps下）
				this.tips!.text = T("Tip_Invalid_File");
				return;
			}
			// 修复 bug2：确认临时文件属于当前输入的模组 ID，避免误应用到其它模组
			if (!MyOptions.CheckTempFileFor(this.id?.value))
			{
				this.tipClearTimer = 120;
				this.tips!.text = string.Format(T("Tip_Wrong_Mod"), MyOptions.TempFileModId, this.id?.value);
				return;
			}

			// 将临时文件内容写入字符串表
			MyOptions.AddToStrings(this.id?.value);
			this.tipClearTimer = 120;
			this.tips!.text = T("Tip_Add_Success");
			return;


			#if false // 旧代码块已由上方修复逻辑替代，不再编译

			{
				this.tipClearTimer = 120; // 提示显示约2秒（60fps下）
				this.tips!.text = "文件内容无效！是否忘记保存temp文件？";
				return;
			}
			// 将临时文件内容写入字符串表
			MyOptions.AddToStrings(this.id?.value);
			this.tipClearTimer = 120;
			this.tips!.text = "添加成功！";

			#endif

		}
		private static Dictionary<string, ModManager.Mod> installedMap
		{
			get
			{
				if (_installedMap != null)
				{
					if (_installedMap.Count != ModManager.InstalledMods.Count)
					{
						_installedMap = null;
					}
				}
				if (_installedMap == null)
				{
					var groups = ModManager.InstalledMods.GroupBy(m => m.id);
					var dict = new Dictionary<string, ModManager.Mod>();

					foreach (var group in groups)
					{
						var mods = group.ToList();
						if (mods.Count > 1)
						{
							Log.LogWarning($"模组 ID '{group.Key}' 重复出现 {mods.Count} 次，将使用第一个。");
						}
						dict[group.Key] = mods.First();
					}

					_installedMap = dict;
				}
				return _installedMap;
			}
		}
		private static Dictionary<string, ModManager.Mod>? _installedMap;
		public static List<string> SortExportIds(IEnumerable<string> idsToExport)
		{
			var inInstalled = new List<(string id, string name)>();
			var notInstalled = new List<string>();

			foreach (string id in idsToExport)
			{
				if (installedMap.TryGetValue(id, out var mod))
					inInstalled.Add((id, mod.name));
				else
					notInstalled.Add(id);
			}

			return inInstalled
				.OrderBy(x => x.name, StringComparer.Ordinal)
				.Select(x => x.id)
				.Concat(notInstalled.OrderBy(s => s, StringComparer.Ordinal))
				.ToList();
		}
		private void ExportAllButton_OnClick(UIfocusable trigger)
		{
			try
			{
				string filePath = GetAllMods();

				// 1. 收集所有需要导出的模组ID
				HashSet<string> idsToExport = new HashSet<string>();

				// 一次性构建键索引缓存
				string[] allStrings = GetStrings();
				Dictionary<string, int> keyIndexCache = new(StringComparer.Ordinal);
				for (int k = 0; k < allStrings.Length; k++)
				{
					string? key = Plugin.GetKey(allStrings[k]);
					if (key != null)
					{
						if (!keyIndexCache.ContainsKey(key))
						{
							keyIndexCache[key] = k;
						}

						// 从保存文件获取（包含未安装的翻译）
						string[] parts = key.Split(new char[] { '-' }, 2);
						if (parts.Length == 2)
						{
							idsToExport.Add(parts[0]);
						}
					}
				}


				// 从已安装模组获取
				foreach (var mod in ModManager.InstalledMods)
					idsToExport.Add(mod.id);

				for (int j = 0; j < 2; j++)
				{
					string with = (j == 0) ? "-name" : "-description";

					// 从 shortStrings 获取
					foreach (var key in inGameTranslator.shortStrings.Keys)
					{
						if (key.EndsWith(with))
						{
							string id = key.Substring(0, key.Length - with.Length); // 去掉 "-name" 或 "-description"
							idsToExport.Add(id);
						}
					}

					
					//string[] strings = MyOptions.GetStrings();
					//for (int i = 0; i < strings.Length; i++)
					//for (int i = 0; i < allStrings.Length; i++)
					//{
					//	//string line = strings[i];
					//	string line = allStrings[i];

					//	if (!line.Contains('|')) continue;

					//	string[] keyAndValue = line.Split(new char[] { '|' }, 2);
					//	if (keyAndValue.Length != 2) continue;

					//	string lineKey = keyAndValue[0];
					//	if (lineKey.EndsWith(with))
					//	{
					//		string id = lineKey.Substring(0, lineKey.Length - with.Length);
					//		idsToExport.Add(id);
					//	}
					//}
				}

				List<string> sorted = SortExportIds(idsToExport);

				using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
				{
					// 写入文件头注释
					writer.WriteLine("# ============================================");
					writer.WriteLine($"# {T("Batch_Trans_File_Guide_1")}");
					writer.WriteLine("# ============================================");
					writer.WriteLine($"# {T("Batch_Trans_File_Guide_2")}");
					writer.WriteLine($"# {T("Batch_Trans_File_Guide_3")}");
					writer.WriteLine($"# {T("Batch_Trans_File_Guide_4")}");
					writer.WriteLine($"# {T("Batch_Trans_File_Guide_5")}");
					writer.WriteLine($"# {T("Batch_Trans_File_Guide_6")}");
					writer.WriteLine($"# {T("Batch_Trans_File_Guide_7")}");
					writer.WriteLine($"# {T("Batch_Trans_File_Guide_8")}");
					writer.WriteLine($"# {T("Batch_Trans_File_Guide_9")}");
					writer.WriteLine($"# {T("Batch_Trans_File_Guide_10")}");
					writer.WriteLine($"# {T("Batch_Trans_File_Guide_11")}");
					writer.WriteLine($"# {T("Batch_Trans_File_Guide_12")}");
					writer.WriteLine($"# {T("Batch_Trans_File_Guide_13")}");
					writer.WriteLine("# ============================================");
					writer.WriteLine();

					//writer.WriteLine("# 本文件用于批量管理模组翻译");
					//writer.WriteLine("# 因翻译索引的原因，此模组若不在模组排序的顶部，则可能无法应用翻译");
					//writer.WriteLine("# 格式：每个模组以 '|模组ID' 开始，其后四行分别为：");
					//writer.WriteLine("# 原始名称、原始描述、翻译名称（留空或注释表示删除该翻译）、翻译描述（留空或注释表示删除该翻译）");
					//writer.WriteLine("# 在翻译描述中，若要换行，请使用 '<LINE>' 换行");
					//writer.WriteLine("# 请不要在翻译使用字符 '|' ");
					//writer.WriteLine("# 编辑完后保存关闭，回到模组设置页点击“应用全部”");
					//writer.WriteLine("# 以 '#' 开头的行是注释，会被忽略");
					//writer.WriteLine();

					foreach (string id in sorted)
					{
						string origName, origDesc;

						if (installedMap.TryGetValue(id, out var mod))
						{
							origName = string.IsNullOrEmpty(mod.name) ? $" # {T("No_Name")}" : mod.name;
							origDesc = string.IsNullOrEmpty(mod.description) ? $" # {T("No_Desc")}" : mod.description.ReplaceLineEndings("<LINE>");
						}
						else
						{
							origName = $" # {T("Name_Unknown")}";
							origDesc = $" # {T("Desc_Unknown")}";
						}

						//string transName = inGameTranslator.shortStrings.TryGetValue(id + "-name", out var tn) ? tn : "# 请添加翻译名称";
						//string transDesc = inGameTranslator.shortStrings.TryGetValue(id + "-description", out var td) ? td : "# 请添加翻译描述";

						string nameKey = id + "-name";
						string descKey = id + "-description";
						string transName = keyIndexCache.TryGetValue(nameKey, out int nameIdx) 
							? allStrings[nameIdx].Split(new char[] { '|' }, 2)[1]
							: $" # {T("Trans_Name_Unknown")}";

						string transDesc = keyIndexCache.TryGetValue(descKey, out int descIdx)
							? allStrings[descIdx].Split(new char[] { '|' }, 2)[1]
							: $" # {T("Trans_Desc_Unknown")}";

						//int nameIndex = MyOptions.GetKeyInStrings(id + "-name");
						//int dicIndex = MyOptions.GetKeyInStrings(id + "-description");
						//string transName = (nameIndex != -1) ? GetStrings()[nameIndex].Split('|')[1] : $"# {T("Trans_Name_Unknown")}";
						//string transDesc = (dicIndex != -1) ? GetStrings()[dicIndex].Split('|')[1] : $"# {T("Trans_Desc_Unknown")}";

						transName = transName.ReplaceLineEndings("<LINE>").Trim();
						transDesc = transDesc.ReplaceLineEndings("<LINE>").Trim();

						writer.WriteLine($"[{id}]");
						writer.WriteLine($"name={origName}");
						writer.WriteLine($"desc={origDesc}");
						writer.WriteLine($"trans_name={transName}");
						writer.WriteLine($"trans_desc={transDesc}");
						writer.WriteLine();


						// 查找是否已安装
						/*ModManager.Mod? mod = ModManager.InstalledMods.FirstOrDefault(m => m.id == id);

						// 原始名称
						string origName = mod?.name ?? "# 模组未安装，原始名称未知";
						// 原始描述（含换行转义）
						string origDesc = (mod?.description ?? "# 模组未安装，原始描述未知").ReplaceLineEndings("<LINE>");

						if (origName == "") origName = "# 没有原始名称";
						if (origDesc == "") origDesc = "# 没有原始描述";

						// 从翻译器读取已有的翻译（若有）
						string transName = inGameTranslator.shortStrings.TryGetValue(id + "-name", out var tn) ? tn : "# 请添加翻译名称";
						string transDesc = inGameTranslator.shortStrings.TryGetValue(id + "-description", out var td) ? td : "# 请添加翻译描述";
						transDesc = transDesc.ReplaceLineEndings("<LINE>");

						writer.WriteLine($"|{id}");
						writer.WriteLine(origName);
						writer.WriteLine(origDesc);
						writer.WriteLine(transName);
						writer.WriteLine(transDesc);
						writer.WriteLine();*/
					}

					/*for (int i = 0; i < ModManager.InstalledMods.Count; i++)
					{
						ModManager.Mod m = ModManager.InstalledMods[i];

						string id = m.id;
						string origName = m.name ?? "# 没有原始名称或无法获取";
						string origDesc = m.description ?? "# 没有原始描述或无法获取";
						origDesc = origDesc.ReplaceLineEndings("<LINE>");

						// 从翻译器读取已有的翻译（若有）
						//int warning_dicExz = MyOptions.GetKeyInStrings(id + "-description");
						//int warning_nameExz = MyOptions.GetKeyInStrings(id + "-name");
						//string transName = (warning_nameExz != -1) ? GetStrings()[warning_nameExz].Split('|')[1] : "# 请添加翻译名称";
						//string transDesc = (warning_dicExz != -1) ? GetStrings()[warning_dicExz].Split('|')[1] : "# 请添加翻译描述";

						string transName = inGameTranslator.shortStrings.TryGetValue(id + "-name", out var tn) ? tn : "# 请添加翻译名称";
						string transDesc = inGameTranslator.shortStrings.TryGetValue(id + "-description", out var td) ? td : "# 请添加翻译描述";
						transDesc = transDesc.ReplaceLineEndings("<LINE>");

						writer.WriteLine($"|{id}");
						writer.WriteLine(origName);
						writer.WriteLine(origDesc);
						writer.WriteLine(transName);
						writer.WriteLine(transDesc);
						writer.WriteLine(); // 空行分隔
					}*/

				}


				// 文件关闭后，统一清洗一遍换行符
				//string content = File.ReadAllText(filePath);
				//File.WriteAllText(filePath, content.ReplaceLineEndings(Environment.NewLine));

				this.tipClearTimer = 120;
				this.tips!.text = T("TransFile_Success");

				// 打开文件
				OpenFileWithDefaultProgram(filePath);
			}
			catch (Exception ex)
			{
				Log.LogError($"导出全部翻译失败: {ex}");
				this.tipClearTimer = 120;
				this.tips!.text = T("TransFile_Failed");
			}
		}
		private void ImportAllButton_OnClick(UIfocusable trigger)
		{
			try
			{
				string filePath = GetAllMods();

				if (!File.Exists(filePath))
				{
					this.tipClearTimer = 120;
					this.tips!.text = T("TransFile_NotFound");
					return;
				}

				// 一次性构建键索引缓存
				string[] allStrings = GetStrings();
				Dictionary<string, int> keyIndexCache = new(StringComparer.Ordinal);
				for (int k = 0; k < allStrings.Length; k++)
				{
					string? key = Plugin.GetKey(allStrings[k]);
					if (key != null && !keyIndexCache.ContainsKey(key))
					{
						keyIndexCache[key] = k;
					}
				}


				string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

				List<(string id, string transName, string transDesc)> entries = new List<(string, string, string)>();

				int i = 0;
				int successCount = 0;
				int failCount = 0;
				int skipCount = 0;

				while (i < lines.Length)
				{
					string line = lines[i].Trim();
					// 跳过空行和注释
					if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
					{
						i++;
						continue;
					}

					// 必须是 '[' 开头的行，表示新模组开始
					if (!line.StartsWith("[") || !line.EndsWith("]"))
					{
						Log.LogWarning($"跳过非模组id行: {line}");
						i++;
						continue;
					}

					try
					{
						string id = line.Substring(1, line.Length - 2).Trim(); // 去掉 '[' ']'

						//string origName = lines[i + 1].Trim();
						//string origDesc = lines[i + 2].Trim();
						string transName = "";
						string transDesc = "";

						i++;
						while (i < lines.Length)
						{
							string kvLine = lines[i].Trim();
							// 遇到下一个节头则跳出，由外层重新处理
							if (kvLine.StartsWith("[") && kvLine.EndsWith("]"))
								break;

							if (string.IsNullOrEmpty(kvLine.Trim()) || kvLine.Trim().StartsWith("#"))
							{
								i++;
								continue;
							}
							int sep = kvLine.IndexOf('=');
							if (sep < 0)
							{
								i++;
								continue;
							}

							string key = kvLine.Substring(0, sep).Trim();
							string value = kvLine.Substring(sep + 1).Trim();

							string processed = StripComment(value);
							processed = processed.Replace(@"\n", "\n").ReplaceLineEndings("<LINE>").Trim();

							if (key == "trans_name")
								transName = processed;
							else if (key == "trans_desc")
								transDesc = processed;

							i++;
						}

						// 比较并决定是否加入
						string lastTransName = GetCurrentTranslation(id, "name", keyIndexCache, allStrings);
						string lastTransDesc = GetCurrentTranslation(id, "description", keyIndexCache, allStrings);


						// 变更检测的对比基准
						bool nameChanged = (transName != lastTransName);
						bool descChanged = (transDesc != lastTransDesc);
						if (nameChanged || descChanged)
						{
							successCount++;

							entries.Add((
								id,
								nameChanged ? transName : "--unchange--",
								descChanged ? transDesc : "--unchange--"
							));
							// 空值 注释 表示删除该条目的翻译；
							// "--unchange--" 表示该字段保持不变（跳过写入与内存更新）。
						}
						else
						{
							skipCount++;
						}
					}
					catch (Exception ex)
					{
						Log.LogWarning($"解析临时文件时发生异常: {ex}");
						failCount++;
						i++;
						continue;
					}

					//i++;
				}
				if (entries.Count == 0)
				{
					this.tipClearTimer = 120;
					this.tips!.text = skipCount > 0
						? T("No_New_TransLines", skipCount)
						: T("No_Valid_TransLines");
					return;
				}
				Log.LogInfo(T("Trans_Apply_Complete", successCount, failCount, skipCount));

				// 批量写入翻译
				int writeSuccess = 0, writeFail = 0, writeSkip = 0;

				// 批量写入翻译
				foreach (var entry in entries)
				{
					try
					{
						// 准备键值对
						List<string> keys = new List<string>();
						List<string> values = new List<string>();

						keys.Add(entry.id + "-name");
						values.Add(entry.transName);
						keys.Add(entry.id + "-description");
						values.Add(entry.transDesc);

						if (keys.Count > 0)
						{
							// 复用现有方法，逐个写入
							AddToStrings(entry.id, keys.ToArray(), values.ToArray());
							writeSuccess++;
						}
						else
						{
							writeSkip++;
						}
					}
					catch (Exception ex)
					{
						writeFail++;
						Log.LogError($"导入模组 {entry.id} 的翻译失败: {ex}");
						continue;
					}
				}

				// 刷新当前预览的模组信息（如果有）
				//if (Plugin.CurrentPreviewMod != null)
				//{
				//	string id = Plugin.CurrentPreviewMod.id;
				//	string newName = inGameTranslator.shortStrings.TryGetValue(id + "-name", out var name) ? name : Plugin.CurrentPreviewMod.name;
				//	string newDesc = inGameTranslator.shortStrings.TryGetValue(id + "-description", out var desc) ? desc : Plugin.CurrentPreviewMod.description;
				//	HotReplaceModInfo(id, newName, newDesc ?? "--unchange--");
				//}

				this.tipClearTimer = 120;
				this.tips!.text = T("Trans_Apply_Complete", writeSuccess, writeFail, writeSkip);
			}
			catch (Exception ex)
			{
				Log.LogError($"导入翻译失败: {ex}");
				this.tipClearTimer = 120;
				this.tips!.text = T("Trans_Apply_Failed");
			}
		}
		private string GetCurrentTranslation(string id, string type, Dictionary<string, int> cache, string[] allStrings)
		{
			string key = id + "-" + type;
			if (cache.TryGetValue(key, out int idx))
			{
				string full = allStrings[idx];
				int pipe = full.IndexOf('|');
				if (pipe >= 0)
					return full.Substring(pipe + 1).ReplaceLineEndings("<LINE>").Trim();
			}
			return "";
		}

		#endregion

		// 检查临时文件 ?
		public static bool CheckTempFile()
		{
			string[] strings = MyOptions.GetTemp();
			if (strings.Length == 0) return false;
			/* 修复 bug3：原判断只检查第一行，文件首行被改后可能误判；改为按 StripComment 结果判断

				strings[0] == Plugin.Menu_tip_tip1 || 
				strings[0] == Plugin.Menu_tip_tip2 || 
				strings[0] == Plugin.Menu_tip_tip3 ||
				strings[0] == Plugin.Menu_tip_nameW || 
				strings[0] == Plugin.Menu_tip_dicW;
			*/
			return MyOptions.StripComment(strings, false).Count > 0;
			// return !flag;
		}

		// 修复 bug2：检查临时文件是否有效，并且确实属于目标模组，防止把 A 的临时文件应用到 B
		public static bool CheckTempFileFor(string? id)
		{
			if (!CheckTempFile()) return false;
			if (id == null) return false;
			return string.Equals(TempFileModId, id, StringComparison.Ordinal);
		}

		public static List<string> StripComment(IEnumerable<string> strings, bool containsEmptyLines)
		{
			var result = new List<string>();

			foreach (string rawLine in strings)
			{
				// 遇到注释标记行，停止处理所有后续行
				if (rawLine.StartsWith("# ===Comments==="))
					break;

				string line = StripComment(rawLine);

				// 根据条件添加
				if (!string.IsNullOrWhiteSpace(line) || containsEmptyLines)
					result.Add(line);
			}

			return result;
		}
		public static string StripComment(string rawLine)
		{
			if (rawLine.StartsWith("# ===Comments==="))
				return string.Empty;

			// 处理行内注释
			int hashIdx = rawLine.IndexOf('#');
			int slashIdx = rawLine.IndexOf("||");
			int idx = (slashIdx, hashIdx) switch
			{
				( >= 0, >= 0) => Math.Min(slashIdx, hashIdx),
				( >= 0, _) => slashIdx,
				(_, >= 0) => hashIdx,
				_ => -1
			};

			return idx >= 0 ? rawLine.Substring(0, idx).TrimEnd().TrimStart() : rawLine;
		}
		public static void RemoveKeysFromFile(string[] keys)
		{
			if (keys == null || keys.Length == 0) return;

			HashSet<string> keySet = new(keys);
			List<string> stringList = new(GetStrings());

			bool changed = false;
			for (int i = stringList.Count - 1; i >= 0; i--)
			{
				string line = stringList[i];
				if (string.IsNullOrEmpty(line) || !line.Contains('|')) continue;

				string[] parts = line.Split(new[] { '|' }, 2);
				if (parts.Length == 2 && keySet.Contains(parts[0]))
				{
					stringList.RemoveAt(i);
					changed = true;
				}
			}

			if (changed)
			{
				WriteIn(stringList.ToArray());
			}
		}

		// 应用到文件和翻译字典
		public static void AddToStrings(string? id)
		{
			if (id == null)
			{
				Log.LogWarning("AddToStrings: id is null, skipping.");
				return;
			}

			List<string> strings = MyOptions.StripComment(GetTemp(), true);

			// 修复 bug3：文件只有注释/空白时，StripComment 后为空，避免 strings[0] 越界
			if (strings.Count == 0)
			{
				Log.LogWarning("AddToStrings: 临时文件没有有效内容，跳过。");
				return;
			}


			string name = strings[0];
			string dic = "";

			for (int i = 1; i < strings.Count; i++)
			{
				dic += strings[i];

				if (i != strings.Count - 1)
				{
					dic += "<LINE>";
				}
			}

			while (dic.EndsWith("<LINE>"))
				dic = dic.Substring(0, dic.Length - "<LINE>".Length).TrimEnd();

			string[] keys = new string[]
			{
					id + "-name",
					id + "-description"
			};
			string[] values = new string[]
			{
					name.Trim(),
					dic.Replace(@"\n", "\n").ReplaceLineEndings("<LINE>").Trim()
			};

			AddToStrings(id, keys, values);
		}
		public static void AddToStrings(string? id, string[] keys, string[] values)
		{
			if (id == null)
			{
				Log.LogWarning("AddToStrings: id is null, skipping.");
				return;
			}
			if (keys == null || values == null)
				throw new ArgumentNullException(keys == null ? nameof(keys) : nameof(values));
			if (keys.Length != values.Length)
				throw new ArgumentException($"keys({keys.Length}) and values({values.Length}) length mismatch.");
			if (keys.Length == 0)
				return;

			MyOptions.SetKeyValuesToFile(keys, values);
			MyOptions.SetToShortStrings(keys, values);
			MyOptions.HotReplaceModInfo(id, values);

			MyOptions.ClearTempFile();
		}
		// 将键值对写入文件
		public static void SetKeyValuesToFile(string[] keys, string[] values)
		{
			if (keys == null || values == null)
				throw new ArgumentNullException(keys == null ? nameof(keys) : nameof(values));
			if (keys.Length != values.Length)
				throw new ArgumentException($"keys({keys.Length}) and values({values.Length}) length mismatch.");
			if (keys.Length == 0)
				return;

			List<string> stringList = new List<string>(MyOptions.GetStrings());

			for (int i = 0; i < keys.Length; i++)
			{
				if (string.IsNullOrWhiteSpace(keys[i]))
				{
					continue;
				}
				if (values[i] == "--unchange--")
				{
					continue;
				}

				// 在本地列表上定位，保证增/改/删操作基于同一份数据
				int index = GetKeyIndexInList(stringList, keys[i]);

				if (string.IsNullOrWhiteSpace(values[i]))
				{
					// 空值：删除该条目
					if (index != -1)
					{
						stringList.RemoveAt(index);
					}
					continue;
				}

				string entry = keys[i] + "|" + values[i];

				if (index != -1)
				{
					stringList[index] = entry; // 更新已有条目
				}
				else
				{
					if (i == 0)
					{
						stringList.Add(String.Empty);
					}
					stringList.Add(entry);     // 新增条目
				}
			}

			MyOptions.WriteIn(stringList.ToArray());
		}
		// 在给定的行列表中查找键
		private static int GetKeyIndexInList(List<string> stringList, string key)
		{
			for (int i = 0; i < stringList.Count; i++)
			{
				string line = stringList[i];
				if (!line.Contains('|')) continue;

				string[] keyAndValue = line.Split(new char[] { '|' }, 2);
				if (keyAndValue.Length == 2 && keyAndValue[0] == key)
				{
					return i;
				}
			}
			return -1;
		}
		// 写入文件
		public static void WriteIn(string[] strings)
		{
			//string path = "text/text_" + LocalizationTranslator.LangShort(inGameTranslator.currentLanguage) + "/strings.txt";
			//string fullPath = MyOptions.ResolveFilePathFromMod(path, "Lvye_AnotherNameForMods", false);
			string path = GetStringsPath();
			File.WriteAllLines(path, strings);

			string savePath = GetSavePath();
			BackupFile(savePath);

			File.WriteAllLines(savePath, strings);
			// 只把模组翻译键（ModID-name / ModID-description）写入存档；UI 译文不写入存档，避免切换语言后覆盖当前语言 UI
			//string[] modStrings = strings.Where(line =>
			//{
			//	if (string.IsNullOrEmpty(line) || !line.Contains('|')) return false;
			//	string[] parts = line.Split(new char[] { '|' }, 2);
			//	return parts.Length == 2 && (parts[0].EndsWith("-name") || parts[0].EndsWith("-description"));
			//}).ToArray();
			//File.WriteAllLines(savePath, modStrings);
		}
		// 将键值对写入短字符串字典
		public static void SetToShortStrings(string[] keys, string[] values)
		{
			for (int i = 0; i < keys.Length; i++)
			{
				if (values[i] != "--unchange--")
				{
					if (inGameTranslator.shortStrings.ContainsKey(keys[i]))
					{
						inGameTranslator.shortStrings.Remove(keys[i]);
					}
					if (!string.IsNullOrWhiteSpace(values[i]))
					{
						inGameTranslator.shortStrings.Add(keys[i], values[i]);
					}
				}
			}
		}
		// 热更新模组信息
		public static void HotReplaceModInfo(string modID, string[] values)
		{
			string name = values.Length > 0 ? values[0] : "";
			string desc = values.Length > 1 ? values[1] : "";
			HotReplaceModInfo(modID, name, desc);
		}
		public static void HotReplaceModInfo(string modID, string modName, string moddic = "--unchange--")
		{
			bool found = false;

			// 更新当前可见的 UI 元素
			foreach (UIelement uIelement in ConfigContainer.instance.GetFocusables())
			{
				if (uIelement is MenuModList.ModButton modButton)
				{
					if (modButton.ModID == modID)
					{
						UpdateButton(modButton, modID, modName, moddic);
						found = true;
						break;
					}
				}
			}

			// 若可见元素中未找到，从模组列表中直接查找
			Log.LogDebug($"found:{found}");
			if (!found)
			{
				var modList = ConfigContainer.menuTab?.modList;
				if (modList != null)
				{
					// 使用已知的 GetModButton 方法（参考 GrabLastActiveModButton 中的用法）
					MenuModList.ModButton? button = modList.GetModButton(modID);
					if (button != null)
					{
						if (button.ModID == modID)
						{
							UpdateButton(button, modID, modName, moddic);
							found = true;
						}
					}
				}
			}

			// 若仍未找到（例如模组未在列表中，或列表尚未构建），记录日志
			if (!found)
			{
				Log.LogWarning($"HotReplaceModInfo: 未找到模组 '{modID}' 的按钮，可能该模组未安装或列表未加载。");
			}


			//foreach (UIelement uIelement in ConfigContainer.instance.GetFocusables())
			//{
			//	if (uIelement is MenuModList.ModButton modButton)
			//	{
			//		if (modButton.ModID == modID)
			//		{
			//			if (modName != "--unchange--")
			//			{
			//				if (!string.IsNullOrWhiteSpace(modName))
			//				{
			//					modButton.text = modName;
			//				}
			//				else
			//				{
			//					if (installedMap.TryGetValue(modID, out var mod))
			//					{
			//						modButton.text = mod.name;
			//					}
			//					else
			//					{
			//						Log.LogWarning($"Mod with ID '{modID}' not found in installed mods. '模组未安装'.");
			//						modButton.text = T("Mod_Not_Installed") + "modButton.text";
			//					}
			//				}
			//			}

			//			if (ConfigContainer.OptItfs[0] is InternalOI_Stats internalOI_Stats)
			//			{
			//				if (modName != "--unchange--")
			//				{
			//					if (!string.IsNullOrWhiteSpace(modName))
			//					{
			//						internalOI_Stats.lblName.text = modName;
			//					}
			//					else
			//					{
			//						if (installedMap.TryGetValue(modID, out var mod))
			//						{
			//							internalOI_Stats.lblName.text = mod.name;
			//						}
			//						else
			//						{
			//							Log.LogWarning($"Mod with ID '{modID}' not found in installed mods. '模组未安装'.");
			//							internalOI_Stats.lblName.text = T("Mod_Not_Installed") + "lblName.text";
			//						}
			//					}
			//				}

			//				if (moddic != "--unchange--")
			//				{
			//					if (!string.IsNullOrWhiteSpace(moddic))
			//					{
			//						internalOI_Stats.lblDescription.text = moddic;
			//					}
			//					else
			//					{
			//						if (installedMap.TryGetValue(modID, out var mod))
			//						{
			//							internalOI_Stats.lblDescription.text = mod.description;
			//						}
			//						else
			//						{
			//							Log.LogWarning($"Mod with ID '{modID}' not found in installed mods. '模组未安装'.");
			//							internalOI_Stats.lblDescription.text = T("Mod_Not_Installed") + "lblDescription.text";
			//						}
			//					}
			//				}
			//			}
			//			else
			//			{
			//				Log.LogWarning("ConfigContainer.OptItfs[0] is not InternalOI_Stats");
			//			}
			//		}
			//	}
			//}
		}
		// 更新单个按钮的显示
		private static void UpdateButton(MenuModList.ModButton button, string modID, string modName, string moddic)
		{
			if (modName != "--unchange--")
			{
				if (!string.IsNullOrWhiteSpace(modName))
				{
					button.text = modName;
				}
				else
				{
					if (installedMap.TryGetValue(modID, out var mod))
					{
						button.text = mod.name;
					}
					else
					{
						Log.LogWarning($"Mod with ID '{modID}' not found in installed mods. '模组未安装'.");
						button.text = T("Mod_Not_Installed") + "modButton.text";
					}
				}
			}

			// 同时更新预览面板中的标签（如果当前预览的是该模组）
			if (ConfigContainer.OptItfs[0] is InternalOI_Stats internalOI_Stats)
			{
				if (modName != "--unchange--")
				{
					if (!string.IsNullOrWhiteSpace(modName))
					{
						internalOI_Stats.lblName.text = modName;
					}
					else
					{
						if (installedMap.TryGetValue(modID, out var mod))
						{
							internalOI_Stats.lblName.text = mod.name;
						}
						else
						{
							Log.LogWarning($"Mod with ID '{modID}' not found in installed mods. '模组未安装'.");
							internalOI_Stats.lblName.text = T("Mod_Not_Installed") + "lblName.text";
						}
					}
				}

				if (moddic != "--unchange--")
				{
					if (!string.IsNullOrWhiteSpace(moddic))
					{
						internalOI_Stats.lblDescription.text = moddic;
					}
					else
					{
						if (installedMap.TryGetValue(modID, out var mod))
						{
							internalOI_Stats.lblDescription.text = mod.description;
						}
						else
						{
							Log.LogWarning($"Mod with ID '{modID}' not found in installed mods. '模组未安装'.");
							internalOI_Stats.lblDescription.text = T("Mod_Not_Installed") + "lblDescription.text";
						}
					}
				}

				// 如果当前预览的模组就是被更新的模组，刷新预览面板以重新布局
				//if (needRefresh)
				//{
				//	internalOI_Stats._RefreshStats();
				//}

				internalOI_Stats._PreviewMod(button);
			}
		}
		public static bool IsKeyInStrings(string key)
		{
			return GetKeyInStrings(key) != -1;
		}
		public static int GetKeyInStrings(string key)
		{
			string[] strings = MyOptions.GetStrings();
			for (int i = 0; i < strings.Length; i++)
			{
				string line = strings[i];

				if (!line.Contains('|')) continue;

				string[] keyAndValue = line.Split(new char[] { '|' }, 2);
				if (keyAndValue.Length != 2) continue;

				string lineKey = keyAndValue[0];
				if (lineKey == key)
				{
					return i;
				}
			}
			return -1;
		}

		// 打开临时文件
		public static void OpenTempFile(string? id)
		{
			if (id == null)
			{
				Log.LogWarning("OpenTempFile: id is null, skipping.");
				TempFileModId = null;

				return;
			}

			// 这个键在字符串中吗
			int warning_dicExz = MyOptions.GetKeyInStrings(id + "-description");
			int warning_nameExz = MyOptions.GetKeyInStrings(id + "-name");

			// 拼接目标文件路径
			string path = GetTempPath();
			//string path = "text/text_" + LocalizationTranslator.LangShort(MyOptions.rainWorld.inGameTranslator.currentLanguage) + "/temp.txt";
			//string fullPath = AssetManager.ResolveFilePath(stringsPath);

			ModManager.Mod? mod = null;
			for (int i = 0; i < ModManager.InstalledMods.Count; i++)
			{
				ModManager.Mod m = ModManager.InstalledMods[i];
				if (m.id == id)
				{
					mod = m;
					break;
				}
			}

			if (warning_nameExz != -1)
			{
				File.WriteAllText(path, GetStrings()[warning_nameExz].Split('|')[1]);
			}
			else if (mod != null)
			{
				File.WriteAllText(path, string.IsNullOrEmpty(mod.name) ? $"# {T("No_Name")}" : mod.name);
			}
			else
			{
				File.WriteAllText(path, $"# {T("Name_Unknown")}");
			}

			if (warning_dicExz != -1)
			{
				File.AppendAllText(path, $"\n{GetStrings()[warning_dicExz].Split('|')[1].ReplaceLineEndings("<LINE>")}");
			}
			else if (mod != null)
			{
				string desc = string.IsNullOrEmpty(mod.description) ? $"# {T("No_Desc")}" : mod.description.ReplaceLineEndings("<LINE>");
				File.AppendAllText(path, $"\n{desc}");
			}
			else
			{
				File.AppendAllText(path, $"\n# {T("Desc_Unknown")}");
			}

			File.AppendAllText(path, $"\n# ===Comments==={T("Comments")}===");
			if (Instance?.enabletur.Value == true)
			{
				File.AppendAllText(path, $"\n# {T("TempFile_Guide_1")}");
				File.AppendAllText(path, $"\n# {T("TempFile_Guide_2")}");
				File.AppendAllText(path, $"\n# {T("TempFile_Guide_3")}");
				File.AppendAllText(path, $"\n# {T("TempFile_Guide_4")}");
				File.AppendAllText(path, $"\n");

				//File.AppendAllText(path, $"\n\n# 此文件中写入你想替换的文本，第一行为名称，其他行将作为描述，或者在同一行内使用<LINE>换行；某一行留空表示删除该行的翻译并回退为原文，完成后请保存关闭此文件，点击确认替换。");
				//File.AppendAllText(path, $"\n# 以 '#' 开头的行是注释，会被忽略");
				//File.AppendAllText(path, $"\n# 你可以通过关闭此模组设置中的文件使用提示选项来使下次此文件中不会出现这三行话。");
				//File.AppendAllText(path, $"\n");
			}

			if (warning_nameExz != -1)
			{
				File.AppendAllText(path, $"\n# {T("TempFile_Warning_Name")}");
				//File.AppendAllText(path, $"\n# 警告：已有为此模组设置的名称存在！确认替换将会覆盖上次修改。");
			}
			if (warning_dicExz != -1)
			{
				File.AppendAllText(path, $"\n# {T("TempFile_Warning_Desc")}");
				//File.AppendAllText(path, $"\n# 警告：已有为此模组设置的简介存在！确认替换将会覆盖上次修改。");
			}

			// 修复 bug2：记录该临时文件属于哪个模组
			TempFileModId = id;

			MyOptions.OpenFileWithDefaultProgram(path);
		}

		// 用默认程序打开文件
		public static void OpenFileWithDefaultProgram(string filePath)
		{
			try
			{
				Process.Start(new ProcessStartInfo
				{
					FileName = filePath,       // txt 文件的完整路径
					UseShellExecute = true        // 为 true 才会调用系统默认关联程序
				});
			}
			catch (Exception ex)
			{
				Log.LogWarning("尝试用默认程序打开文件失败: " + ex.Message);
				try
				{
					ProcessStartInfo startInfo = new ProcessStartInfo
					{
						FileName = "notepad.exe",
						Arguments = "\"" + filePath + "\"",
						UseShellExecute = false,
						WindowStyle = ProcessWindowStyle.Normal
					};
					Process process = Process.Start(startInfo);
					if (process != null)
					{
						process.WaitForInputIdle(1000);
					}
				}
				catch (Exception innerEx)
				{
					Log.LogError("打开文件失败: " + innerEx.Message);
				}
			}
		}


		/// <summary>
		/// 从指定模组目录中解析文件路径!!!
		/// </summary>
		/*public static string ResolveFilePathFromMod(string path, string modid, bool skipMergedMods = false)
		{
			path = path.Replace('/', Path.DirectorySeparatorChar);

			if (!skipMergedMods)
			{
				string mergedPath = Path.Combine(Path.Combine(Custom.RootFolderDirectory(), "mergedmods"), path.ToLowerInvariant());
				if (File.Exists(mergedPath))
				{
					return mergedPath;
				}
			}
			ModManager.Mod targetMod = ModManager.ActiveMods.FirstOrDefault<ModManager.Mod>((ModManager.Mod m) => string.Equals(m.id, modid, StringComparison.OrdinalIgnoreCase));

			if (targetMod != null)
			{
				if (targetMod.hasTargetedVersionFolder)
				{
					string path2 = Path.Combine(targetMod.TargetedPath, path.ToLowerInvariant());

					if (File.Exists(path2))
					{
						return path2;
					}
				}
				if (targetMod.hasNewestFolder)
				{
					string path3 = Path.Combine(targetMod.NewestPath, path.ToLowerInvariant());
					if (File.Exists(path3))
					{
						return path3;
					}
				}
				string path4 = Path.Combine(targetMod.path, path.ToLowerInvariant());
				if (File.Exists(path4))
				{
					return path4;
				}
			}
			return Path.Combine(Custom.RootFolderDirectory(), path.ToLowerInvariant());
		}*/

		#region //
		//public static Dictionary<string, string> CoverStrings2Dic(IEnumerable<string> strings)
		//{
		//	Dictionary<string, string> dic = new Dictionary<string, string>();
		//	foreach (string s in strings)
		//	{
		//		string[] keyAndValue = s.Split(new char[] { '|' });

		//		if (keyAndValue.Length == 2)
		//		{
		//			dic.Add(keyAndValue[0], keyAndValue[1]);
		//		}
		//	}
		//	return dic;
		//}

		//public static string[] CoverDic2Strings(Dictionary<string, string> dic)
		//{
		//	List<string> strings = new List<string>();

		//	foreach (KeyValuePair<string, string> keyAndValue in dic)
		//	{
		//		strings.Add(keyAndValue.Key + "|" + keyAndValue.Value);
		//	}
		//	return strings.ToArray();
		//}

		//public static Dictionary<string, string> GetStringsDictionary()
		//{
		//	return MyOptions.CoverStrings2Dic(MyOptions.GetStrings());
		//}


		//public static void WriteIn(Dictionary<string, string> dic)
		//{
		//	string[] strings = MyOptions.CoverDic2Strings(dic);
		//	WriteIn(strings);
		//}

		//public static void AddKeyValueToFile(string[] keys, string[] values)
		//{
		//	Dictionary<string, string> dic = MyOptions.CoverStrings2Dic(MyOptions.GetStrings());
		//	int i = 0;
		//	while (i < keys.Length && i < values.Length)
		//	{
		//		string key = keys[i];
		//		string value = values[i];

		//		if (dic.ContainsKey(key))
		//		{
		//			dic.Remove(key);
		//		}
		//		dic.Add(key, value);
		//		i++;
		//	}
		//	MyOptions.WriteIn(dic);
		//}
		#endregion

	}
}
