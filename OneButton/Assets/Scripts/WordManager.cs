using UnityEngine;
using TMPro; // 引入 UI 文字
using System.Collections; // 引入协程（用于延迟清空）

public class WordManager : MonoBehaviour
{
    [Header("UI 引用")][Tooltip("请按顺序拖入 4 个格子的 Text 组件")]
    public TextMeshProUGUI[] slotTexts; // 存放4个格子的数组
    
    [Header("逻辑引用")]
    public PlayerController player;
    public LetterCarousel carousel;

    private string currentWord = "";    // 当前玩家打出的字母组合
    private bool isProcessing = false;  // 是否正在处理判定（判定时不允许玩家继续打字）

    // 游戏开始时初始化 UI
    void Start()
    {
        UpdateUI();
    }
    
    void Update()
    {
        // 只有在允许输入的时候（没有在判定、没有在移动），才能按快捷键
        if (CanInput()) 
        {
            // 按下键盘上的数字键 1~4，瞬间填满单词并执行！
            if (Input.GetKeyDown(KeyCode.Alpha1)) ForceWord("MOVE");
            if (Input.GetKeyDown(KeyCode.Alpha2)) ForceWord("TURN");
            if (Input.GetKeyDown(KeyCode.Alpha3)) ForceWord("DASH");
            if (Input.GetKeyDown(KeyCode.Alpha4)) ForceWord("JUMP");
        }
    }

    // 强行写入单词并直接走判定流程
    private void ForceWord(string debugWord)
    {
        currentWord = debugWord; // 直接把当前字母组合替换成完整单词
        
        // 瞬间刷新 4 个格子的 UI 显示
        for (int i = 0; i < slotTexts.Length; i++)
        {
            slotTexts[i].text = currentWord[i].ToString();
        }

        // 直接呼叫原来的单词判定逻辑！
        CheckWord(); 
    }

    // 提供给外部的方法：判断当前是否允许玩家继续输入字母
    public bool CanInput()
    {
        // 如果正在结算，或者单词已经达到4个字母，就禁止输入
        return !isProcessing && currentWord.Length < 4;
    }

    // 接收来自轮播表的字母
    public void AddLetter(string letter)
    {
        if (!CanInput()) return; // 安全检查

        currentWord += letter; // 把新字母加到末尾
        UpdateUI();            // 更新头顶的格子显示

        // 如果填满了 4 个字母，开始判定
        if (currentWord.Length == 4)
        {
            CheckWord();
        }
    }

    // 刷新那 4 个格子的显示
    private void UpdateUI()
    {
        for (int i = 0; i < slotTexts.Length; i++)
        {
            if (i < currentWord.Length)
                slotTexts[i].text = currentWord[i].ToString(); // 显示已打出的字母
            else
                slotTexts[i].text = "_"; // 没打出的地方显示下划线
        }
    }

    // 判定单词对错
    private void CheckWord()
    {
        isProcessing = true;
        string word = currentWord.ToUpper();

        if (word == "MOVE")
        {
            Debug.Log("<color=cyan>成功打出指令: MOVE! 执行向前移动</color>");
            player.MoveForward(); // 呼叫移动
            ClearWord(1f); // 1秒后清空填字格
        }
        else if (word == "TURN")
        {
            Debug.Log("<color=cyan>成功打出指令: TURN! 切换轮播表</color>");
            carousel.SwitchToTurnMode(); // 呼叫轮播表切换模式
            ClearWord(0f); // 立刻清空填字格，以便玩家专心选方向
        }
        else if (word == "DASH")
        {
            Debug.Log("<color=cyan>成功打出指令: DASH! 执行冲刺</color>");
            player.Dash(); // 呼叫玩家执行冲刺
            ClearWord(1f); // 1秒后清空填字格
        }
        else if (word == "JUMP")
        {
            Debug.Log("<color=cyan>成功打出指令: JUMP! 执行跳跃</color>");
            player.Jump(); // 呼叫跳跃动作
            ClearWord(1f); // 1秒后清空
        }
        else
        {
            // 打错了的情况
            Debug.Log("<color=red>错误单词: " + word + "，即将清空重来</color>");
            ClearWord(0.5f); // 发现打错，0.5秒后自动清空（体验比较顺滑）
        }
    }

    // 触发延迟清空
    private void ClearWord(float delay)
    {
        StartCoroutine(ClearRoutine(delay));
    }

    // 协程：等待几秒后清空数据
    private IEnumerator ClearRoutine(float delay)
    {
        yield return new WaitForSeconds(delay); // 等待 delay 秒
        
        currentWord = "";    // 清空内部字符串
        UpdateUI();          // 刷新UI为空白
        isProcessing = false;// 解锁，允许玩家重新打字
    }
}