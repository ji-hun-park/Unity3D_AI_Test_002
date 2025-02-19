using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ScreenUI : MonoBehaviour
{
    [SerializeField] private Image screenImage;
    [SerializeField] private string spriteAddress; // Addressables 리소스 주소
    public UnityEvent onSkipKeyPressed2;
    
    private void Start()
    {
        //screenImage = UIManager.Instance.FindChildByName(transform ,"Draw").GetComponent<Image>();
        LoadSprite();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            onSkipKeyPressed2?.Invoke();
        }
    }

    public void LoadSprite()
    {
        // Addressables에서 스프라이트 로드
        Addressables.LoadAssetAsync<Texture2D>(spriteAddress).Completed += OnSpriteLoaded;
    }
    
    private void OnSpriteLoaded(AsyncOperationHandle<Texture2D> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            // 스프라이트 로드 성공
            Texture2D texture2D = handle.Result;
            Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
            screenImage.sprite = sprite;
            Debug.Log("Sprite loaded and applied successfully!");
        }
        else
        {
            // 로드 실패
            Debug.LogError($"Failed to load sprite at address: {spriteAddress}");
        }
    }

    public void OnClickCanvasButton()
    {
        UIManager.Instance.UIList[3].gameObject.SetActive(true);
    }

    public void OnClickSkipButton()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
    
    private void OnDestroy()
    {
        //Addressables.Release(spriteAddress);
    }
}
