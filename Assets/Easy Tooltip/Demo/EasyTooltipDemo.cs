namespace PixeLadder.EasyTooltip.Demo
{
    using UnityEngine;
    using UnityEngine.UI;
    using PixeLadder.EasyTooltip;

    /// <summary>
    /// Controls the "Logic" panel in the demo scene. 
    /// Demonstrates procedural generation and event handling.
    /// </summary>
    [AddComponentMenu("PixeLadder/Easy Tooltip/Demo/Demo Controller")]
    public class EasyTooltipDemo : MonoBehaviour
    {
        #region References
        [Header("Procedural Example")]
        [Tooltip("The button in Panel D labeled 'Code Generated'.")]
        [SerializeField] private GameObject proceduralButton;

        [Tooltip("Icon to use for the procedural tooltip.")]
        [SerializeField] private Sprite surpriseIcon;

        [Header("Event Example")]
        [Tooltip("The Image component of the button in Panel D labeled 'Hover Effects'.")]
        [SerializeField] private Image eventButtonImage;
        [SerializeField] private Color highlightColor = Color.green;
        private Color originalColor;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            // 1. Setup Procedural Tooltip
            if (proceduralButton != null)
            {
                SetupProceduralTooltip();
            }

            // 2. Store original color for the Event example
            if (eventButtonImage != null)
            {
                originalColor = eventButtonImage.color;
            }
        }
        #endregion

        #region Logic
        private void SetupProceduralTooltip()
        {
            // Static API call to add a tooltip at runtime
            var trigger = TooltipTrigger.AddTooltip(
                proceduralButton,
                "This tooltip was created entirely by C# code at runtime!\n<color=yellow>No Inspector work required.</color>",
                "Procedural Magic",
                surpriseIcon
            );

            // Optional: We can even style it via code!
            if (trigger != null)
            {
                trigger.ShowOutline = true;
                trigger.OutlineColor = Color.cyan;
                trigger.HoverDelay = 0.5f;

                // Position Logic: Fixed to Top Left
                trigger.PositionMode = TooltipPositionMode.Fixed;
                trigger.AnchorPosition = TooltipAnchor.TopLeft;
            }
        }

        // --- Public Methods for UnityEvents ---

        /// <summary>
        /// Assign this to the 'OnTooltipShow' event on the 'Hover Effects' Trigger.
        /// </summary>
        public void OnHoverStart()
        {
            if (eventButtonImage != null)
            {
                eventButtonImage.color = highlightColor;
            }
        }

        /// <summary>
        /// Assign this to the 'OnTooltipHide' event on the 'Hover Effects' Trigger.
        /// </summary>
        public void OnHoverEnd()
        {
            if (eventButtonImage != null)
            {
                eventButtonImage.color = originalColor;
            }
        }
        #endregion
    }
}