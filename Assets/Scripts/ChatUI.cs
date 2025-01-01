using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ChatUI : MonoBehaviour
{
    //public TMP_Text chatText;
    public TextMeshProUGUI chatText;
    public UnityEvent onSkipKeyPressed;

    private void Start()
    {
        chatText.text = "제대로 안그리면 죽여버리겠다!\r\n\r\n키워드 : "+GameManager.Instance.keyWord;
    }

    private void Update()
    {
        if (Input.GetKeyDown(GameManager.Instance.interactKey)||Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.Space))
        {
            onSkipKeyPressed?.Invoke();
        }
    }

    public void OnClickCloseButton()
    {
        Time.timeScale = 1;
        CameraManager.Instance.ActivateMainCamera();
        gameObject.SetActive(false);
    }
}
