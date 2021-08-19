using System;
// ReSharper disable InconsistentNaming

namespace Robusta
{
	[Serializable]
	public class SettingsData
	{
		public bool RequestSucceeded => success == "true";

		public string success;
		public DataStruct data;
		public string errorCode;
		public ErrorDataStruct errorData;

		[Serializable]
		public struct DataStruct
		{
			public StatsParamsStruct StatsParams;

			[Serializable]
			public struct StatsParamsStruct
			{
				public string ApiKey;
				public string AppId;
				public string FacebookId;
				public string Url;
				public VersionsStruct Versions;

				[Serializable]
				public struct VersionsStruct
				{
					public VersionStruct Unity;
					public VersionStruct FbSdk;
					public VersionStruct RobustaSdk;

					[Serializable]
					public struct VersionStruct
					{
						public string min;
						public string max;
						public string recommended;
					}
				}
			}
		}

		[Serializable]
		public struct ErrorDataStruct
		{
			public string Key;
			public string Value;
		}
	}
}