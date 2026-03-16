using UnityEngine;
using TMPro;

public class LetterCarousel : MonoBehaviour
{
    [Header("UI 引用 (密码锁)")]
    public TextMeshProUGUI topText;
    public TextMeshProUGUI middleText;
    public TextMeshProUGUI bottomText;
    
    [Header("逻辑引用")]
    public WordManager wordManager;
    public PlayerController player;
    public AudioSource scrollAudio;

    [Header("轮播设置")]
    public float scrollSpeed = 0.15f;
    private float timer = 0f;

    // 【核心大改】：把转向符直接放在最前面，并且剔除了没用的干扰字母！
    private string[] alphabet = { 
        "<", ">", "A", "D", "E", "H", "J", "M", "N", "O", "P", "S", "T", "U", "V" 
    };
    
    // 如果你依然想保留完整的 26 个字母，请删掉上面那行，用下面这行：
    // private string[] alphabet = { "<", ">", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

    private int currentIndex = 0;
    private bool isScrolling = false;

    void Start()
    {
        UpdateUIText();
    }

    void Update()
    {
        bool isClickingUI = UnityEngine.EventSystems.EventSystem.current != null && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
        bool canInput = wordManager.CanInput() && !player.isActing && !PauseManager.isPaused && !isClickingUI;

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && canInput)
        {
            isScrolling = true;
            timer = 0f;
            if (scrollAudio != null) scrollAudio.Play();
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
            if (isScrolling) 
            {
                isScrolling = false;
                if (scrollAudio != null) scrollAudio.Stop();
                LockCurrentItem();
            }
        }
    }

    private void NextItem()
    {
        currentIndex++;
        if (currentIndex >= alphabet.Length) currentIndex = 0;
        UpdateUIText();
    }

    private void UpdateUIText()
    {
        int prevIndex = currentIndex - 1;
        if (prevIndex < 0) prevIndex = alphabet.Length - 1;

        int nextIndex = currentIndex + 1;
        if (nextIndex >= alphabet.Length) nextIndex = 0;

        if (topText != null) topText.text = alphabet[prevIndex];
        if (middleText != null) middleText.text = alphabet[currentIndex];
        if (bottomText != null) bottomText.text = alphabet[nextIndex];
    }

    private void LockCurrentItem()
    {
        string selectedItem = alphabet[currentIndex];
        
        // 【全新逻辑】：拦截转向符，不发给 WordManager，直接让主角转身！
        if (selectedItem == "<")
        {
            player.Turn(true);  // 直接执行左转
        }
        else if (selectedItem == ">")
        {
            player.Turn(false); // 直接执行右转
        }
        else
        {
            // 如果是普通字母，才发送给拼字格
            if (wordManager != null) wordManager.AddLetter(selectedItem);
        }
        
        currentIndex = 0; // 操作完毕后，轮播表重置回起始点（也就是 "<"）
        UpdateUIText();
    }
}