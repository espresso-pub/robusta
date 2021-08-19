using System.IO;
using Robusta.Editor.Utils;
using UnityEditor.Android;

public class PostprocessAndroidBuild : IPostGenerateGradleAndroidProject
{
  public int callbackOrder => 0;
  
  public void OnPostGenerateGradleAndroidProject(string path)
  {
#if UNITY_2019_3_OR_NEWER
    path += "/../";
#endif
    
    var fileInfo = new FileInfo(Path.Combine(path, "gradle.properties"));

    var javaPropertiesFile = new JavaPropertiesFile(fileInfo.FullName);

    javaPropertiesFile.Set("android.enableR8", "false");
    javaPropertiesFile.Set("android.useAndroidX", "true");
    javaPropertiesFile.Set("android.enableJetifier", "true");

    javaPropertiesFile.Save();
  }
}