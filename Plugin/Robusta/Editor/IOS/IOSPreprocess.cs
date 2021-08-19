using System.Globalization;
using Google;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Robusta.Editor.iOS
{
    public class IOSPreprocess : IPreprocessBuildWithReport
    {
        private const float MinIosVersion = 9.0f;

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.iOS) {
                return;
            }

            PrepareResolver();
            PreparePlayerSettings();
        }

        private static void PrepareResolver()
        {
            IOSResolver.PodfileGenerationEnabled = true;
            IOSResolver.PodToolExecutionViaShellEnabled = true;
            IOSResolver.AutoPodToolInstallInEditorEnabled = true;
            IOSResolver.UseProjectSettings = true;
            IOSResolver.CocoapodsIntegrationMethodPref = IOSResolver.CocoapodsIntegrationMethod.Project;
        }

        private static void PreparePlayerSettings()
        {
            PlayerSettings.iOS.allowHTTPDownload = true;
            PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 2);

            var changeMinVersion = true;
            if (float.TryParse(PlayerSettings.iOS.targetOSVersionString, out float iosMinVersion)) {
                if (iosMinVersion >= MinIosVersion) {
                    changeMinVersion = false;
                }
            }

            if (changeMinVersion) {
                PlayerSettings.iOS.targetOSVersionString = MinIosVersion.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
