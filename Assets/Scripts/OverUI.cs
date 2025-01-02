using TMPro;
using UnityEngine;

public class OverUI : MonoBehaviour
{
    public TMP_Text overText;
   
    public void OnClickRestartButton()
    {
        Debug.Log("Game Restart");
        Time.timeScale = 1;
        GameManager.Instance.animator.ResetTrigger("Melee Set");
        GameManager.Instance.animator.Play("Idle"); // Idle 상태로 전환
        LLMAPIManager.Instance.isCatch = false;
        LLMAPIManager.Instance.apiResponse = "";
        GameManager.Instance.clearFlag = false;
        GameManager.Instance.failFlag = false;
        GameManager.Instance.InitKeyWord();
        UIManager.Instance.UIList[0].GetComponent<ChatUI>().InitKeyWord();
        UIManager.Instance.UIList[0].GetComponent<ChatUI>().isCorrect = false;
        UIManager.Instance.UIList[0].GetComponent<ChatUI>().isIncorrect = false;
        UIManager.Instance.UIList[0].GetComponent<ChatUI>().OneShot = true;
        UIManager.Instance.UIList[3].GetComponent<ScreenManager>().InitCanvas();
        gameObject.SetActive(false);
    }
    
    public void OnClickEndButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
