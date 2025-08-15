namespace PixelAdder.SimpleTooltip
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// Attach to any UI element to show a tooltip on hover.
    /// Ensures that a TooltipManager exists in the scene.
    /// </summary>
    [ExecuteAlways]
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private const string TOOLTIP_PREFAB_PATH = "TooltipManager";

        [Header("Tooltip Content")]
        [SerializeField] private Sprite icon;
        [SerializeField] private string title;
        [TextArea(3, 10)]
        [SerializeField] private string content;

        [Header("Custom Styles")]
        [SerializeField] private Color titleColor = Color.white;
        [SerializeField] private Color iconColor = Color.white;

        [Header("Settings")]
        [SerializeField, Min(0f)] private float hoverDelay = 0.5f;

        private void Reset() => EnsureManagerExists();
        private void OnEnable() { if (Application.isPlaying) EnsureManagerExists(); }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (TooltipManager.Instance)
            {
                TooltipManager.Instance.ShowTooltip(content, title, icon, titleColor, iconColor, hoverDelay);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipManager.Instance?.HideTooltip();
        }

        private static void EnsureManagerExists()
        {
            if (TooltipManager.Instance != null || FindFirstObjectByType<TooltipManager>() != null)
                return;

            GameObject prefab = Resources.Load<GameObject>(TOOLTIP_PREFAB_PATH);
            if (!prefab)
            {
                Debug.LogError($"Tooltip System: Missing prefab at 'Assets/Resources/{TOOLTIP_PREFAB_PATH}.prefab'");
                return;
            }

            GameObject instance = Instantiate(prefab);
            instance.name = "TooltipManager";
            Debug.Log("TooltipManager created automatically.", instance);
        }
    }
}