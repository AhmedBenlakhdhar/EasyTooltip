namespace PixelAdder.SimpleTooltip
{
    using System.Collections;
    using System.Text;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    /// <summary>
    /// Singleton that controls a single tooltip instance's lifecycle.
    /// </summary>
    public class TooltipManager : MonoBehaviour
    {
        public static TooltipManager Instance { get; private set; }

        [Header("Core Configuration")]
        [SerializeField] private Tooltip tooltipPrefab;

        [Header("Layout Settings")]
        [SerializeField, Min(50f)] private float maxTooltipWidth = 350f;

        [Header("Animation Settings")]
        [SerializeField, Min(0f)] private float fadeDuration = 0.2f;

        [Header("Positioning")]
        [SerializeField] private Vector2 positionOffset = new(0, -20);

        private Tooltip tooltipInstance;
        private RectTransform tooltipRect;
        private CanvasGroup canvasGroup;
        private Coroutine activeCoroutine;

        private void Awake()
        {
            if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
            else { Destroy(gameObject); }
        }

        private void Start()
        {
            Canvas rootCanvas = FindFirstObjectByType<Canvas>();
            if (!rootCanvas)
            {
                Debug.LogError("TooltipManager: No Canvas found in scene.");
                return;
            }

            GameObject tooltipObj = Instantiate(tooltipPrefab.gameObject, rootCanvas.transform, false);
            tooltipInstance = tooltipObj.GetComponent<Tooltip>();
            tooltipRect = tooltipObj.GetComponent<RectTransform>();
            canvasGroup = tooltipObj.GetComponent<CanvasGroup>();

            tooltipObj.SetActive(false);
        }

        public void ShowTooltip(string content, string title, Sprite icon, Color titleColor, Color iconColor, float delay)
        {
            if (!tooltipInstance) return;
            canvasGroup.alpha = 0;
            if (activeCoroutine != null) StopCoroutine(activeCoroutine);
            activeCoroutine = StartCoroutine(ShowRoutine(content, title, icon, titleColor, iconColor, delay));
        }

        public void HideTooltip()
        {
            if (!tooltipInstance) return;
            if (activeCoroutine != null) StopCoroutine(activeCoroutine);
            if (tooltipInstance.gameObject.activeInHierarchy)
                activeCoroutine = StartCoroutine(FadeOut());
        }

        private IEnumerator ShowRoutine(string content, string title, Sprite icon, Color titleColor, Color iconColor, float delay)
        {
            yield return new WaitForSeconds(delay);
            yield return ResizeTooltipRoutine(content, title, icon, titleColor, iconColor);

            tooltipInstance.gameObject.SetActive(true);
            tooltipInstance.transform.SetAsLastSibling();
            PositionTooltip();
            activeCoroutine = StartCoroutine(FadeIn());
        }

        private IEnumerator ResizeTooltipRoutine(string content, string title, Sprite icon, Color titleColor, Color iconColor)
        {
            tooltipInstance.gameObject.SetActive(false);

            // --- LOGIC FIX 1: Using direct references instead of GetComponentInChildren ---
            // This ensures we are wrapping the correct text component every time.
            float availableTitleWidth = CalculateAvailableWidthForText(tooltipInstance.titleField);
            float availableContentWidth = CalculateAvailableWidthForText(tooltipInstance.contentField);

            string wrappedTitle = WrapText(title, tooltipInstance.titleField, availableTitleWidth);
            string wrappedContent = WrapText(content, tooltipInstance.contentField, availableContentWidth);

            tooltipInstance.SetText(wrappedContent, wrappedTitle, icon, titleColor, iconColor);

            // --- LOGIC FIX 2: Restored the robust "triple cycle" resize method ---
            // This is critical for handling complex, nested layouts reliably.
            for (int i = 0; i < 3; i++)
            {
                tooltipInstance.gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipRect);
                yield return new WaitForEndOfFrame();
                tooltipInstance.gameObject.SetActive(false);
            }
        }

        private IEnumerator FadeIn()
        {
            float start = Time.unscaledTime;
            while (Time.unscaledTime < start + fadeDuration)
            {
                canvasGroup.alpha = Mathf.Lerp(0, 1, (Time.unscaledTime - start) / fadeDuration);
                yield return null;
            }
            canvasGroup.alpha = 1;
        }

        private IEnumerator FadeOut()
        {
            float start = Time.unscaledTime;
            float startAlpha = canvasGroup.alpha;
            while (Time.unscaledTime < start + fadeDuration)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, (Time.unscaledTime - start) / fadeDuration);
                yield return null;
            }
            canvasGroup.alpha = 0;
            tooltipInstance.gameObject.SetActive(false);
        }

        private void PositionTooltip()
        {
            Vector3 mousePos = Input.mousePosition + (Vector3)positionOffset;
            float x = Mathf.Clamp(mousePos.x, 0, Screen.width - tooltipRect.rect.width);
            float y = Mathf.Clamp(mousePos.y, tooltipRect.rect.height, Screen.height);
            tooltipRect.position = new Vector3(x, y, 0);
        }

        private float CalculateAvailableWidthForText(TMP_Text textElement)
        {
            float availableWidth = maxTooltipWidth;
            if (textElement == null) return availableWidth;
            Transform current = textElement.transform;
            while (current != null && current != tooltipInstance.transform)
            {
                if (current.TryGetComponent<LayoutGroup>(out var layoutGroup))
                {
                    availableWidth -= (layoutGroup.padding.left + layoutGroup.padding.right);
                    if (layoutGroup is HorizontalLayoutGroup hlg)
                    {
                        availableWidth -= hlg.spacing * (current.parent.childCount - 1);
                    }
                }
                current = current.parent;
            }
            return availableWidth;
        }

        private string WrapText(string text, TMP_Text tmp, float maxWidth)
        {
            if (string.IsNullOrEmpty(text) || tmp == null) return text;
            if (tmp.GetPreferredValues(text).x <= maxWidth) return text;

            StringBuilder sb = new();
            string[] words = text.Split(' ');
            string line = "";

            foreach (var word in words)
            {
                string testLine = string.IsNullOrEmpty(line) ? word : $"{line} {word}";
                if (tmp.GetPreferredValues(testLine).x > maxWidth && !string.IsNullOrEmpty(line))
                {
                    sb.AppendLine(line);
                    line = word;
                }
                else line = testLine;
            }
            sb.Append(line);
            return sb.ToString();
        }
    }
}