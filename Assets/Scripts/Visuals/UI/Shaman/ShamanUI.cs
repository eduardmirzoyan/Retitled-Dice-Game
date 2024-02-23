using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShamanUI : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Dynamic Data")]
    [SerializeField] private bool isOpen;

    private void Start()
    {

    }

    private void Show()
    {
        if (!isOpen)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            isOpen = true;
        }
    }

    private void Hide()
    {
        if (isOpen)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            isOpen = false;
        }
    }
}
