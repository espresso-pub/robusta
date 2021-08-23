using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Robusta.Editor
{
    public class WizardCheckWork
    {
        public class VersionCheckResult
        {
            public bool Supported;
            public string Current;
            public string Recommended;

        }
        public class CheckWork
        {
            public bool Setup;
            public VersionCheckResult Unity;
            public bool AnalyticsScene;
            public VersionCheckResult RobustaSDK;
            public string FacebookAppId;
            public bool VideoRecordingReady;
            public bool FacebookInstalled;
            public bool FacebookRobustaSDKInstalled;
        }
        
        private static WizardCheckWork _instance;

        public static WizardCheckWork Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WizardCheckWork();
                }

                return _instance;
            }
        }

        private const string PatternPartVersion = @"^(\d+)";

        /// <summary>
        /// Чек настроек, проверка на соответствие версий
        /// </summary>
        /// <param name="cb"></param>
        /// <returns></returns>
        public CheckWork Check(Action<CheckWork> cb = null)
        {
            var settings = Settings.Get;
            if (settings == null) return new CheckWork();

            return new CheckWork()
            {
                Setup = true,
                Unity = CheckVersion(Application.unityVersion, settings.Versions.Unity),
                AnalyticsScene = GameObject.FindObjectOfType<RobustaBehaviour>() != null,
                RobustaSDK = CheckVersion(GetRobustaSdkVersion(), settings.Versions.RobustaSdk),
                FacebookAppId = settings.FacebookId,
                VideoRecordingReady = true,
                FacebookInstalled = CheckInstallFacebook(),
                FacebookRobustaSDKInstalled = CheckInstallFacebookRobusta()
            };
        }

        /// <summary>
        /// Получение версии RobustaSDK
        /// </summary>
        /// <returns></returns>
        private string GetRobustaSdkVersion()
        {
            var robustaVersion = "";

            var packageJsons = AssetDatabase.FindAssets("package")
                .Select(AssetDatabase.GUIDToAssetPath).Where(x => AssetDatabase.LoadAssetAtPath<TextAsset>(x) != null)
                .Select(PackageInfo.FindForAssetPath)
                .ToList();
            
            foreach (var packageInfo in packageJsons)
            {
                if (packageInfo != null && packageInfo.name == "com.espresso-pub.robusta")
                {
                    robustaVersion = packageInfo.version;
                    break;
                }
            }

            return robustaVersion;
        }

        /// <summary>
        /// Проверяет, установлен ли фейсбук.
        /// </summary>
        /// <returns></returns>
        private bool CheckInstallFacebook()
        {
            try
            {
                var assembly = Assembly.Load("Facebook.Unity");
                // Проверяем по наличию основного класса апи фб
                var fbType = assembly.GetType("Facebook.Unity.FB", false, true);
                return fbType != null;
            }
            catch (Exception _)
            {
                return false;
            }
        }

        /// <summary>
        /// Проверяет, установлена ли робуста фейсбук
        /// </summary>
        /// <returns></returns>
        private bool CheckInstallFacebookRobusta()
        {
            return AssetDatabase.FindAssets("package")
                .Select(AssetDatabase.GUIDToAssetPath).Where(x => AssetDatabase.LoadAssetAtPath<TextAsset>(x) != null)
                .Select(PackageInfo.FindForAssetPath)
                .ToList()
                .Any(packageInfo => packageInfo is {name: "com.espresso-pub.robusta.facebook"});
        }

        /// <summary>
        /// Проверяет версию.
        /// Результат - поддерживается ли и какова рекомендуемая (заполнена только в случае, если у нас не она)
        /// </summary>
        /// <param name="yourVersion"></param>
        /// <param name="supportedVersions"></param>
        /// <returns></returns>
        private VersionCheckResult CheckVersion(string yourVersion, 
            SettingsData.DataStruct.StatsParamsStruct.VersionsStruct.VersionStruct supportedVersions)
        {
            var supported = CompareVersions(AnalyzeVersion(yourVersion), AnalyzeVersion(supportedVersions.min),
                AnalyzeVersion(supportedVersions.max), AnalyzeVersion(supportedVersions.recommended), out var equalRecommended);
            
            return new VersionCheckResult()
            {
                Supported = supported,
                Recommended = !equalRecommended ? supportedVersions.recommended : "",
                Current = yourVersion
            };
        }

        /// <summary>
        /// Сравнение твоей версии с мин, макс и рекомендуемой
        /// </summary>
        /// <param name="yourVersion">Твоя версия</param>
        /// <param name="minVersion">Минимальная версия</param>
        /// <param name="maxVersion">Максимальная версия</param>
        /// <param name="recommendedVersion">Рекоммендуемая версия</param>
        /// <param name="equalRecommended">Равна ли твоя версия рекомендуемой</param>
        /// <returns>Находится ли твоя версия между минимальной и максимальной</returns>
        private bool CompareVersions(int[] yourVersion, int[] minVersion, 
            int[] maxVersion, int[] recommendedVersion,
            out bool equalRecommended)
        {
            var compareResult = true;
            equalRecommended = true;
            
            for (int i = 0; i < 3; i++)
            {
                // Ищем до первого несовпадения с рекомендуемой версией
                if (recommendedVersion[i] != yourVersion[i])
                {
                    equalRecommended = false;
                    // Сравниваем, находится ли наша версия между минимальной и максимальной версиями
                    compareResult = yourVersion[i] >= minVersion[i] && yourVersion[i] <= maxVersion[i];
                    break;
                }
            }

            return compareResult;
        }

        /// <summary>
        /// Переводит версию типа строка вида xx.xx.xx в массив чисел из 3х элементов
        /// </summary>
        /// <param name="version">Версия вида xx.xx.xx</param>
        /// <returns></returns>
        private int[] AnalyzeVersion(string version)
        {
            // Версия всегда состоит из 3х значений - чисел
            var ver = new int[3] { 0, 0, 0 };
            if (string.IsNullOrEmpty(version)) return ver;
            
            // Разбиваем по точке
            var parts = version.Split('.');
            // Максимум 3 части нас интересует, если их больше - откидываем
            // Если будет меньше, то оставшаяся часть версии будет нулями
            var count = Mathf.Min(parts.Length, 3);
            for (int i = 0; i < count; i++)
            {
                var part = parts[i];
                if (string.IsNullOrEmpty(part)) continue;
                // Выбираем только числа до встречи с первым символом, не являющемся числом
                var v = Regex.Match(parts[i], PatternPartVersion)?.Value;
                if (string.IsNullOrEmpty(v)) continue;
                int.TryParse(v, out ver[i]);
            }

            return ver;
        }
    }
}