using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hollow.Core
{
    /// <summary>
    /// Fade-based scene transitions that preserve intended player state
    /// (Master §15.3). Saves a checkpoint before loading when requested.
    /// </summary>
    public class SceneTransitionManager : MonoBehaviour
    {
        [SerializeField] private float fadeDuration = 1.2f;
        [SerializeField] private Canvas fadeCanvas;

        private bool _isTransitioning;

        public bool IsTransitioning => _isTransitioning;

        private void Awake()
        {
            if (fadeCanvas == null)
            {
                // Minimal runtime fade canvas if none assigned in Inspector.
                var go = new GameObject("FadeCanvas");
                fadeCanvas = go.AddComponent<Canvas>();
                fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                fadeCanvas.sortingOrder = 100;
                var imgGo = new GameObject("FadeImage");
                imgGo.transform.SetParent(go.transform, false);
                var img = imgGo.AddComponent<UnityEngine.UI.Image>();
                img.color = Color.black;
                var rect = imgGo.GetComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                SetFade(0f);
                DontDestroyOnLoad(go);
            }
        }

        public void LoadScene(string sceneName, bool saveCheckpointFirst = false)
        {
            if (_isTransitioning) return;
            StartCoroutine(LoadSceneRoutine(sceneName, saveCheckpointFirst));
        }

        private IEnumerator LoadSceneRoutine(string sceneName, bool saveCheckpointFirst)
        {
            _isTransitioning = true;

            if (saveCheckpointFirst)
            {
                GameManager.Instance?.SaveCheckpoint();
            }

            yield return FadeTo(1f);

            var op = SceneManager.LoadSceneAsync(sceneName);
            while (op != null && !op.isDone)
            {
                yield return null;
            }

            yield return FadeTo(0f);
            _isTransitioning = false;
        }

        private IEnumerator FadeTo(float targetAlpha)
        {
            if (fadeCanvas == null) yield break;
            var img = fadeCanvas.GetComponentInChildren<UnityEngine.UI.Image>();
            if (img == null) yield break;

            float start = img.color.a;
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / fadeDuration);
                float a = Mathf.Lerp(start, targetAlpha, t);
                SetFade(a);
                yield return null;
            }
            SetFade(targetAlpha);
        }

        private void SetFade(float alpha)
        {
            if (fadeCanvas == null) return;
            var img = fadeCanvas.GetComponentInChildren<UnityEngine.UI.Image>();
            if (img == null) return;
            var c = img.color;
            c.a = alpha;
            img.color = c;
        }
    }
}
