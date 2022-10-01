using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TextTooltipUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private LayoutElement layoutElement;
    

    [Header("Data")]
    [SerializeField] private int characterWrapLimit;

    public static TextTooltipUI instance;
    private void Awake()
    {
        // Singleton Logic
        if (TextTooltipUI.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        FollowMouse();

        if (Application.isEditor) {
            Resize();
        }
    }

    public void Show(string header, string description)
    {
        // Update text
        headerText.text = header;
        descriptionText.text = description;

        // See if you need to resize window
        Resize();

        // Show
        canvasGroup.alpha = 1f;
    }

    public void Hide()
    {
        // Hide
        canvasGroup.alpha = 0f;
    }

    private void Resize() {
        int headerLength = headerText.text.Length;
        int descriptionLength = descriptionText.text.Length;

        layoutElement.enabled = (headerLength > characterWrapLimit || descriptionLength > characterWrapLimit);
    }

    private void FollowMouse() {
        // Update position
        Vector2 position = Input.mousePosition;
        Vector2 adjustedPosition = Camera.main.ScreenToWorldPoint(position);
        transform.position = adjustedPosition;

        // Update pivot
        float pivotX = position.x / Screen.width;
        float pivotY = position.y / Screen.height;

        rectTransform.pivot = new Vector2(pivotX, pivotY);

        
    }

}
