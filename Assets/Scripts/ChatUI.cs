using UnityEngine;

public class ChatUI : MonoBehaviour
{
    public void OnClickCloseButton()
    {
        Time.timeScale = 1;
        CameraManager.Instance.ActivateMainCamera();
        gameObject.SetActive(false);
    }
}
