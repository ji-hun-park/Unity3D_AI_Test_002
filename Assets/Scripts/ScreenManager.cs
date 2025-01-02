using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class ScreenManager : MonoBehaviour
{
    public RawImage targetImage; // 그림을 그릴 UI Image (RawImage 사용 권장)
    public int textureWidth = 512; // 캔버스 너비
    public int textureHeight = 512; // 캔버스 높이
    public Color drawColor = Color.black; // 브러쉬 색상
    public float brushSize = 5f; // 브러시 크기
    
    public Button blackButton;  // UI 버튼 (검정색)
    public Button whiteButton;  // UI 버튼 (하얀색)
    public Button redButton;    // UI 버튼 (빨간색)
    public Button blueButton;   // UI 버튼 (파란색)
    public Button greenButton;  // UI 버튼 (녹색)
    public Slider brushSizeSlider; // 브러시 크기 조절 슬라이더
    
    private Texture2D drawTexture;
    private RectTransform rectTransform;

    void Start()
    {
        InitCanvas();
    }

    void Update()
    {
        // 마우스 입력 감지
        if (Input.GetMouseButton(0)) // 좌클릭
        {
            Vector2 localPoint;
            // UI 좌표로 변환
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localPoint))
            {
                // UI Image 좌표를 텍스처 좌표로 변환
                float x = (localPoint.x + rectTransform.rect.width / 2) / rectTransform.rect.width * textureWidth;
                float y = (localPoint.y + rectTransform.rect.height / 2) / rectTransform.rect.height * textureHeight;

                DrawAt((int)x, (int)y); // 텍스처에 그리기
            }
        }

        // 저장 단축키 감지
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveTexture();
        }
        
        // 불러오기 단축키 감지
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadTexture();
        }
    }

    public void InitCanvas()
    {
        // 브러쉬 색 초기화
        drawColor = Color.black;
        
        // 새 텍스처 생성
        drawTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        for (int x = 0; x < textureWidth; x++)
        {
            for (int y = 0; y < textureHeight; y++)
            {
                drawTexture.SetPixel(x, y, Color.white); // 초기화
            }
        }
        drawTexture.Apply();

        // 텍스처를 RawImage에 연결
        targetImage.texture = drawTexture;
        rectTransform = targetImage.GetComponent<RectTransform>();
        
        // 버튼 이벤트 연결
        if (blackButton != null) blackButton.onClick.AddListener(() => ChangeColor(Color.black));
        if (whiteButton != null) whiteButton.onClick.AddListener(() => ChangeColor(Color.white));
        if (redButton != null) redButton.onClick.AddListener(() => ChangeColor(Color.red));
        if (blueButton != null) blueButton.onClick.AddListener(() => ChangeColor(Color.blue));
        if (greenButton != null) greenButton.onClick.AddListener(() => ChangeColor(Color.green));

        // 슬라이더 이벤트 연결
        if (brushSizeSlider != null)
        {
            brushSizeSlider.minValue = 1f;
            brushSizeSlider.maxValue = 20f;
            brushSizeSlider.value = brushSize;
            brushSizeSlider.onValueChanged.AddListener(ChangeBrushSize);
        }
    }
    
    void DrawAt(int x, int y)
    {
        // 브러시 영역에 색상 적용
        for (int i = -Mathf.FloorToInt(brushSize); i < Mathf.CeilToInt(brushSize); i++)
        {
            for (int j = -Mathf.FloorToInt(brushSize); j < Mathf.CeilToInt(brushSize); j++)
            {
                int px = x + i;
                int py = y + j;

                // 텍스처 범위를 벗어나지 않도록 제한
                if (px >= 0 && px < textureWidth && py >= 0 && py < textureHeight)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(px, py));
                    if (distance <= brushSize)
                    {
                        drawTexture.SetPixel(px, py, drawColor);
                    }
                }
            }
        }
        drawTexture.Apply(); // 변경 사항 적용
    }

    void SaveTexture()
    {
        // 텍스처를 PNG 데이터로 변환
        byte[] pngData = drawTexture.EncodeToPNG();
        if (pngData != null)
        {
            // 파일 저장 경로 설정
            string filePath = Path.Combine(Application.persistentDataPath, "SavedDrawing.png");

            // PNG 데이터를 파일로 저장
            File.WriteAllBytes(filePath, pngData);
            
            // 저장 알림 메시지 띄우기
            UIManager.Instance.RunPopupCoroutine("그림이 저장되었습니다!");
            
            // 저장 완료 로그
            Debug.Log($"그림이 저장되었습니다: {filePath}");
        }
        else
        {
            Debug.LogError("PNG 데이터를 생성하는 데 실패했습니다.");
        }
    }
    
    void LoadTexture()
    {
        // 파일 경로 설정
        string filePath = Path.Combine(Application.persistentDataPath, "SavedDrawing.png");

        if (File.Exists(filePath))
        {
            // 저장된 파일 읽기
            byte[] pngData = File.ReadAllBytes(filePath);

            // 텍스처에 로드
            if (drawTexture != null)
            {
                drawTexture.LoadImage(pngData);
                drawTexture.Apply();
                UIManager.Instance.RunPopupCoroutine("그림이 성공적으로 불러와졌습니다.");
                Debug.Log("그림이 성공적으로 불러와졌습니다.");
            }
        }
        else
        {
            Debug.LogWarning($"저장된 그림이 없습니다: {filePath}");
            UIManager.Instance.RunPopupCoroutine("저장된 그림이 없습니다!");
        }
    }

    public void OnClickSendButton()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "SavedDrawing.png");

        if (File.Exists(filePath))
        {
            LLMAPIManager.Instance.SendRequest();
            UIManager.Instance.RunPopupCoroutine("그림이 전송됐습니다!");
        }
        else
        {
            Debug.LogWarning("저장된 그림이 없습니다!");
            UIManager.Instance.RunPopupCoroutine("저장된 그림이 없습니다!");
        }
    }
    
    void ChangeColor(Color newColor)
    {
        drawColor = newColor;
        Debug.Log($"브러쉬 색상이 변경되었습니다: {newColor}");
    }

    void ChangeBrushSize(float newSize)
    {
        brushSize = newSize;
        Debug.Log($"브러쉬 크기가 변경되었습니다: {newSize}");
    }

    public void OnClickLoadButton()
    {
        LoadTexture();
    }
    
    public void OnClickSaveButton()
    {
        SaveTexture();
    }

    public void OnClickReturnButton()
    {
        gameObject.SetActive(false);
    }
}
