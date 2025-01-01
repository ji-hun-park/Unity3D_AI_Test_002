using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // 싱글톤 패턴 적용
    public static UIManager Instance;
    
    [SerializeField]private GameObject canvas;
    public List<RectTransform> UIList = new List<RectTransform>();
    
    /*[Header("Scripts")]
    public AlertUI altUI;
    public NPCUI npcUI;
    public ScrollUI scrollUI;*/
    
    void Awake()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        if (canvas != null)
        {
            FindChatUI();
            FindInteractUI();
            FindScreenUI();
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
}
