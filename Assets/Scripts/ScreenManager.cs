using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class ScreenManager : MonoBehaviour
{
    public enum DrawMode { Brush, Line, Rectangle, Circle }
    
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
    public Button brushButton;
    public Button lineButton;
    public Button rectButton;
    public Button circleButton;
    
    private Texture2D drawTexture;
    private RectTransform rectTransform;
    private DrawMode currentMode = DrawMode.Brush;

    private Vector2 startPoint; // 시작 점
    private bool isDrawing = false;

    void Start()
    {
        InitCanvas();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localPoint))
            {
                float x = (localPoint.x + rectTransform.rect.width / 2) / rectTransform.rect.width * textureWidth;
                float y = (localPoint.y + rectTransform.rect.height / 2) / rectTransform.rect.height * textureHeight;
                startPoint = new Vector2(x, y);
                isDrawing = true;

                if (currentMode == DrawMode.Brush)
                {
                    DrawBrush((int)x, (int)y);
                }
            }
        }
        else if (Input.GetMouseButton(0) && isDrawing && currentMode == DrawMode.Brush)
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localPoint))
            {
                float x = (localPoint.x + rectTransform.rect.width / 2) / rectTransform.rect.width * textureWidth;
                float y = (localPoint.y + rectTransform.rect.height / 2) / rectTransform.rect.height * textureHeight;
                DrawBrush((int)x, (int)y);
            }
        }
        else if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localPoint))
            {
                float x = (localPoint.x + rectTransform.rect.width / 2) / rectTransform.rect.width * textureWidth;
                float y = (localPoint.y + rectTransform.rect.height / 2) / rectTransform.rect.height * textureHeight;

                Vector2 endPoint = new Vector2(x, y);

                if (currentMode == DrawMode.Line) DrawLine(startPoint, endPoint);
                else if (currentMode == DrawMode.Rectangle) DrawRectangle(startPoint, endPoint);
                else if (currentMode == DrawMode.Circle) DrawCircle(startPoint, endPoint);

                isDrawing = false;
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
        
        // 전송 단축키 감지
        if (Input.GetKeyDown(KeyCode.T))
        {
            OnClickSendButton();
        }
        
        // 뒤로가기 단축키 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnClickReturnButton();
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
        if (brushButton != null) brushButton.onClick.AddListener(() => SetDrawMode(DrawMode.Brush));
        if (lineButton != null) lineButton.onClick.AddListener(() => SetDrawMode(DrawMode.Line));
        if (rectButton != null) rectButton.onClick.AddListener(() => SetDrawMode(DrawMode.Rectangle));
        if (circleButton != null) circleButton.onClick.AddListener(() => SetDrawMode(DrawMode.Circle));
        
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
    
    void DrawBrush(int x, int y)
    {
        for (int i = -Mathf.FloorToInt(brushSize); i < Mathf.CeilToInt(brushSize); i++)
        {
            for (int j = -Mathf.FloorToInt(brushSize); j < Mathf.CeilToInt(brushSize); j++)
            {
                int px = x + i;
                int py = y + j;

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
        drawTexture.Apply();
    }

    void DrawLine(Vector2 start, Vector2 end)
    {
        int x0 = (int)start.x;
        int y0 = (int)start.y;
        int x1 = (int)end.x;
        int y1 = (int)end.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            drawTexture.SetPixel(x0, y0, drawColor);

            if (x0 == x1 && y0 == y1) break;

            int e2 = err * 2;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
        drawTexture.Apply();
    }

    void DrawRectangle(Vector2 start, Vector2 end)
    {
        int xMin = Mathf.RoundToInt(Mathf.Min(start.x, end.x));
        int xMax = Mathf.RoundToInt(Mathf.Max(start.x, end.x));
        int yMin = Mathf.RoundToInt(Mathf.Min(start.y, end.y));
        int yMax = Mathf.RoundToInt(Mathf.Max(start.y, end.y));

        // 사각형의 네 변 그리기
        for (int x = xMin; x <= xMax; x++)
        {
            // 상단 변
            if (yMin >= 0 && yMin < textureHeight)
                drawTexture.SetPixel(x, yMin, drawColor);

            // 하단 변
            if (yMax >= 0 && yMax < textureHeight)
                drawTexture.SetPixel(x, yMax, drawColor);
        }

        for (int y = yMin; y <= yMax; y++)
        {
            // 왼쪽 변
            if (xMin >= 0 && xMin < textureWidth)
                drawTexture.SetPixel(xMin, y, drawColor);

            // 오른쪽 변
            if (xMax >= 0 && xMax < textureWidth)
                drawTexture.SetPixel(xMax, y, drawColor);
        }

        drawTexture.Apply(); // 변경 사항 적용
    }

    void DrawCircle(Vector2 start, Vector2 end)
    {
        float radius = Vector2.Distance(start, end) / 2;
        Vector2 center = (start + end) / 2;

        for (int x = 0; x < textureWidth; x++)
        {
            for (int y = 0; y < textureHeight; y++)
            {
                if (Vector2.Distance(new Vector2(x, y), center) <= radius)
                {
                    drawTexture.SetPixel(x, y, drawColor);
                }
            }
        }
        drawTexture.Apply();
    }

    void SetDrawMode(DrawMode mode)
    {
        currentMode = mode;
        Debug.Log($"모드 변경: {mode}");
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
