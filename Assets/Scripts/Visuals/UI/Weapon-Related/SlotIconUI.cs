using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotIconUI : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;

    public void Initialize(Color color)
    {
        backgroundImage.color = color;
    }
}
