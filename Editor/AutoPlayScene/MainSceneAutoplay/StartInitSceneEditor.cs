using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;

namespace Trantorian.Editor
{
  public class StartInitSceneEditor
  {
    static class ToolbarStyles
    {
      public static readonly GUIStyle commandButtonStyle;

      static ToolbarStyles()
      {
        commandButtonStyle = new GUIStyle("Command")
        {
          fontSize = 12,
          alignment = TextAnchor.MiddleCenter,
          imagePosition = ImagePosition.ImageAbove,
          fontStyle = FontStyle.Bold,
          fixedHeight = 20,
          fixedWidth = 25,
          richText = true
        };
      }
    }

    [InitializeOnLoad]
    public class PlayWithMainMenu
    {
      static PlayWithMainMenu()
      {
        ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
      }

      static void OnToolbarGUI()
      {
        if (EditorApplication.isPlaying)
          return;

        GUILayout.FlexibleSpace();

        if (GUILayout.Button(new GUIContent("<color=white>M</color>", "Load main scene and play"),
          ToolbarStyles.commandButtonStyle))
          SceneHelper.StartScene("Init", EditorSceneManager.GetActiveScene().name);

      }
    }

    static class SceneHelper
    {
      static string sceneToOpen;

      public static void StartScene(string sceneName, string currentScene)
      {
        if (EditorApplication.isPlaying)
        {
          EditorApplication.isPlaying = false;
        }

        sceneToOpen = sceneName;
        EditorApplication.update += OnExecuteScene;
      }

      static void OnExecuteScene()
      {
        OnUpdate();
      }

      static void OnUpdate()
      {
        if (sceneToOpen == null ||
            EditorApplication.isPlaying || EditorApplication.isPaused ||
            EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
        {
          return;
        }

        EditorApplication.update -= OnUpdate;

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
          // need to get scene via search because the path to the scene
          // file contains the package version so it'll change over time
          string[] guids = AssetDatabase.FindAssets("t:scene " + sceneToOpen, null);
          if (guids.Length == 0)
          {
            Debug.LogWarning("Couldn't find scene file");
          }
          else
          {
            string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
            EditorSceneManager.OpenScene(scenePath);
            EditorApplication.isPlaying = true;
          }
        }
        sceneToOpen = null;
      }
    }
  }
}
