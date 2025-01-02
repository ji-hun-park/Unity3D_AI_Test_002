using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // 싱글톤 패턴 적용
    public static UIManager Instance;
    
    [SerializeField]private GameObject canvas;
    public List<RectTransform> UIList = new List<RectTransform>();
    public string popupMessage;
    
    /*[Header("Scripts")]
    public AlertUI altUI;
    public NPCUI npcUI;
    public ScrollUI scrollUI;*/
    
    void Awake()
    {
        // Instance 존재 유무에 따라 게임 매니저 파괴 여부 정함
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 기존에 존재 안하면 이걸로 대체하고 파괴하지 않기
        }
        else
        {
            Destroy(gameObject); // 기존에 존재하면 자신파괴
        }
        
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        if (canvas != null)
        {
            FindChatUI();
            FindInteractUI();
            FindScreenUI();
            FindCanvasUI();
            FindPopupUI();
            FindOverUI();
        }
    }

    private void FindChatUI()
    {
        FindUI("ChatUI");
    }
    
    private void FindInteractUI()
    {
        FindUI("InteractUI");
    }
    
    private void FindScreenUI()
    {
        FindUI("ScreenUI");
    }
    
    private void FindCanvasUI()
    {
        FindUI("CanvasUI");
    }

    private void FindPopupUI()
    {
        FindUI("PopupUI");
    }
    
    private void FindOverUI()
    {
        FindUI("OverUI");
    }
    
    private void FindUI(string UIName)
    {
        Transform target = FindChildByName(canvas.transform, UIName);

        if (target != null)
        {
            Debug.Log("찾은 오브젝트: " + target.name);
            UIList.Add(target.GetComponent<RectTransform>());
        }
        else
        {
            Debug.Log("오브젝트를 찾을 수 없습니다.");
        }
    }
    
    public Transform FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                return child;
            }

            // 재귀적으로 자식 검색
            Transform result = FindChildByName(child, name);
            if (result != null)
            {
                return result;
            }
        }
        return null; // 찾지 못했을 경우 null 반환
    }

    public void RunPopupCoroutine(string MT)
    {
        StartCoroutine(PopupAlertMessage(MT));
    }
    
    private IEnumerator PopupAlertMessage(string PM)
    {
        popupMessage = PM;
        UIList[4].gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1f); // Time.timeScale과 상관없이 n초 대기
        UIList[4].gameObject.SetActive(false);
    }
}
