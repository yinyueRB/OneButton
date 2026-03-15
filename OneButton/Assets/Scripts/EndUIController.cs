using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndUIController : MonoBehaviour
{
    [Header("场景设置")]
    public string menuSceneName = "Menu";

    [Header("音效设置")]
    public AudioSource uiAudioSource;
    public AudioClip buttonClickClip;

    [Header("延迟设置")]
    public float soundDelay = 0.15f;

    void Start()
    {
        if (uiAudioSource == null)
        {
            Debug.LogWarning("UI AudioSource 未设置，按钮音效无法播放");
        }
    }

    private void PlayButtonClickSound()
    {
        if (uiAudioSource != null && buttonClickClip != null)
        {
            uiAudioSource.PlayOneShot(buttonClickClip);
            Debug.Log("按钮音效播放成功");
        }
        else
        {
            Debug.LogWarning("AudioSource 或 AudioClip 未设置");
        }
    }

    // 返回主菜单按钮
    public void OnReturnToMenuButtonClick()
    {
        PlayButtonClickSound();

        Invoke("LoadMenuScene", soundDelay);
    }

    private void LoadMenuScene()
    {
        try
        {
            SceneManager.LoadScene(menuSceneName);
            Debug.Log("返回主菜单：" + menuSceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError("加载菜单失败：" + e.Message);
        }
    }
}

