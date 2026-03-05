using UnityEngine;
using TMPro;

public class LetterCarousel : MonoBehaviour
{[Header("UI 引用")]
    public TextMeshProUGUI topText;     // 上一个字母
    public TextMeshProUGUI middleText;  // 当前字母 (选中项)
    public TextMeshProUGUI bottomText;  // 下一个字母

    public WordManager wordManager;
    public PlayerController player;

    [Header("轮播设置")]
    public float scrollSpeed = 0.15f; // 现在你可以把速度稍微调快一点了，比如 0.1f
    private float timer = 0f;

    public enum CarouselMode { Alphabet, TurnSelection }
    public CarouselMode currentMode = CarouselMode.Alphabet;

    // 完整的字母表
    private string[] alphabet = { 
        "A", "D", "E", "H", "J", "M", "N", "O", "P", "R", "S", "T", "U", 
        "V"
    };
    private string[] turnIcons = { "<", ">" }; 
    
    private string[] currentArray;
    private int currentIndex = 0;
    private bool isScrolling = false;

    void Start()
    {
        ResetToAlphabet();
    }

    public void SwitchToTurnMode()
    {
        currentMode = CarouselMode.TurnSelection;
        currentArray = turnIcons;
        currentIndex = 0;
        UpdateUIText();
    }

    public void ResetToAlphabet()
    {
        currentMode = CarouselMode.Alphabet;
        currentArray = alphabet;
        currentIndex = 0;
        UpdateUIText();
    }

    void Update()
    {
        bool canInput = wordManager.CanInput() && !player.isActing;

        // 1. 按下瞬间 -> 开启滚动状态，重置计时器（但不立刻切换字母！）
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && canInput)
        {
            isScrolling = true;
            timer = 0f; 
            // 【修复点】：删掉了这里的 NextItem();
        }

        // 2. 长按过程中 -> 只要时间超过阈值，就滚动到下一个
        if (isScrolling)
        {
            timer += Time.deltaTime;
            if (timer >= scrollSpeed)
            {
                timer = 0f;
                NextItem();
            }
        }

        // 3. 松手瞬间 -> 停止滚动，并锁定当前的字母
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0))
        {
            if (isScrolling) 
            {
                isScrolling = false;
                LockCurrentItem();
            }
        }
    }

    private void NextItem()
    {
        currentIndex++;
        if (currentIndex >= currentArray.Length) currentIndex = 0;
        UpdateUIText();
    }

    // ================= 核心修改点：同时更新 3 个文本 =================
    private void UpdateUIText()
    {
        // 1. 计算上一项的索引 (如果是0，就退回到数组末尾)
        int prevIndex = currentIndex - 1;
        if (prevIndex < 0) prevIndex = currentArray.Length - 1;

        // 2. 计算下一项的索引 (如果是末尾，就回到0)
        int nextIndex = currentIndex + 1;
        if (nextIndex >= currentArray.Length) nextIndex = 0;

        // 3. 赋值给 UI
        if (topText != null) topText.text = currentArray[prevIndex];
        if (middleText != null) middleText.text = currentArray[currentIndex];
        if (bottomText != null) bottomText.text = currentArray[nextIndex];
    }
    // ==============================================================

    private void LockCurrentItem()
    {
        string selectedItem = currentArray[currentIndex];
        
        if (currentMode == CarouselMode.Alphabet)
        {
            if (wordManager != null) wordManager.AddLetter(selectedItem);
        }
        else if (currentMode == CarouselMode.TurnSelection)
        {
            bool isLeft = (currentIndex == 0); 
            player.Turn(isLeft); 
            ResetToAlphabet();   
        }
        
        currentIndex = 0; 
        UpdateUIText();
    }
}