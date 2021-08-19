using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Robusta
{
	public class RobustaBehaviour : MonoBehaviour
	{
		private static RobustaBehaviour _instance;

#if UNITY_EDITOR
		/// <summary>
		/// Менеджер записи видео и скриншотов
		/// </summary>
		private RecordManager _recordManager;
		
		/// <summary>
		/// Корутина записи видео
		/// </summary>
		private Coroutine _recordCoroutine;

		/// <summary>
		/// Продолжительность видео
		/// </summary>
		private const float RecordDuration = 30f;
		/// <summary>
		/// Стиль кнопки записи
		/// </summary>
		private GUIStyle _recordStyle;
		
		/// <summary>
		/// Текущее время длительности записи видео (сколько уже длится запись видео)
		/// </summary>
		private float _currentTimeRecord;
#endif

		private void Awake()
		{
			if (_instance != null)
			{
				Destroy(gameObject);
				return;
			}

			_instance = this;

			DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
			_recordManager = new RecordManager();
			
			_recordStyle = new GUIStyle(EditorStyles.miniButton);
			_recordStyle.fontSize = 24;
			_recordStyle.alignment = TextAnchor.MiddleCenter;
			_recordStyle.fixedHeight = 0;
#endif

			RobustaAnalytics.Init(Settings.Get);
		}

		private void Update()
		{
#if UNITY_EDITOR
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				// Shift + R - запись видео на 30 сек
				if (Input.GetKey(KeyCode.R) && !_recordManager.IsVideoRecording())
				{
					_currentTimeRecord = 0;
					_recordCoroutine = StartCoroutine(_recordManager.StartVideo(0, RecordDuration));
				}

				// Shift + S - скриншоты
				// Будет конфликт разрешений, поэтому не делаем во время записи видео
				if (Input.GetKey(KeyCode.S) && !_recordManager.IsVideoRecording())
				{
					_recordManager.ScreenShot();
				}
			}
			
			// Считаем, сколько длится запись (она не зависит от паузы в игре)
			if (_recordManager.IsVideoRecording()) _currentTimeRecord += Time.unscaledDeltaTime;
#endif
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			Debug.Log($"OnApplicationFocus {hasFocus}");
			var isPaused = !hasFocus;
			OnApplicationPause(isPaused);
		}

		private void OnApplicationPause(bool paused)
		{
			Debug.Log($"OnApplicationPause {paused}");
			if (paused)
			{
				RobustaAnalytics.ApplicationPaused();
			}
			else
			{
				RobustaAnalytics.ApplicationResumed();
			}
		}
		
#if UNITY_EDITOR
		void OnGUI()
		{
			if (!_recordManager.IsVideoRecording()) return;

			var time = Mathf.RoundToInt(RecordDuration - _currentTimeRecord);
			if (GUI.Button(new Rect(10, 10, 150, 80), $"REC {time}", _recordStyle))
			{
				StopCoroutine(_recordCoroutine);
				_recordManager.StopVideo();
			}
		}
#endif
	}
}