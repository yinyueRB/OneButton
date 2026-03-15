using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // 必须引入这个才能使用协程

public class LevelManager : MonoBehaviour
{
    [Header("房间墙壁引用")]
    public GameObject frontWall;  // 前门
    public GameObject backWall;   // 后门

    [Header("通关设置")]
    public string winSceneName = "WinScene"; 
    public float leaveDistance = 4.0f;       

    [Header("机关渐变设置")]
    public float triggerDelay = 0.5f; // 玩家走出去后，延迟多少秒触发机关
    public float fadeDuration = 1.0f; // 渐变动画持续多长时间

    private Transform player;
    private Vector3 startPos;         
    private bool hasLeftRoom = false; 
    private bool isFinished = false;  

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            startPos = player.position;
        }

        // 初始化：前门隐藏并设为全透明；后门显示并设为不透明
        if (frontWall != null) 
        {
            SetWallAlpha(frontWall, 0f);
            frontWall.SetActive(false);
        }
        if (backWall != null) 
        {
            SetWallAlpha(backWall, 1f);
            backWall.SetActive(true);
        }
    }

    void Update()
    {
        if (player == null || isFinished) return;

        float distance = Vector3.Distance(player.position, startPos);

        // 1. 检测“离开房间”
        if (!hasLeftRoom && distance > leaveDistance)
        {
            hasLeftRoom = true; 
            Debug.Log("<color=cyan>玩家已离开出生点，等待 " + triggerDelay + " 秒后触发机关渐变...</color>");
            
            // 开启延迟触发的协程
            StartCoroutine(TriggerDoorsRoutine());
        }

        // 2. 检测“回到终点”
        if (hasLeftRoom && distance < 0.5f)
        {
            isFinished = true; 
            Debug.Log("<color=yellow>成功绕回终点！准备跳转...</color>");
            SceneManager.LoadScene(winSceneName);
        }
    }

    // --- 以下是新增的动画控制协程 ---

    // 统筹机关触发的协程
    private IEnumerator TriggerDoorsRoutine()
    {
        // 1. 等待设定的延迟时间 (比如 0.5 秒)
        yield return new WaitForSeconds(triggerDelay);

        // 2. 同时启动前门出现、后门消失的渐变协程
        if (frontWall != null) StartCoroutine(FadeWallRoutine(frontWall, true));
        if (backWall != null) StartCoroutine(FadeWallRoutine(backWall, false));
    }

    // 控制单一墙壁渐变的协程
    private IEnumerator FadeWallRoutine(GameObject wall, bool isAppearing)
    {
        Renderer rend = wall.GetComponent<Renderer>();
        if (rend == null) yield break; // 防错保护

        // 获取材质并拿到起始颜色
        Material mat = rend.material; 
        Color matColor = mat.color;

        float targetAlpha = isAppearing ? 1f : 0f;
        float startAlpha = isAppearing ? 0f : 1f;

        // 如果是“出现”，必须先激活物体，这样物理碰撞体才会立刻生效，然后视觉上慢慢浮现
        if (isAppearing)
        {
            wall.SetActive(true);
            matColor.a = 0f;
            mat.color = matColor;
        }

        // 逐帧计算渐变
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            matColor.a = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            mat.color = matColor;
            yield return null; // 等待下一帧
        }

        // 确保最终透明度精准
        matColor.a = targetAlpha;
        mat.color = matColor;

        // 如果是“消失”，等完全透明后，彻底关闭物体，移除物理碰撞
        if (!isAppearing)
        {
            wall.SetActive(false);
        }
    }

    // 辅助方法：瞬间设置墙壁透明度（用于游戏刚开始时的初始化）
    private void SetWallAlpha(GameObject wall, float alpha)
    {
        Renderer rend = wall.GetComponent<Renderer>();
        if (rend != null)
        {
            Color c = rend.material.color;
            c.a = alpha;
            rend.material.color = c;
        }
    }
}