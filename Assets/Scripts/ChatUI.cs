using System;
using TMPro;
using UnityEngine;

public class ChatUI : MonoBehaviour
{
    //public TMP_Text chatText;
    public TextMeshProUGUI chatText;

    private void Start()
    {
        chatText.text = "제대로 안그리면 죽여버리겠다!\r\n\r\n키워드 : "+GameManager.Instance.keyWord;
    }

    public void OnClickCloseButton()
    {
        Time.timeScale = 1;
        CameraManager.Instance.ActivateMainCamera();
        gameObject.SetActive(false);
    }
}
