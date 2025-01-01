using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Screen 레이어에 접촉했는지 확인
        if (other.gameObject.layer == LayerMask.NameToLayer("Screen"))
        {
            Debug.Log("Player has interacted with Screen object!");
            UIManager.Instance.UIList[1].gameObject.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKey(GameManager.Instance.interactKey))
        {
            UIManager.Instance.UIList[1].gameObject.SetActive(false);
            InteractWithScreen(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        UIManager.Instance.UIList[1].gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            UIManager.Instance.UIList[1].gameObject.SetActive(true);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (Input.GetKey(GameManager.Instance.interactKey))
        {
            CameraManager.Instance.ActivateSubCamera();
            Time.timeScale = 0;
            UIManager.Instance.UIList[1].gameObject.SetActive(false);
            UIManager.Instance.UIList[0].gameObject.SetActive(true);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        UIManager.Instance.UIList[1].gameObject.SetActive(false);
    }

    private void InteractWithScreen(GameObject screen)
    {
        // 상호작용 로직
        UIManager.Instance.UIList[2].gameObject.SetActive(true);
        Debug.Log($"Interacting with {screen.name}");
    }
}
