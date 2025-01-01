using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    // 싱글톤 패턴 적용
    public static GameManager Instance;
    
    [Header("GameSettings")]
    public Transform player;
    public KeyCode interactKey;

    [SerializeField] private string KeyWord;
    public string keyWord
    {
        get => KeyWord; set => KeyWord = value;
    }
    
    [SerializeField] private List<string> KeyWordList = new List<string>();
    
    public event Action OnFlagTrue; // 이벤트 선언
    public UnityEvent onFlagTrue; // UnityEvent 선언
    
    [Header("Flags")]
    [SerializeField] private bool ClearFlag;
    public bool clearFlag
    {
        get => ClearFlag;
        set
        {
            if (!ClearFlag && value) // 값이 false에서 true로 변경될 때만 실행
            {
                ClearFlag = value;
                OnFlagTrue?.Invoke(); // 이벤트 발생
            }
            else
            {
                ClearFlag = value;
            }
        }
    }
    [SerializeField] private bool FailFlag;
    public bool failFlag
    {
        get => FailFlag;
        set
        {
            if (!FailFlag && value) // 값이 false에서 true로 변경될 때만 실행
            {
                FailFlag = value;
                onFlagTrue?.Invoke(); // UnityEvent 호출
            }
            else
            {
                FailFlag = value;
            }
        }
    }
    
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
        
        player = GameObject.FindGameObjectWithTag("Player").transform;
        interactKey = KeyCode.F;
        OnFlagTrue += HandleFlagTrue; // 이벤트 구독
        
        KeyWordListSetting();
        InitKeyWord();
    }

    private void HandleFlagTrue()
    {
        Debug.Log("GameClear");
        Time.timeScale = 0;
        //UIManager.Instance.UIList[4].gameObject.SetActive(true);    
    }

    void OnDestroy()
    {
        OnFlagTrue -= HandleFlagTrue; // 이벤트 구독 해제
    }

    public void InitKeyWord()
    {
        keyWord = KeyWordList[Random.Range(0, KeyWordList.Count)];
    }

    private void KeyWordListSetting()
    {
        KeyWordList.Add("곰");
        KeyWordList.Add("나무");
        KeyWordList.Add("사람");
        KeyWordList.Add("사과");
        KeyWordList.Add("집");
    }

    public void GameOver()
    {
        Debug.Log("GameOver");
        Time.timeScale = 0;
        //UIManager.Instance.UIList[4].gameObject.SetActive(true);
    }
}
