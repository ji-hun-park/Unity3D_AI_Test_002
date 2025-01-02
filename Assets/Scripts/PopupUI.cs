using System;
using TMPro;
using UnityEngine;

public class PopupUI : MonoBehaviour
{
    public TMP_Text popupText;

    private void OnEnable()
    {
        popupText.text = UIManager.Instance.popupMessage;
    }
}
