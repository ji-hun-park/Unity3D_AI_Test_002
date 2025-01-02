using System;
using System.Text.RegularExpressions;
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
    private bool isCorrect = false;
    
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
            CheckAnswer();
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

    private void CheckAnswer()
    {
        // 특정 단어가 텍스트에 포함되어 있는지 확인
        if (Regex.IsMatch(LLMAPIManager.Instance.apiResponse, $@"(^|[^가-힣]){Regex.Escape(GameManager.Instance.keyWord)}([^가-힣]|$)"))
        {
            // 단어를 리치 텍스트 태그로 감쌈
            /*string coloredText = Regex.Replace(
                originalText,
                $@"\b{Regex.Escape(wordToColor)}\b",
                $"<color=#{colorHex}>{wordToColor}</color>"
            );*/
            
            isCorrect = true;
        }
        else
        {
            // 키워드가 없는 경우 다른 기능 수행
            isCorrect = false;
        }
    }

    public void OnClickCloseButton()
    {
        Time.timeScale = 1;
        CameraManager.Instance.ActivateMainCamera();
        if (LLMAPIManager.Instance.isCatch)
        {
            if (isCorrect)
            {
                GameManager.Instance.clearFlag = true;
            }
            else
            {
                GameManager.Instance.failFlag = true;
            }
        }
        gameObject.SetActive(false);
    }
}
