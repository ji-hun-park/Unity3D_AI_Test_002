using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera mainCamera;  // 메인 카메라
    public Camera subCamera;   // 서브 카메라

    void Start()
    {
        // 메인 카메라 활성화, 서브 카메라 비활성화
        ActivateMainCamera();
    }

    void Update()
    {
        // 'C' 키로 카메라 전환
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (mainCamera.enabled)
            {
                ActivateSubCamera();
            }
            else
            {
                ActivateMainCamera();
            }
        }
    }

    void ActivateMainCamera()
    {
        mainCamera.enabled = true;
        subCamera.enabled = false;
    }

    void ActivateSubCamera()
    {
        mainCamera.enabled = false;
        subCamera.enabled = true;
    }
}
