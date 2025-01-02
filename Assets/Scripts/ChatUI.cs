using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ChatUI : MonoBehaviour
{
    //public TMP_Text chatText;
    public TextMeshProUGUI chatText;
    public UnityEvent onSkipKeyPressed;
    public string colorHex = ColorUtility.ToHtmlStringRGB(Color.red);
    private string originalText;
    
    private void Start()
    {
        originalText = "제대로 안그리면 죽여버리겠습니다!\r\n\r\n키워드 : "+GameManager.Instance.keyWord;
        KeyWordChange(originalText);
    }

    private void Update()
    {
        if (Input.GetKeyDown(GameManager.Instance.interactKey)||Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.Space))
        {
            onSkipKeyPressed?.Invoke();
        }

        if (LLMAPIManager.Instance.isCatch)
        {
            AnswerChange();
        }
    }

    public void KeyWordChange(string message)
    {
        string Message = message.Replace(GameManager.Instance.keyWord, $"<color=#{colorHex}>{GameManager.Instance.keyWord}</color>");
        chatText.text = Message;
    }

    private void AnswerChange()
    {
        KeyWordChange(LLMAPIManager.Instance.apiResponse);
    }

    public void OnClickCloseButton()
    {
        Time.timeScale = 1;
        CameraManager.Instance.ActivateMainCamera();
        gameObject.SetActive(false);
    }
}
