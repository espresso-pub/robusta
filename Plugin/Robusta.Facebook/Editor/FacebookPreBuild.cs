using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Facebook.Unity.Settings;
using Robusta.Editor.Android;
using UnityEditor;


namespace Robusta.Editor.Facebook
{
	public class FacebookPreBuild : IPreprocessBuildWithReport
	{
		public int callbackOrder => 1;

		public void OnPreprocessBuild(BuildReport report)
		{
#if UNITY_ANDROID || UNITY_IOS
			var setting = Settings.Get;

			if (setting == null || string.IsNullOrEmpty(setting.FacebookId))
			{
				//TODO сделать окно ошибок билда

				EditorUtility.DisplayDialog("Build error", "Facebook settings is nothing", "Ok");

				return;
			}

			Debug.Log("Robusta: Setting Facebook Ids");

			FacebookSettings.AppIds = new List<string> {setting.FacebookId};
			FacebookSettings.AppLabels = new List<string> {Application.productName};
			EditorUtility.SetDirty(FacebookSettings.Instance);

			InstallLinkXml();
#endif
		}

		private void InstallLinkXml()
		{
			string robustaPath = Path.Combine(Application.dataPath, "Robusta");
			if (!Directory.Exists(robustaPath))
			{
				Directory.CreateDirectory(robustaPath);
			}

			string facebookPath = Path.Combine(robustaPath, "Facebook");
			if (!Directory.Exists(facebookPath))
			{
				Directory.CreateDirectory(facebookPath);
			}

			var destFile = Path.Combine(facebookPath, "link.xml");

			if (!File.Exists(destFile))
			{
				var sourceFile = Path.Combine(Path.Combine("Packages", "com.espresso-pub.robusta.facebook"), "Vendor", "link.xml");

				File.Copy(sourceFile, destFile);
			}
		}
	}
}