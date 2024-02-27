using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonAudio : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(PlayClickAudio);
    }

    private void PlayClickAudio()
    {
        AudioManager.instance.PlaySFX("button_click");
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
            AudioManager.instance.PlaySFX("button_hover");
    }
}
