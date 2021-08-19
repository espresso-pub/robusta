using System;
using System.Collections;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Robusta.Editor
{
	
	using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
 
//Here is a static class to hold my gui stuff.
public static class RobustaGUILayout {
 
    //Alternate Method
    public static bool LinkLabel(string labelText){
        return LinkLabel (labelText,  new Color (0x00/255f, 0x78/255f, 0xDA/255f, 1f), new Vector2 (), 0);
    }
 
    //Alternate Method
    public static bool LinkLabel(string labelText, Color labelColor){
        return LinkLabel (labelText, labelColor, new Vector2 (), 0);
    }
 
    //Alternate Method
    public static bool LinkLabel(string labelText, Color labelColor, Vector2 contentOffset){
        return LinkLabel (labelText, labelColor, contentOffset, 0);
    }
 
    //The Main Method
    public static bool LinkLabel(string labelText, Color labelColor, Vector2 contentOffset, int fontSize){
        //Let's use Unity's label style for this
        GUIStyle stl = EditorStyles.label;
        //Next let's record the settings for Unity's label style because we will have to make sure these settings get returned back to
        //normal after we are done changing them and drawing our LinkLabel.
        Color col = stl.normal.textColor;
        Vector2 os = stl.contentOffset;
        int size = stl.fontSize;
        //Now we can modify the label's settings via the editor style : EditorStyles.label (stl).
        stl.normal.textColor = labelColor;
        stl.contentOffset = contentOffset;
        stl.fontSize = fontSize;
        //We are now ready to draw our Linklabel. I will actually use a GUILayout.Button to do this and our "stl" style will
        //make the button appear as a label.
 
        //Note : You may include a web address parameter in this method and open a URL at this point if the button is clicked,
        //however, I am going to just return bool based on weather or not the link was clicked. This gives me more control over
        //what actually happens when a link label is used. I also will instead include a "URL version" of this method below.
 
        //Since the button already returns bool, I will just return that result straight across like this.
 
        try{
            return GUILayout.Button(labelText, stl);
        }
        finally{
            //Remember to set the editor style (stl) back to normal here. A try / finally clause will work perfectly for this!!!
 
            stl.normal.textColor = col;
            stl.contentOffset = os;
            stl.fontSize = size;
        }
    }
 
    //This is a modified version of link label that opens a URL automatically. Note : this can also return bool if you want.
    public static void LinkLabel(string labelText, Color labelColor, Vector2 contentOffset, int fontSize, string webAddress){
        if (LinkLabel (labelText, labelColor, contentOffset, fontSize)) {
            try{
                Application.OpenURL(@webAddress);
                //if returning bool, return true here.
            }
            catch{
                //In most cases, the catch clause would not happen but in the interest of being thorough I will log an
                //error and have Unity "beep" if an exception gets thrown for any reason.
                Debug.LogError("Could not open URL. Please check your network connection and ensure the web address is correct.");
                EditorApplication.Beep ();
            }
        }
        //if returning bool, return false here.
    }
}
	public class WizardWindow : EditorWindow
	{
		private const string RobustaAnalyticsPrefabPath = "Packages/Robusta/Prefabs/FxAnalytics.prefab";

		bool m_Initialized;
	
		GUIStyle LinkStyle { get { return m_LinkStyle; } }
		[SerializeField] GUIStyle m_LinkStyle;
	
		GUIStyle TitleStyle { get { return m_TitleStyle; } }
		[SerializeField] GUIStyle m_TitleStyle;
	
		GUIStyle HeadingStyle { get { return m_HeadingStyle; } }
		[SerializeField] GUIStyle m_HeadingStyle;
	
		GUIStyle BodyStyle { get { return m_BodyStyle; } }
		[SerializeField] GUIStyle m_BodyStyle;
		
		GUIStyle BodyWithRichStyle { get { return m_BodyWithRichStyle; } }
		[SerializeField] GUIStyle m_BodyWithRichStyle;

		[MenuItem("Robusta/Wizard")]
		public static void Init()
		{

			
			var w = Instance;
		}

		private static WizardWindow _instance;

		public static WizardWindow Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = (WizardWindow) GetWindow(typeof(WizardWindow), true, "Robusta wizard", true);
				}

				return _instance;
			}
		}

		public static void InitIfNeeded()
		{
			var settings = Resources.Load<Settings>("Settings");
			if (settings != null) return;

			Init();
		}

		private string _settingsURL;
		
		bool LinkLabel(GUIContent label, params GUILayoutOption[] options) {
			var position = GUILayoutUtility.GetRect(label, LinkStyle, options);
         
			Handles.BeginGUI();
			Handles.color = LinkStyle.normal.textColor;
			Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
			Handles.color = Color.white;
			Handles.EndGUI();
         
			EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);
         
			return GUI.Button(position, label, LinkStyle);
		}

		private void OnGUI()
		{
			if (!m_Initialized)
			{
				m_BodyStyle = new GUIStyle(EditorStyles.label);
				m_BodyStyle.wordWrap = true;
				m_BodyStyle.fontSize = 14;
				
				m_BodyWithRichStyle = new GUIStyle(m_BodyStyle);
				m_BodyWithRichStyle.richText = true;

				m_TitleStyle = new GUIStyle(m_BodyStyle);
				m_TitleStyle.fontSize = 24;
				m_TitleStyle.alignment = TextAnchor.MiddleCenter;


				m_HeadingStyle = new GUIStyle(m_BodyStyle);
				m_HeadingStyle.fontSize = 18 ;
		
				m_LinkStyle = new GUIStyle(m_BodyStyle);
				m_LinkStyle.wordWrap = false;
				// Match selection color which works nicely for both light and dark skins
				m_LinkStyle.normal.textColor = new Color (0x00/255f, 0x78/255f, 0xDA/255f, 1f);
				m_LinkStyle.stretchWidth = false;
				m_Initialized = true;
				
				// Устанавливаем предыдущее значение
				_settingsURL = Settings.Get?.ConfigUrl;
			}
			
			EditorGUILayout.Space(5);
			DrawSettingsURL();
			DrawCheckWork();
			EditorGUILayout.Space(5);
			DrawRecordSettings();
		}

		private void DrawRecordSettings()
		{
			EditorGUILayout.BeginVertical();
			GUILayout.Label("Game image / video recording", m_HeadingStyle);
			var color = GUI.color;
			GUI.color = Color.green;
			GUILayout.Label("Press: Shift + R to record 30 seconds video clip in editor", m_BodyStyle);
			if (GUILayout.Button("Open clips folder", GUILayout.Width(150)))
			{
				var path = RecordManager.VideoFolder;
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				
				EditorUtility.OpenFilePanel("Video", path, "");
			}
			GUILayout.Label("Press: Shift + S to screenshots", m_BodyStyle);
			if (GUILayout.Button("Open screenshots folder", GUILayout.Width(200)))
			{
				var path = RecordManager.ImageFolder;
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}

				EditorUtility.OpenFilePanel("Screenshots", path, "");
			}
			
			GUI.color = color;
			EditorGUILayout.EndVertical();
		}

		/// <summary>
		/// Отрисовка результатов проверки
		/// </summary>
		private void DrawCheckWork()
		{
			// Выполняем проверку
			var checkResult = WizardCheckWork.Instance.Check();
			// Переводим результаты проверки в удобный формат для отрисовки
			var checkView = WizardCheckWorkViewHelper.GetDrawData(checkResult);
			
			EditorGUILayout.BeginVertical();
			GUILayout.Label("Status", m_HeadingStyle);

			var color = GUI.color;
			foreach (var checkInfo in checkView)
			{
				EditorGUILayout.BeginHorizontal();
				var check = checkInfo.Check ? "V" : "X";
				GUI.color = checkInfo.Check ? Color.green : Color.red;
				GUILayout.Label($"<b>[{check}]</b> {checkInfo.Info}", m_BodyWithRichStyle);
				EditorGUILayout.EndHorizontal();
			}
			GUI.color = color;
			
			EditorGUILayout.EndVertical();
		}

		private void DrawSettingsURL()
		{
			EditorGUILayout.BeginVertical();
			GUILayout.Label("Robusta SDK",m_TitleStyle);
			
			string address = "https://espresso-pub.com";
				
			if (RobustaGUILayout.LinkLabel ($"Get your settings link at {address}")) {
				Application.OpenURL (@address);
			}
			
			EditorGUILayout.BeginHorizontal();
			_settingsURL = EditorGUILayout.TextField("Settings URL", _settingsURL);
			
			GUI.enabled = !string.IsNullOrEmpty(_settingsURL);

			var color = GUI.color;
			GUI.color = Color.yellow;

			if (GUILayout.Button("GET Settings", GUILayout.Width(100)))
			{
				GetSettings();
			}

			GUI.color = color;
			GUI.enabled = true;
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
		}

		private void GetSettings()
		{
			EditorCoroutineUtility.StartCoroutine(GetSettingsRoutine(), this);
		}

		private IEnumerator GetSettingsRoutine()
		{
			using (var www = UnityWebRequest.Get(_settingsURL))
			{
				yield return www.SendWebRequest();

				if (string.IsNullOrEmpty(www.error))
				{
					var json = www.downloadHandler.text;
					var parsed = ParseSettings(json, _settingsURL);

					if (!parsed)
					{
						Debug.LogError(json);
						EditorUtility.DisplayDialog("Settings loading error", "Check the settings URL and try again", "Ok");
					}
					else
					{
						SetupScene();
					}
				}
				else
				{
					Debug.LogError(www.error);
					EditorUtility.DisplayDialog("Settings loading error", "Check the settings URL and try again", "Ok");
				}
			}

			Debug.Log("Settings loading done");
		}

		private void SetupScene()
		{
			var go = FindObjectOfType<RobustaBehaviour>();

			if (go == null)
			{
				if (EditorUtility.DisplayDialog("Setup Scene",
					"Settings successfully loaded. Do you want to place a analytics behaviour on the scene?", "Yes",
					"Later"))
				{
					CreateRobustaBehaviour();
					ShowSuccessMessage();
				}
			}
			else
			{
				ShowSuccessMessage();
			}
		}

		private static bool ParseSettings(string json, string configUrl)
		{
			try
			{
				var data = JsonUtility.FromJson<SettingsData>(json);


				if (!data.RequestSucceeded)
				{
					return false;
				}

				var settings = Settings.Get;

				if (settings == null)
				{
					settings = CreateInstance<Settings>();

					if (!AssetDatabase.IsValidFolder("Assets/Resources"))
					{
						AssetDatabase.CreateFolder("Assets", "Resources");
					}

					AssetDatabase.CreateAsset(settings, "Assets/Resources/Settings.asset");
				}

				settings.FacebookId = data.data.StatsParams.FacebookId;
				settings.AppId = data.data.StatsParams.AppId;
				settings.ApiKey = data.data.StatsParams.ApiKey;
				settings.Url = data.data.StatsParams.Url;
				settings.ConfigUrl = configUrl;
				settings.Versions = data.data.StatsParams.Versions;
				EditorUtility.SetDirty(settings);
				AssetDatabase.SaveAssets();
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				EditorUtility.DisplayDialog("Settings loading error", "Check the settings URL and try again", "Ok");
				return false;
			}

			return true;
		}

		private void ShowSuccessMessage()
		{
			if (EditorUtility.DisplayDialog("Setup scene", "Analytics setup completed successfully", "Ok"))
			{
				// Не закрываем после установки, чтобы посмотреть чек
				// Close();
			}
		}

		private static void CreateRobustaBehaviour()
		{
			var prefab = Resources.Load<GameObject>("RobustaBehaviour");
			if (prefab != null)
			{
				var go = Instantiate(prefab);
				go.name = "RobustaBehaviour";

				EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
			}
		}
	}
}