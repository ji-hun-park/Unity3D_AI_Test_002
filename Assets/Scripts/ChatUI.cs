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
    public bool isCorrect = false;
    public bool isIncorrect = false;
    public bool OneShot = true;
    
    private void Start()
    {
        OneShot = true;
        InitKeyWord();
    }

    private void Update()
    {
        if (Input.GetKeyDown(GameManager.Instance.interactKey)||Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.Space))
        {
            onSkipKeyPressed?.Invoke();
        }

        if (LLMAPIManager.Instance.isCatch && LLMAPIManager.Instance.apiResponse != "" && OneShot)
        {
            CheckAnswer();
            AnswerChange();
            OneShot = false;
        }
    }

    public void InitKeyWord()
    {
        originalText = "제대로 안그리면 죽여버리겠습니다!\r\n캔버스에 그림 그리고 저장 후 전송하세요!\r\n단축키 : 저장(S), 불러오기(L)\r\n\r\n키워드 : "+GameManager.Instance.keyWord;
        KeyWordChange(originalText);
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
        //if (Regex.IsMatch(LLMAPIManager.Instance.apiResponse, $@"(^|[^가-힣]){Regex.Escape(GameManager.Instance.keyWord)}([^가-힣]|$)"))
        if (Regex.IsMatch(LLMAPIManager.Instance.apiResponse, GameManager.Instance.keyWord))
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
            isIncorrect = true;
            GameManager.Instance.StartAction();
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
            else if (isIncorrect)
            {
                GameManager.Instance.failFlag = true;
            }
        }
        gameObject.SetActive(false);
    }
}
