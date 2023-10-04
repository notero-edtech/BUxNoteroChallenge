using UnityEditor;
using UnityEngine.SceneManagement;

namespace Hendrix.Editor
{
    public static class EditorSceneManager
    {
        public enum SaveSceneDialogButton : int
        {
            Save = 0,
            Cancel = 1,
            DontSave = 2,
        }

        /// <summary>
        /// Open scene in Unity Editor
        /// </summary>
        /// <param name="scenePath">Location of target scene inside the project</param>
        public static void OpenScene(string scenePath)
        {
            var activeScene = SceneManager.GetActiveScene();

            if(activeScene.path.Equals(scenePath))
            {
                return;
            }

            if(activeScene.isDirty)
            {
                var result = DisplaySaveSceneDialog(activeScene);

                if(result == SaveSceneDialogButton.Cancel)
                {
                    return;
                }

                if(result == SaveSceneDialogButton.Save)
                {
                    string pathToSave = activeScene.path;
                    if(string.IsNullOrEmpty(pathToSave))
                    {
                        pathToSave = SaveScenePanel();

                        // if user press cancel
                        if(string.IsNullOrEmpty(pathToSave))
                        {
                            return;
                        }

                        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(activeScene, pathToSave);
                        AssetDatabase.Refresh();
                    }
                    else
                    {
                        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(activeScene);
                    }
                }
            }

            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);
        }

        /// <summary>
        /// Displays a warning dialog with three buttons.
        /// </summary>
        /// <param name="scene">The dirty scene that needs to be saved</param>
        /// <returns>Button chosen by the user</returns>
        public static SaveSceneDialogButton DisplaySaveSceneDialog(Scene scene)
        {
            string path = string.IsNullOrEmpty(scene.path) ? "Untitled" : scene.path;
            string title = "Scene(s) Have Been Modified";
            string message = $"Do you want to save the changes you made in the scenes:\n {path}";
            string ok = "Save";
            string cancel = "Cancel";
            string alt = "Don't Save";

            int result = EditorUtility.DisplayDialogComplex(title, message, ok, cancel, alt);
            return (SaveSceneDialogButton)result;
        }

        /// <summary>
        /// Displays the "Save Scene" dialog and returns the selected path name.
        /// </summary>
        /// <returns>Selected path name</returns>
        public static string SaveScenePanel()
        {
            return EditorUtility.SaveFilePanel("Save Scene", "Assets/Scenes", "", "unity");
        }
    }
}