using UnityEngine;
using TMPro;

public class LetterCarousel : MonoBehaviour
{
    [Header("UI 引用")]
    public TextMeshProUGUI letterText;
    public WordManager wordManager; // 关联单词管理器
    public PlayerController player; // 关联刚刚写的玩家控制器

    [Header("轮播设置")]
    public float scrollSpeed = 0.15f;
    private float timer = 0f;

    // 状态枚举：当前处于打字模式，还是在选转向模式？
    public enum CarouselMode { Alphabet, TurnSelection }
    public CarouselMode currentMode = CarouselMode.Alphabet;

    // 两个不同的内容数组
    private string[] alphabet = { 
        "M", "O", "V", "E", "T", "U", "R", "N", "J", "P", "D", "A", "S", 
        "H"
    };
    private string[] turnIcons = { "<", ">" }; // 用 < 和 > 代表左右转
    
    private string[] currentArray;     // 当前正在滚动的是哪个数组
    private int currentIndex = 0;
    private bool isScrolling = false;

    void Start()
    {
        ResetToAlphabet(); // 初始化为字母模式
    }

    // 提供给外部：切换到转向模式
    public void SwitchToTurnMode()
    {
        currentMode = CarouselMode.TurnSelection;
        currentArray = turnIcons;
        currentIndex = 0;
        UpdateUIText();
    }

    // 恢复到字母模式
    public void ResetToAlphabet()
    {
        currentMode = CarouselMode.Alphabet;
        currentArray = alphabet;
        currentIndex = 0;
        UpdateUIText();
    }

    void Update()
    {
        // 只有当管理器允许输入，且玩家没在移动时，才能操作
        bool canInput = wordManager.CanInput() && !player.isActing;

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && canInput)
        {
            isScrolling = true;
            timer = 0f;
        }

        if (isScrolling)
        {
            timer += Time.deltaTime;
            if (timer >= scrollSpeed)
            {
                timer = 0f;
                NextItem();
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0))
        {
            if (isScrolling) // 防止没按下就松手触发bug
            {
                isScrolling = false;
                LockCurrentItem();
            }
        }
    }

    private void NextItem()
    {
        currentIndex++;
        // 注意这里改成了 currentArray.Length，这样能自动适配两个数组的长度
        if (currentIndex >= currentArray.Length) currentIndex = 0;
        UpdateUIText();
    }

    private void UpdateUIText()
    {
        if (letterText != null) letterText.text = currentArray[currentIndex];
    }

    private void LockCurrentItem()
    {
        string selectedItem = currentArray[currentIndex];
        
        // 如果是在拼写字母模式
        if (currentMode == CarouselMode.Alphabet)
        {
            if (wordManager != null) wordManager.AddLetter(selectedItem);
        }
        // 如果是在选方向模式
        else if (currentMode == CarouselMode.TurnSelection)
        {
            bool isLeft = (currentIndex == 0); // 索引0是"<"，代表左
            player.Turn(isLeft); // 调用主角转身
            ResetToAlphabet();   // 转身后马上恢复成字母表
        }
        
        currentIndex = 0; 
        UpdateUIText();
    }
}
