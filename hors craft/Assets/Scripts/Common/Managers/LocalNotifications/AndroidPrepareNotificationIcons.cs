// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.LocalNotifications.AndroidPrepareNotificationIcons
using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Common.Managers.LocalNotifications
{
	public static class AndroidPrepareNotificationIcons
	{
		private class DrawableInfo
		{
			public string dirName;

			public int size;

			public DrawableInfo(string dirName, int size)
			{
				this.dirName = dirName;
				this.size = size;
			}
		}

		public static string commonCodePath;

		private static DrawableInfo[] drawableInfos = new DrawableInfo[6]
		{
			new DrawableInfo("drawable", 32),
			new DrawableInfo("drawable-hdpi", 32),
			new DrawableInfo("drawable-mdpi", 32),
			new DrawableInfo("drawable-xhdpi", 64),
			new DrawableInfo("drawable-xxhdpi", 64),
			new DrawableInfo("drawable-xxxhdpi", 128)
		};

		public static void RegenerateAndroidResPushIcons(string relativePathToMainIcon)
		{
		}

		private static void DisplayScalingProgressBar(float progress, string currentPath)
		{
		}

		private static void ScaleImage(string input, int w, int h, string output, bool optional = true, string customImgMagickCommand = "-gravity center")
		{
			if (File.Exists(input))
			{
				string text = "\"" + input + "\" -filter Lanczos -geometry " + w + "x" + h;
				string text2 = " \"" + output + "\"";
				ConvertWithImageMagick(text + " " + customImgMagickCommand + " -background \"#00000000\" -extent " + w + "x" + h + text2);
			}
			else if (!optional)
			{
				throw new Exception("Cannot find input file for scaling: " + input + ", current dir now is " + Directory.GetCurrentDirectory());
			}
		}

		private static void ConvertWithImageMagick(string arguments)
		{
			UnityEngine.Debug.Log("Calling ImageMagick with arguments: " + arguments);
			Process process = new Process();
			process.StartInfo.FileName = GetImageMagickConvertExecutablePath();
			process.StartInfo.Arguments = arguments;
			process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.Start();
			if (!process.WaitForExit(20000))
			{
				throw new Exception("Process failed to exit after " + 20000 + " miliseconds. Giving up.");
			}
			if (process.ExitCode != 0)
			{
				UnityEngine.Debug.LogWarning("ImageMagick convert failed, exit code " + process.ExitCode + ", output: " + process.StandardOutput.ReadToEnd() + ", error output: " + process.StandardError.ReadToEnd());
			}
		}

		public static string GetImageMagickConvertExecutablePath()
		{
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				return Environment.CurrentDirectory + commonCodePath + "Editor/ImageMagick/convert.exe";
			}
			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				return "/opt/ImageMagick/bin/convert";
			}
			throw new Exception("Wait, what?");
		}
	}
}
