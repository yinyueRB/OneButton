using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    [Header("UI�������")]
    public GameObject developerPanel;
    public string gameSceneName = "GameScene";

    [Header("��Ч����")]
    public AudioSource uiAudioSource;
    public AudioClip buttonClickClip;
    [Header("�ӳ�����")]
    public float soundDelay = 0.15f; // ��Ч�����ӳ٣��㹻��ť��Ч�����꣩

    void Start()
    {
        if (developerPanel != null)
        {
            developerPanel.SetActive(false);
        }

        if (uiAudioSource == null)
        {
            //Debug.LogWarning("δ��ֵUI��ƵԴ����Ч���޷����ţ�");
        }
    }

    private void PlayButtonClickSound()
    {
        if (uiAudioSource != null && buttonClickClip != null)
        {
            uiAudioSource.PlayOneShot(buttonClickClip);
            //Debug.Log("��Ч���ųɹ���");
        }
        else
        {
            //Debug.LogError("��Ч����ʧ�ܣ�AudioSource��AudioClipΪ�գ�");
        }
    }

    // ��ʼ��Ϸ���ӳټ��س�����ȷ����Ч������
    public void OnStartGameButtonClick()
    {
        PlayButtonClickSound(); // �Ȳ�����Ч
        // �ӳ�soundDelay�����س���
        Invoke("LoadGameScene", soundDelay);
    }

    // �˳���Ϸ���ӳ��˳���ȷ����Ч������
    public void OnQuitGameButtonClick()
    {
        PlayButtonClickSound(); // �Ȳ�����Ч
        // �ӳ�soundDelay����˳�
        Invoke("QuitGame", soundDelay);
    }

    // ������������ԭ���߼�����
    public void ShowDeveloperPanel()
    {
        PlayButtonClickSound();
        if (developerPanel != null)
        {
            developerPanel.SetActive(true);
        }
    }

    public void HideDeveloperPanel()
    {
        PlayButtonClickSound();
        if (developerPanel != null)
        {
            developerPanel.SetActive(false);
        }
    }

    // ��װ���������߼������ӳٵ��ã�
    private void LoadGameScene()
    {
        try
        {
            SceneManager.LoadScene(gameSceneName);
            //Debug.Log("��ʼ��Ϸ�����س�����" + gameSceneName);
        }
        catch (System.Exception e)
        {
            //Debug.LogError("���س���ʧ�ܣ�" + e.Message);
        }
    }

    // ��װ�˳��߼������ӳٵ��ã�
    private void QuitGame()
    {
        //Debug.Log("�˳���Ϸ");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}