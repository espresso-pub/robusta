using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace Robusta.Editor
{
    /// <summary>
    /// Установщик модуля фб
    /// </summary>
    public static class WizardFacebookInstaller
    {
        private static RemoveRequest _fbRemoveRequest;
        private static AddRequest _fbAddRequest;

        /// <summary>
        /// Удаление модуля фб
        /// </summary>
        public static void Remove()
        {
            _fbRemoveRequest = Client.Remove("com.espresso-pub.robusta.facebook");
            EditorApplication.update += Progress;
        }

        /// <summary>
        /// Установка модуля фб
        /// </summary>
        /// <param name="robustaVersion"></param>
        public static void Install(string robustaVersion)
        {
            Debug.Log("Facebook robusta SDK module intalling...");
            _fbAddRequest =
                Client.Add(
                    $"https://github.com/espresso-pub/robusta.git?path=/Plugin/Robusta.Facebook#v{robustaVersion}");
            EditorApplication.update += Progress;
        }
        
        private static void Progress()
        {
            // Проверяем, закончилось ли удаление
            if (_fbRemoveRequest?.IsCompleted ?? false)
            {
                if (_fbRemoveRequest.Status == StatusCode.Success)
                    Debug.Log("Facebook robusta SDK module removed");
                else if (_fbRemoveRequest.Status >= StatusCode.Failure)
                    Debug.LogError(_fbRemoveRequest.Error.message);

                EditorApplication.update -= Progress;
                _fbRemoveRequest = null;
                return;
            }
            
            // Проверяем, закончилось ли установка
            if (_fbAddRequest?.IsCompleted ?? false)
            {
                if (_fbAddRequest.Status == StatusCode.Success)
                    Debug.Log("Facebook robusta SDK module intalled: " + _fbAddRequest.Result.packageId);
                else if (_fbAddRequest.Status >= StatusCode.Failure)
                    Debug.LogError(_fbAddRequest.Error.message);

                EditorApplication.update -= Progress;
                _fbAddRequest = null;
            }
        }
    }
}