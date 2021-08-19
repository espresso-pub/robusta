using UnityEditor;

namespace Robusta.Editor
{
  [InitializeOnLoad]
  public class WizardRunner
  {
    static WizardRunner()
    {
      // Waiting for Editor to initialize
      EditorApplication.delayCall += WizardWindow.InitIfNeeded;
    }
  }
}
