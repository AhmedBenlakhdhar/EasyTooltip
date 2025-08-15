namespace PixelAdder.SimpleTooltip
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    /// <summary>
    /// A simple "view" component that displays tooltip data.
    /// Only responsible for rendering — no logic for when/where it appears.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class Tooltip : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject header;
        [SerializeField] public TextMeshProUGUI titleField; // Made public for the manager
        [SerializeField] public TextMeshProUGUI contentField; // Made public for the manager
        [SerializeField] private Image iconField;

        /// <summary>
        /// Sets the tooltip UI elements.
        /// </summary>
        public void SetText(
            string content,
            string title = "",
            Sprite icon = null,
            Color? titleColor = null,
            Color? iconColor = null)
        {
            bool hasTitle = !string.IsNullOrEmpty(title);
            if (titleField)
            {
                titleField.gameObject.SetActive(hasTitle);
                if (hasTitle)
                {
                    titleField.text = title;
                    titleField.color = titleColor ?? Color.white;
                }
            }

            if (contentField)
            {
                contentField.text = content ?? string.Empty;
            }

            bool hasIcon = icon != null;
            if (iconField)
            {
                iconField.gameObject.SetActive(hasIcon);
                if (hasIcon)
                {
                    iconField.sprite = icon;
                    iconField.color = iconColor ?? Color.white;
                }
            }

            if (header)
            {
                header.SetActive(hasTitle || hasIcon);
            }
        }
    }
}