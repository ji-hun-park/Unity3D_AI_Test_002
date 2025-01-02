using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class LLMAPIManager : MonoBehaviour
{
    // 싱글톤 패턴 적용
    public static LLMAPIManager Instance;
    
    public string promptMessage = null;
    public string apiResponse = null;
    public bool isCatch;
    private int maxToken;
    private string apiUrl = null;
    private string apiKey = null;
    private string base64Image;
    
    [Serializable]
    private class ApiKeyData
    {
        public string apiKey;
    }

    private void Awake()
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
        
        string path = Path.Combine(Application.streamingAssetsPath, "config.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            apiKey = JsonUtility.FromJson<ApiKeyData>(json).apiKey;
        }

        maxToken = 1000;
        
        apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key=" + apiKey;   
    }

    public void SendRequest()
    {
        promptMessage = "인라인데이타로 주어진 이미지를 보고 설명해!";
        StartCoroutine(LLMAPIRequest(promptMessage, maxToken));
    }

    private void ImagetoInlineData()
    {
        // 파일 경로 설정
        string filePath = Path.Combine(Application.persistentDataPath, "SavedDrawing.png");

        if (File.Exists(filePath))
        {
            // 저장된 파일 읽기
            byte[] pngData = File.ReadAllBytes(filePath);
            base64Image = Convert.ToBase64String(pngData); // Base64로 인코딩
        }
    }

    private IEnumerator LLMAPIRequest(string prompt, int maxTokens)
    {
        // 이미지를 InlineData로 만들기
        ImagetoInlineData();
        
        // POST로 보내기 위해 JSON 형식 데이터로 만듬
        string jsonData = "{\"contents\":[{\"parts\":[{\"text\":\"" + prompt + "\"},{\"inlineData\": {\"mimeType\": \"image/png\",\"data\": \"" + base64Image + "\"}}]}], \"generationConfig\": {\"maxOutputTokens\": " + maxTokens + "}}";

        // UnityWebRequest 보내기 위해 필요한 것 들
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Header 작성
        request.SetRequestHeader("Content-Type", "application/json");

        // 리퀘스트 보냄
        yield return request.SendWebRequest();

        // 성공하면 응답받고 텍스트 파싱
        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseText = request.downloadHandler.text;
            ParseResponse(responseText);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
    }

    private void ParseResponse(string jsonResponse)
    {
        // JSON 파싱
        JObject response = JObject.Parse(jsonResponse);

        // candidates[0].content.parts[0].text 파싱
        string modelResponse = response["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

        if (modelResponse != null)
        {
            Debug.Log("Model Response: " + modelResponse);
            apiResponse = modelResponse;
            isCatch = true;
            UIManager.Instance.UIList[0].GetComponent<ChatUI>().OneShot = true;
        }
        else
        {
            Debug.LogError("Could not parse the response.");
        }
    }
}
