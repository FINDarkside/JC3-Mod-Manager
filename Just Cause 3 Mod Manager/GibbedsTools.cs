﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Just_Cause_3_Mod_Manager
{
	public class GibbedsTools
	{

		public enum ConvertMode
		{
			Import,
			Export,
			Unknown
		}

		public static string convertProperty = Path.Combine(Settings.gibbedsTools, "Gibbed.JustCause3.ConvertProperty.exe");
		public static string smallPack = Path.Combine(Settings.gibbedsTools, "Gibbed.JustCause3.SmallPack.exe");
		public static string smallUnpack = Path.Combine(Settings.gibbedsTools, "Gibbed.JustCause3.SmallUnpack.exe");
		public static string convertAdf = Path.Combine(Settings.gibbedsTools, "Gibbed.JustCause3.ConvertAdf.exe");
		public static string unpack = Path.Combine(Settings.gibbedsTools, "Gibbed.JustCause3.Unpack.exe");

		public static string ConvertProperty(string inputPath)
		{
			string outputPath = Path.ChangeExtension(inputPath, inputPath.EndsWith(".xml") ? ".bin" : ".xml");
			return ConvertProperty(inputPath, outputPath, ConvertMode.Unknown);
		}

		public static string ConvertProperty(string inputPath, string outputPath, ConvertMode mode)
		{
			string extraArg = mode != ConvertMode.Unknown ? (mode == ConvertMode.Import ? "--i" : "--e") : "";
			Run(convertProperty, "\"" + inputPath + "\" \"" + outputPath + "\" " + extraArg);

			if (File.Exists(outputPath))
				return outputPath;
			return null;
		}

		public static string SmallUnpack(string inputPath)
		{
			string outputPath = Path.GetFileNameWithoutExtension(inputPath);
			return SmallUnpack(inputPath, outputPath);
		}

		public static string SmallUnpack(string inputPath, string outputPath)
		{
			Run(smallUnpack, "\"" + inputPath + "\" \"" + outputPath + "\"");

			if (Directory.Exists(outputPath))
				return outputPath;
			return null;
		}

		public static string SmallPack(string inputPath)
		{
			string outputPath = inputPath + ".ee";
			return SmallPack(inputPath, outputPath);
		}

		public static string SmallPack(string inputPath, string outputPath)
		{
			Run(smallPack, "\"" + inputPath + "\" \"" + outputPath + "\"");

			if (File.Exists(outputPath))
				return outputPath;
			return null;
		}

		public static string ConvertAdf(string inputPath)
		{
			string outputPath = Path.ChangeExtension(inputPath, ".xml");
			return ConvertAdf(inputPath, outputPath);
		}

		public static string ConvertAdf(string inputPath, string outputPath)
		{
			Run(convertAdf, "\"" + inputPath + "\" \"" + outputPath + "\" --e");

			if (File.Exists(outputPath))
				return outputPath;
			return null;
		}

		public static string Unpack(string inputPath)
		{
			string outputPath = Path.GetFileNameWithoutExtension(inputPath);
			return Unpack(inputPath, outputPath, null);
		}

		public static string Unpack(string inputPath, string outputPath, string regex)
		{
			Run(unpack, "\"" + inputPath + "\" \"" + outputPath + "\"" + (regex != null ? " -f:" + regex : ""));

			if (Directory.Exists(outputPath))
				return outputPath;
			return null;
		}

		private static void Run(string fileName, string args)
		{
			var proc = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = fileName,
					Arguments = args,
					UseShellExecute = false,
					CreateNoWindow = true
				}
			};

			proc.Start();
			proc.WaitForExit();
			if (proc.ExitCode != 0)
			{
				throw new Exception(fileName + " crashed with exit code " + proc.ExitCode);
			}
		}

		public static bool CanConvert(string inputPath, string exe)
		{
			var outputPath = TempFolder.GetTempFile();
			Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
			var proc = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = exe,
					Arguments = "\"" + inputPath + "\" \"" + outputPath + "\"",
					UseShellExecute = false,
					CreateNoWindow = true
				}
			};
			proc.Start();
			proc.WaitForExit();

			if (File.Exists(outputPath))
				File.Delete(outputPath);
			if (Directory.Exists(outputPath))
				Directory.Delete(outputPath, true);


			return proc.ExitCode == 0;
		}

	}
}
