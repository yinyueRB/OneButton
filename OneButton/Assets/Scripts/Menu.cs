using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // 引用 UI 面板（需要在 Inspector 面板中赋值）
    public GameObject developerPanel;
    // 可选：引用文本组件用于显示开发者信息
    public TextMeshProUGUI developerInfoText;

    public void StartGame()
    {
        SceneManager.LoadScene(0);
        
    }

    // 退出游戏
    public void ExitGame()
    {
        // 编辑器中退出不会生效，添加日志提示
#if UNITY_EDITOR
        Debug.Log("退出游戏（编辑器模式下仅为日志提示）");
#else
        Application.Quit();
#endif
    }
   
    // 显示开发者信息UI的核心函数
    public void ShowDeveloperUI()
    {
        // 检查面板引用是否为空
        if (developerPanel != null)
        {
            // 显示UI面板
            developerPanel.SetActive(true);
            
        }
        else
        {
            Debug.LogError("请在Inspector面板中为Menu脚本赋值developerPanel！");
        }
    }

    // 隐藏开发者UI（可绑定到UI的关闭按钮）
    public void HideDeveloperUI()
    {
        if (developerPanel != null)
        {
            developerPanel.SetActive(false);
        }
    }

    // 初始化：确保UI默认隐藏
     void Start()
    {
        HideDeveloperUI();
    }
}
