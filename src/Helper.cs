#region using
using BepInEx;
using CommonUtils;
using CommonUtils.Core;
using CoralBrain;
using Expedition;
using Fisobs.Core;
using HUD;
using ImprovedInput;
using JollyCoop;
using JollyCoop.JollyMenu;
using Menu;
using Menu.Remix.MixedUI;
using MonoMod.RuntimeDetour;
using MoreSlugcats;
using Newtonsoft.Json;
using Noise;
using RWCustom;
using SlugBase;
using SlugBase.DataTypes;
using SlugBase.Features;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Watcher;
using static Player.ObjectGrabability;
using static SlugBase.Features.FeatureTypes;
#endregion

namespace Translator
{
	internal static class Helper
	{

		#region UITranslate
		public static RainWorld RainWorld => Custom.rainWorld;
		public static InGameTranslator inGameTranslator => RainWorld.inGameTranslator;
		public static InGameTranslator Translator => inGameTranslator;
		public static InGameTranslator Trans => inGameTranslator;


		public static string? currentLang;
		private static Dictionary<string, string> _dict = [];
		private static Dictionary<string, string> Dict
		{
			get
			{
				if (currentLang != LocalizationTranslator.LangShort(Translator.currentLanguage))
				{
					currentLang = LocalizationTranslator.LangShort(Translator.currentLanguage);

					string path = MyOptions.GetTranslatorPath();
					if (File.Exists(path))
					{
						_dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path)) ?? [];
					}
					else
					{
						Log.LogError("找不到语言文件: " + currentLang);

						path = MyOptions.GetTranslatorPath("eng");
						if (File.Exists(path))
						{
							_dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path)) ?? [];
						}
						else
						{
							Log.LogError("找不到默认语言文件: eng");
							_dict = [];
						}
					}
					return _dict;
				}
				else
				{
					return _dict;
				}
			}
		}


		extension(string key)
		{
			public string Translation => Dict.TryGetValue(key, out var val) ? val : key;
		}
		public static string Translation(string key, params object[] args) => string.Format(key.Translation, args);
		#endregion

	}
}
