using UnityEngine;
using Hollow.Core;

namespace Hollow.UI
{
    /// <summary>
    /// Pause menu: resume, restart checkpoint, return to menu.
    /// Buttons wired in Inspector; logic kept here (Master §14.2).
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        public void Resume()
        {
            if (GameManager.Instance != null && GameManager.Instance.IsPaused)
            {
                GameManager.Instance.TogglePause();
            }
        }

        public void RestartCheckpoint()
        {
            Time.timeScale = 1f;
            GameManager.Instance?.SaveSystem?.LoadGame();
            Resume();
        }

        public void ReturnToMenu()
        {
            Time.timeScale = 1f;
            GameManager.Instance?.SceneTransitionManager?.LoadScene("MainMenu", saveCheckpointFirst: true);
        }
    }
}
