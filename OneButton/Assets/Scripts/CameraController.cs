using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("追踪目标")]
    public Transform player; // 拖入主角

    [Header("镜头视距设置")]
    public float zoomedInDistance = 15f; // 聚焦时离玩家有多远（数字越小越放大）
    public float smoothSpeed = 6f;       // 镜头切换时的丝滑平滑度

    private Vector3 fullMapPosition; // 保存一开始的全图固定坐标

    void Start()
    {
        // 游戏刚开始时，自动记录下你在编辑器里摆好的全图完美视角！
        fullMapPosition = transform.position;
    }

    // 摄像机跟随必须写在 LateUpdate 里，确保在玩家移动完之后再跟随，防止画面抖动
    void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPosition;

        // 1. 如果玩家按住了 鼠标右键 (1) -> 镜头回到全图中心
        if (Input.GetMouseButton(1))
        {
            targetPosition = fullMapPosition;
        }
        // 2. 正常状态 -> 镜头死死聚焦玩家
        else
        {
            // 核心魔法算法：目标位置 = 玩家当前位置 - 摄像机的正前方 * 聚焦距离
            // 这样不管你的摄像机一开始是怎么倾斜的，它都会完美保持原角度拉近玩家！
            targetPosition = player.position - transform.forward * zoomedInDistance;
        }

        // 使用 Lerp 进行极其丝滑的平滑插值移动
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
    }
}