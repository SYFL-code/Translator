#region using
using BepInEx;
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
using Noise;
using RWCustom;
using SlugBase;
using SlugBase.Features;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
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

public static class Helper
{

	#region Translate
	public static InGameTranslator Translator => Custom.rainWorld.inGameTranslator;

	public static string Tra(this string originalName)
	{
		return Translator.Translate(originalName);
	}
	public static string Translate(this string originalName)
	{
		return Translator.Translate(originalName);
	}
	public static string[] Translate(this IEnumerable<string> originalNames)
	{
		List<string> items = new();
		foreach (string name in originalNames)
		{
			items.Add(Translator.Translate(name));
		}
		return items.ToArray();
	}
	public static ListItem[] ToListItem(this IEnumerable<string> originalNames)
	{
		List<ListItem> items = new();

		int i = 0;
		foreach (string name in originalNames)
		{
			// name 作为实际值，displayName 使用翻译后的文本
			items.Add(new ListItem(name, Translator.Translate(name), i));
			i += 1;
		}
		return items.ToArray();
	}
    #endregion

    #region String
    public static string ReplaceLineEndings(this string s, string lineEndings = "\r\n")
	{
		return s.Replace("\r\n", "\n")
				.Replace("\r", "\n")
				.Replace("\n", lineEndings);
	}
    #endregion
}
