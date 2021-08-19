using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Robusta.Editor.Utils
{
	[InitializeOnLoad]
	public class StartupFixMacOsM1
	{
		class SdkSettings
		{
			public string PathInUnity;
			public string[] Binary;
			public string EditorPrefsKey;
		}

		static StartupFixMacOsM1()
		{
#if UNITY_EDITOR_OSX
			var sdks = new Dictionary<string, SdkSettings>()
			{
				{
					"JAVA_HOME",
					new SdkSettings
					{
						PathInUnity = "PlaybackEngines/AndroidPlayer/OpenJDK/",
						Binary = new string[] { "bin", "java" }
					}
				},
				{
					"ANDROID_SDK_HOME",
					new SdkSettings
					{
						PathInUnity = "PlaybackEngines/AndroidPlayer/SDK",
						Binary = new string[] { "platform-tools", "adb" },
						EditorPrefsKey = "AndroidSdkRoot"
					}
				}
			};


			Debug.Log("Up and running");
			// hack for m1 mac builds, since it tends to loose JAVA_HOME for some reason

			foreach (var sdk in sdks)
			{
				var sdkHome = Environment.GetEnvironmentVariable(sdk.Key);
				if (String.IsNullOrEmpty(sdkHome) || !Directory.Exists(sdkHome))
				{
					sdkHome = EditorApplication.applicationContentsPath.Replace("Unity.app/Contents",
						sdk.Value.PathInUnity);
					// check if we have SDK exisits
					string checkPath = Path.Combine(sdkHome, Path.Combine(sdk.Value.Binary));
					if (File.Exists(checkPath))
					{
						Environment.SetEnvironmentVariable(sdk.Key, sdkHome);
						if (!String.IsNullOrEmpty(sdk.Value.EditorPrefsKey))
						{
							EditorPrefs.SetString(sdk.Value.EditorPrefsKey, sdkHome);
						}
					}
					else
					{
						Debug.LogError(
							"Please install Android SDK & NDK tools with UnityHub https://docs.unity3d.com/Manual/android-sdksetup.html");
					}
				}
			}
#endif
		}
	}
}