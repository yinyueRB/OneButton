using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("UI 引用")]
    public GameObject pauseMenuPanel; // 拖入刚才做的半透明菜单面板

    // 全局静态变量，方便其他脚本知道游戏是不是暂停了
    public static bool isPaused = false;

    void Update()
    {
        // 按下 ESC 键切换暂停/继续状态
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // 继续游戏
    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false); // 隐藏菜单
        Time.timeScale = 1f;             // 恢复时间流逝
        isPaused = false;                // 更新状态
    }

    // 暂停游戏
    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);  // 显示菜单
        Time.timeScale = 0f;             // 冻结时间（所有 Update 里的位移动画都会停下）
        isPaused = true;                 // 更新状态
    }

    // 退出游戏
    public void QuitGame()
    {
        Debug.Log("<color=red>正在退出游戏...</color>");
        
        // 打包出电脑游戏后，这行代码生效
        Application.Quit();
        
        // 在 Unity 编辑器里测试时，用这行代码强制停止运行
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
