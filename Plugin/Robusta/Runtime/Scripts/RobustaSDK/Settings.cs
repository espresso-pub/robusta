using System;
using UnityEngine;

namespace Robusta
{
  [Serializable]
  public class Settings : ScriptableObject
  {
    public const string SettingsAssetName = "Settings";

    public static Settings Get => Resources.Load<Settings>(SettingsAssetName);

    [SerializeField]
    public string FacebookId;
    [SerializeField]
    public string AppId;
    [SerializeField]
    public string ApiKey;
    [SerializeField]
    public string Url;
    [SerializeField]
    public string ConfigUrl;
    [SerializeField]
    public SettingsData.DataStruct.StatsParamsStruct.VersionsStruct Versions;
  }
}
