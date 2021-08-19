
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
#endif

namespace Robusta
{
    public class RecordManager
    {
        public Action<int> OnDelayEvent;
        public Action OnBeforeStartVideoEvent;
        public Action OnVideoStopedEvent;
        
        /// <summary>
        /// Путь к видео записям
        /// (нужен именно полный путь, поэтому Path.GetFullPath)
        /// </summary>
        public static string VideoFolder => Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Records", "Video"));
        /// <summary>
        /// Путь к картинкам
        /// (нужен именно полный путь, поэтому Path.GetFullPath)
        /// </summary>
        public static string ImageFolder => Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Records", "ScreenShots"));

        private bool _videoStarted;

#if UNITY_EDITOR
        private RecorderController _recorderController;
        private RecorderController _screenShotController;
        
        /// <summary>
        /// Разрешения для скриншота
        /// </summary>
        private List<StandardImageInputSettings> _imageInputSettings = new List<StandardImageInputSettings>()
        {
            new CameraInputSettings()
            {
                OutputWidth = 1080,
                OutputHeight = 2340
            }
        };

        /// <summary>
        /// Настройки для скриншота
        /// </summary>
        private void ScreenShotSettings()
        {
            var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            _screenShotController = new RecorderController(controllerSettings);
            
            var mediaOutputFolder = ImageFolder;

            foreach (var inputSettings in _imageInputSettings)
            {
                controllerSettings.AddRecorderSettings(GetImageRecorderSettings(inputSettings, mediaOutputFolder));
            }
            
            controllerSettings.SetRecordModeToSingleFrame(0);
            controllerSettings.FrameRate = 60.0f;
            RecorderOptions.VerboseMode = false;
        }

        /// <summary>
        /// Получение настроек под разные разрешения для скриншота
        /// </summary>
        /// <param name="inputSettings"></param>
        /// <param name="mediaOutputFolder"></param>
        /// <returns></returns>
        private ImageRecorderSettings GetImageRecorderSettings(StandardImageInputSettings inputSettings, string mediaOutputFolder)
        {
            var imageRecorder = ScriptableObject.CreateInstance<ImageRecorderSettings>();
            imageRecorder.name = "Image Recorder";
            imageRecorder.Enabled = true;
            // Записываем в формате png
            imageRecorder.OutputFormat = ImageRecorderSettings.ImageRecorderOutputFormat.PNG;
            imageRecorder.CaptureAlpha = false;

            // К имени добавляем дату и разрешение
            imageRecorder.OutputFile = Path.Combine(mediaOutputFolder, "image_") + DateTime.Now + 
                                       $"_{inputSettings.OutputWidth}x{inputSettings.OutputHeight}";

            imageRecorder.imageInputSettings = inputSettings;

            return imageRecorder;
        }

        /// <summary>
        /// Настройки для записи видео
        /// </summary>
        private void VideoSettings()
        {
            var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            _recorderController = new RecorderController(controllerSettings);
            
            var mediaOutputFolder = VideoFolder;

            var settings = ScriptableObject.CreateInstance<MovieRecorderSettings>();
            settings.name = "My Video Recorder";
            settings.Enabled = true;

            // Формат и качество
            settings.OutputFormat = MovieRecorderSettings.VideoRecorderOutputFormat.MP4;
            settings.VideoBitRateMode = VideoBitrateMode.High;

            // Разрешение (если не указать, то берет текущее в редакторе)
            settings.ImageInputSettings = new CameraInputSettings()
            {
                OutputWidth = 1080,
                OutputHeight = 1350
            };

            // Записывать ли аудио (обязательно аудио в редакторе должно быть включено, иначе не запишет)
            settings.AudioInputSettings.PreserveAudio = true;

            // К имени добавляем текущую дату
            settings.OutputFile = Path.Combine(mediaOutputFolder, "video_") + DateTime.Now;

            controllerSettings.AddRecorderSettings(settings);
            controllerSettings.SetRecordModeToManual();
            controllerSettings.FrameRate = 60.0f;

            RecorderOptions.VerboseMode = false;
        }
#endif
        
        /// <summary>
        /// Скриншот
        /// </summary>
        public void ScreenShot()
        {
#if UNITY_EDITOR
            ScreenShotSettings();
            Debug.Log("RecordManager: ScreenShot");
            _screenShotController.PrepareRecording();
            _screenShotController.StartRecording();
#endif
        }

        /// <summary>
        /// Идет ли запись видео
        /// </summary>
        /// <returns></returns>
        public bool IsVideoRecording()
        {
#if UNITY_EDITOR
            return _videoStarted || (_recorderController?.IsRecording() == true);
#endif
            return false;
        }

        /// <summary>
        /// Старт записи видео
        /// </summary>
        /// <param name="delay">Пауза перед стартом</param>
        /// <param name="duration">Длительность записи</param>
        /// <returns></returns>
        public IEnumerator StartVideo(int delay = 0, float duration = 0f)
        {
#if UNITY_EDITOR
            // Проверка, есть ли запись видео
            if (_videoStarted || _recorderController?.IsRecording() == true) yield break;

            _videoStarted = true;

            // Обратный отсчет до старта видео
            while (delay > 0)
            {
                OnDelayEvent?.Invoke(delay);
                yield return new WaitForSecondsRealtime(1f);
                delay--;
            }
            
            // Настройки для записи видео
            VideoSettings();
            
            // Событие перед началом записи
            OnBeforeStartVideoEvent?.Invoke();
            
            Debug.Log("RecordManager: video recording STARTED");
            
            // Старт записи
            _recorderController.PrepareRecording();
            _recorderController.StartRecording();

            if (duration > 0)
            {
                yield return new WaitForSecondsRealtime(duration);
                StopVideo();
            }
#else
            yield break;
#endif
        }

        /// <summary>
        /// Остановка видео
        /// </summary>
        public void StopVideo()
        {
#if UNITY_EDITOR
            if (_recorderController?.IsRecording() != true)
            {
                _videoStarted = false;
                return;
            }
            _recorderController.StopRecording();
            _videoStarted = false;
            Debug.Log("RecordManager: video recording STOPED");
            OnVideoStopedEvent?.Invoke();
#endif
        }
    }
}
