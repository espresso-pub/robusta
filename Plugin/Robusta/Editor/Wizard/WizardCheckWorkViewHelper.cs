using System.Collections.Generic;

namespace Robusta.Editor
{
    public static class WizardCheckWorkViewHelper
    {
        public class CheckInfo
        {
            public bool Check;
            public string Info;
        }
        
        public static List<CheckInfo> GetDrawData(WizardCheckWork.CheckWork checkWorkData)
        {
            var data = new List<CheckInfo>();

            if (checkWorkData.Setup)
            {
                data.Add(new CheckInfo()
                {
                    Check = checkWorkData.Setup,
                    Info = "setup - complite"
                });
            }
            else
            {
                data.Add(new CheckInfo()
                {
                    Check = checkWorkData.Setup,
                    Info = "setup - pending"
                });

                return data;
            }

            if (checkWorkData.Unity != null)
            {
                var unitySupported = checkWorkData.Unity.Supported ? "supported" : "not supported";
                var unityRecommended = !string.IsNullOrEmpty(checkWorkData.Unity.Recommended)
                    ? $" Recommended version: {checkWorkData.Unity.Recommended}"
                    : "";
                data.Add(new CheckInfo()
                {
                    Check = checkWorkData.Unity.Supported,
                    Info = $"unity version: {checkWorkData.Unity.Current} - {unitySupported}.{unityRecommended}"
                });
            }
            else
            {
                data.Add(new CheckInfo()
                {
                    Check = false,
                    Info = $"unity version undefined"
                });
            }

            data.Add(new CheckInfo()
            {
                Check = checkWorkData.AnalyticsScene,
                Info = "analytics added to scene"
            });

            if (checkWorkData.RobustaSDK != null)
            {
                var robustaSupported = checkWorkData.RobustaSDK.Supported ? "supported" : "not supported";
                var robustaRecommended = !string.IsNullOrEmpty(checkWorkData.RobustaSDK.Recommended)
                    ? $" Recommended version: {checkWorkData.RobustaSDK.Recommended}"
                    : "";
                data.Add(new CheckInfo()
                {
                    Check = checkWorkData.RobustaSDK.Supported,
                    Info =
                        $"robusta SDK version: {checkWorkData.RobustaSDK.Current} - {robustaSupported}.{robustaRecommended}"
                });
            }
            else
            {
                data.Add(new CheckInfo()
                {
                    Check = false,
                    Info = $"robusta SDK version undefined"
                });
            }

            data.Add(new CheckInfo()
            {
                Check = !string.IsNullOrEmpty(checkWorkData.FacebookAppId),
                Info = $"facebook app id: {checkWorkData.FacebookAppId}"
            });

            data.Add(new CheckInfo()
            {
                Check = checkWorkData.VideoRecordingReady,
                Info = "video recording ready"
            });

            return data;
        }
    }
}