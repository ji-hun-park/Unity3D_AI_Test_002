using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // 싱글톤 패턴 적용
    public static CameraManager Instance;
    
    public Camera mainCamera;  // 메인 카메라
    public Camera subCamera;   // 서브 카메라
    
    void Start()
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
        
        // 메인 카메라 활성화, 서브 카메라 비활성화
        ActivateMainCamera();
    }

    public void ActivateMainCamera()
    {
        mainCamera.enabled = true;
        subCamera.enabled = false;
    }

    public void ActivateSubCamera()
    {
        subCamera.enabled = true;
        mainCamera.enabled = false;
    }
}
