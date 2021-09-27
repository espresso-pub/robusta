using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Robusta.Editor.Android
{
    public class PreprocessAndroidBuild : IPreprocessBuildWithReport
    {
        private static readonly string PackageName = "com.espresso-pub.robusta";

        public static readonly string SourceFolderPath = Path.Combine("Packages", PackageName);

        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report)
        {


            if (report.summary.platform != BuildTarget.Android)
            {
                return;
            }

            PreparePlayerSettings();
        }

        private static void PreparePlayerSettings()
        {
            // Set Android ARM64/ARMv7 Architecture
            PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup,
                ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
            // Set Android min version
            if (PlayerSettings.Android.minSdkVersion < AndroidSdkVersions.AndroidApiLevel19)
            {
                PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel19;
            }

// 			PlayerSettings.SetPropertyInt("useCustomMainManifest", 1);
// 			PlayerSettings.SetPropertyInt("useCustomLauncherManifest", 1);
// 			PlayerSettings.SetPropertyInt("useCustomMainGradleTemplate", 1);
// 			PlayerSettings.SetPropertyInt("useCustomLauncherGradleManifest", 1);
// #if UNITY_2020_3_OR_NEWER
// 			PlayerSettings.SetPropertyInt("useCustomGradlePropertiesTemplate", 1);
// #endif
        }
    }
}