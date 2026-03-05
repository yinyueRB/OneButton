using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveDistance = 2f;    // 每次 Move 移动的距离
    public float actionDuration = 0.3f;// 移动或转向花费的时间
    public float dashSpeed = 15f;      // DASH（冲刺）的速度，数字越大越快
    
    [Header("跳跃设置")]
    public float jumpDistance = 2f; 
    public float jumpHeight = 1.5f; // 跳跃最高点的高度
    public float jumpDuration = 0.4f; // 跳跃在空中的时间

    [Header("环境检测设置")]
    public LayerMask obstacleLayer;    // 障碍物所在的图层（用于Dash检测）
    
    [HideInInspector] 
    public bool isActing = false; 

    private Vector3 lastSafePosition;  // 记录掉下坑之前的安全位置
    private Renderer playerRenderer;   // 用于控制闪烁效果

    void Start()
    {
        // 获取玩家身上的渲染器（用来做复活闪烁）
        playerRenderer = GetComponent<Renderer>();
    }

    // --- 公开指令方法 ---
    public void MoveForward()
    {
        if (!isActing)
        {
            // 在玩家肚子高度，向正前方发射一条射线，检测距离等于 moveDistance (也就是2米)
            Vector3 rayStart = transform.position + Vector3.up * 0.5f;
            
            // 发射射线：起点, 方向, 结果存入hit(这里不需要), 检测距离, 检测的图层
            if (Physics.Raycast(rayStart, transform.forward, moveDistance, obstacleLayer))
            {
                // 如果射线打到了 Obstacle 层的物体，说明前方有墙！
                Debug.Log("<color=yellow>前方有墙壁阻挡，无法移动！</color>");
                // 这里可以什么都不做，玩家就原地不动，也不会穿模
            }
            else
            {
                // 如果前方空空如也，才执行真正的平滑移动逻辑
                StartCoroutine(MoveRoutine(transform.position + transform.forward * moveDistance));
            }
        }
    }

    public void Dash()
    {
        if (!isActing) StartCoroutine(DashRoutine());
    }

    public void Turn(bool isLeft)
    {
        if (!isActing) StartCoroutine(TurnRoutine(isLeft));
    }
    
    public void Jump()
    {
        if (!isActing) StartCoroutine(JumpRoutine());
    }
    
    // --- 核心协程逻辑 ---

    // 通用移动协程（供 Move 使用）
    private IEnumerator MoveRoutine(Vector3 targetPos)
    {
        isActing = true;
        lastSafePosition = transform.position; // 行动前，记录当前位置为“安全位置”
        
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < actionDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / actionDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.position = targetPos; 
        
        // 移动结束后，检测脚底下是不是坑
        CheckGround();
    }

    // 冲刺协程 (Dash)
    private IEnumerator DashRoutine()
    {
        isActing = true;
        lastSafePosition = transform.position; // 记录冲刺前的安全位置
        Vector3 startPos = transform.position;
        Vector3 targetPos;

        // 向正前方发射一条射线，最远检测 100 米，只检测 Obstacle 层的物体
        RaycastHit hit;
        // 射线起点稍微抬高0.5米，防止蹭到地板
        Vector3 rayStart = transform.position + Vector3.up * 0.5f; 

        if (Physics.Raycast(rayStart, transform.forward, out hit, 100f, obstacleLayer))
        {
            // 【核心修改点在这里】
            // hit.distance 是玩家中心点到墙壁表面的实际距离
            // 我们希望玩家中心停在离墙表面 1 米的地方，所以实际移动距离 = 总距离 - 1米
            float travelDistance = hit.distance - 1f;

            // 做一个安全限制：如果玩家起步时离墙就已经不足1米了，就让他原地不动 (距离设为0)
            if (travelDistance < 0f) 
            {
                travelDistance = 0f;
            }

            // 计算出最终停下的目标位置
            targetPos = startPos + transform.forward * travelDistance;
        }
        else
        {
            // 如果前方100米都没有障碍物，就直接冲刺20米（防止飞出地图边界）
            targetPos = startPos + transform.forward * 20f;
        }

        // 按固定速度计算冲刺需要的时间
        float distance = Vector3.Distance(startPos, targetPos);
        
        // 如果需要移动的距离大于0，才执行位移过程
        if (distance > 0f)
        {
            float duration = distance / dashSpeed;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        transform.position = targetPos; // 确保精准停在离墙1米处

        // 冲刺结束后，检测停下的位置脚底是不是坑
        CheckGround(); 
    }

    // 转向协程（未变动）
    private IEnumerator TurnRoutine(bool isLeft)
    {
        isActing = true;
        Quaternion startRot = transform.rotation;
        float angle = isLeft ? -90f : 90f;
        Quaternion targetRot = startRot * Quaternion.Euler(0, angle, 0);
        float elapsed = 0f;

        while (elapsed < actionDuration)
        {
            transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / actionDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRot;
        isActing = false;
    }

    // --- 跳跃协程 ---
    private IEnumerator JumpRoutine()
    {
        isActing = true;
        lastSafePosition = transform.position; // 记录跳跃前的安全位置

        Vector3 startPos = transform.position;
        // 计算落地点
        Vector3 targetPos = startPos + transform.forward * jumpDistance;
        float elapsed = 0f;

        while (elapsed < jumpDuration)
        {
            // 1. 在 XZ 平面上进行平滑移动 (水平匀速)
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, elapsed / jumpDuration);
            
            // 2. 利用 Mathf.Sin 曲线计算 Y 轴高度 (0 到 PI 刚好是一个拱起的半圆)
            float height = Mathf.Sin((elapsed / jumpDuration) * Mathf.PI) * jumpHeight;
            currentPos.y = startPos.y + height; // 加上拱起的高度

            // 应用位置
            transform.position = currentPos;
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos; // 确保落点绝对精确
        
        // 落地后检测脚底是不是坑
        CheckGround(); 
    }
    
    // --- 机制检测逻辑 ---

    // 检测脚底
    private void CheckGround()
    {
        RaycastHit hit;
        // 从玩家肚子处向下打一条 2 米的射线
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 2f))
        {
            // 如果射线打到的东西，标签是 "Pit" (坑)
            if (hit.collider.CompareTag("Pit"))
            {
                // 触发掉落与复活机制
                StartCoroutine(FallAndRespawnRoutine());
                return; // 直接返回，让 FallRoutine 结尾去解除 isActing
            }
        }
        // 如果脚下是安全的（没碰到坑），则结束行动，允许玩家进行下一次输入
        isActing = false;
    }

    // 掉落、延迟复活与闪烁协程
    private IEnumerator FallAndRespawnRoutine()
    {
        // 1. 掉落动画 (0.3秒内往下掉3米)
        Vector3 startPos = transform.position;
        Vector3 fallPos = startPos + Vector3.down * 3f;
        float elapsed = 0f;
        while (elapsed < 0.3f)
        {
            transform.position = Vector3.Lerp(startPos, fallPos, elapsed / 0.3f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 2. 在坑底等待 1 秒钟
        yield return new WaitForSeconds(1f);

        // 3. 瞬间传送到行动前的安全位置
        transform.position = lastSafePosition;

        // 4. 执行复活无敌闪烁动画 (闪烁 4 次)
        if (playerRenderer != null)
        {
            for (int i = 0; i < 4; i++)
            {
                playerRenderer.enabled = false; // 隐身
                yield return new WaitForSeconds(0.15f);
                playerRenderer.enabled = true;  // 显形
                yield return new WaitForSeconds(0.15f);
            }
        }

        // 5. 动画结束，允许玩家再次行动
        isActing = false;
    }
}