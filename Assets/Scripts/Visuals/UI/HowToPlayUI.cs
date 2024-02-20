using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HowToPlayUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI messageLabel;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    [Header("Debug")]
    [SerializeField] private int index;

    [Header("Data")]
    [SerializeField][TextArea(10, 20)] private List<string> messages;

    private void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();

        if (messages == null || messages.Count == 0)
            throw new System.Exception("HELP MENU NOT PROPERLY SET.");

        index = 0;
    }

    public void Open()
    {
        // Reset index and set message
        index = 0;
        messageLabel.text = messages[index];

        HandleButtonState();

        // Show UI
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Close()
    {
        // Hide UI
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void NextMessage()
    {
        index++;
        messageLabel.text = messages[index];

        HandleButtonState();
    }

    public void PreviousMessage()
    {
        index--;
        messageLabel.text = messages[index];

        HandleButtonState();
    }

    private void HandleButtonState()
    {
        // If we are at the beginning of list
        if (index == 0)
        {
            prevButton.interactable = false;
        }
        else
        {
            prevButton.interactable = true;
        }

        // If we are at the end of the list
        if (index == messages.Count - 1)
        {
            nextButton.interactable = false;
        }
        else
        {
            nextButton.interactable = true;
        }
    }
}
