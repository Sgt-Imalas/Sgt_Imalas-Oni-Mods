//using HarmonyLib;
//using Klei;
//using PeterHan.PLib.Core;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection.Emit;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace UtilLibs.Updating
//{
//	public static class LoadExtraAnimations
//	{
//		static string AnimationPath = string.Empty;
//		public static void Initialize(Harmony harmony, string path)
//		{
//			AnimationPath = path;
//			try
//			{
//				var targetMethod = AccessTools.Method(typeof(KMod.Manager), nameof(KMod.Manager.Load));
//				var postfix = AccessTools.Method(typeof(LoadExtraAnimations), nameof(LoadModAnimations));
//				harmony.Patch(targetMethod, postfix: new(postfix));
//			}
//			catch { }
//		}
//		private static void LoadModAnimations(KMod.Content content)
//		{
//			if (content != KMod.Content.Animation)
//				return;

//			string path = FileSystem.Normalize(System.IO.Path.Combine(AnimationPath, "anim"));
//			if (!System.IO.Directory.Exists(path))
//				return;
//			int num = 0;
//			foreach (DirectoryInfo directory1 in new DirectoryInfo(path).GetDirectories())
//			{
//				foreach (DirectoryInfo directory2 in directory1.GetDirectories())
//				{
//					KAnimFile.Mod anim_mod = new KAnimFile.Mod();
//					foreach (FileInfo file in directory2.GetFiles())
//					{
//						if (!file.Name.StartsWith("._"))
//						{
//							if (file.Extension == ".png")
//							{
//								byte[] data = File.ReadAllBytes(file.FullName);
//								Texture2D tex = new Texture2D(2, 2);
//								tex.LoadImage(data);
//								anim_mod.textures.Add(tex);
//							}
//							else if (file.Extension == ".bytes")
//							{
//								string withoutExtension = System.IO.Path.GetFileNameWithoutExtension(file.Name);
//								byte[] numArray = File.ReadAllBytes(file.FullName);
//								if (withoutExtension.EndsWith("_anim"))
//									anim_mod.anim = numArray;
//								else if (withoutExtension.EndsWith("_build"))
//									anim_mod.build = numArray;
//								else
//									DebugUtil.LogWarningArgs((object)$"Unhandled TextAsset ({file.FullName})...ignoring");
//							}
//							else
//								DebugUtil.LogWarningArgs((object)$"Unhandled asset ({file.FullName})...ignoring");
//						}
//					}
//					string name = directory2.Name + "_kanim";
//					if (anim_mod.IsValid() && ModUtil.AddKAnimMod(name, anim_mod))
//						++num;
//				}
//			}
//			SgtLogger.l($"Loaded {num} animations");
//		}
//	}

//}
