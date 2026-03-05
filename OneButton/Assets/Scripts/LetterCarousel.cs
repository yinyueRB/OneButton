using UnityEngine;
using TMPro; // 引入 TextMeshPro 命名空间

public class LetterCarousel : MonoBehaviour
{
    [Header("引用")]
    public WordManager wordManager; // 关联单词管理器
    
    [Header("UI 引用")]
    public TextMeshProUGUI letterText; // 挂载我们刚才创建的 Text 文本

    [Header("轮播设置")]
    public float scrollSpeed = 0.15f;  // 字母滚动的间隔时间（秒），越小滚得越快
    private float timer = 0f;          // 计时器

    // 完整的字母表（为了后续方便改成图标，我们用字符串数组）
    private string[] alphabet = { 
        "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", 
        "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" 
    };
    
    private int currentIndex = 0;      // 当前轮播到的字母索引
    private bool isScrolling = false;  // 是否正在长按滚动

    void Start()
    {
        // 游戏开始时显示第一个字母
        UpdateUIText();
    }

    void Update()
    {
        // 1. 检测按下 (空格键 或 鼠标左键) -> 开始滚动
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) && wordManager.CanInput())
        {
            isScrolling = true;
            timer = 0f; // 按下时重置计时器，确保立即有反馈
        }

        // 2. 检测长按过程中 -> 按速度切换字母
        if (isScrolling)
        {
            timer += Time.deltaTime; // 累加时间
            
            if (timer >= scrollSpeed) // 当时间超过我们设置的间隔
            {
                timer = 0f; // 重置计时器
                NextLetter(); // 切换到下一个字母
            }
        }

        // 3. 检测松手 -> 停止滚动并锁定字母
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0))
        {
            isScrolling = false;
            LockCurrentLetter(); // 锁定并输出当前字母
        }
    }

    // 切换到下一个字母的方法
    private void NextLetter()
    {
        currentIndex++;
        
        // 如果超出了 Z，就回到 A (循环)
        if (currentIndex >= alphabet.Length)
        {
            currentIndex = 0;
        }
        
        UpdateUIText();
    }

    // 更新 UI 显示的方法
    private void UpdateUIText()
    {
        if (letterText != null)
        {
            letterText.text = alphabet[currentIndex];
        }
    }

    // 松手时锁定字母的方法
    private void LockCurrentLetter()
    {
        string selectedLetter = alphabet[currentIndex];
    
        if (wordManager != null)
        {
            wordManager.AddLetter(selectedLetter);
        }
    
        currentIndex = 0; 
        UpdateUIText();
    }
}
