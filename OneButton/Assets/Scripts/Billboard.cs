using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // 游戏开始时，自动获取场景中的主摄像机
        mainCamera = Camera.main;
        
        // 防错提示
        if (mainCamera == null)
        {
            Debug.LogError("Billboard 脚本找不到主摄像机！请检查你的摄像机 Tag 是否设置为 'MainCamera'");
        }
    }

    // 注意：这里使用的是 LateUpdate 而不是 Update
    void LateUpdate()
    {
        if (mainCamera != null)
        {
            // 让这个 UI 物体的旋转，完全等于摄像机的旋转
            // 这样 UI 就像贴在屏幕表面一样，永远正对着玩家
            transform.rotation = mainCamera.transform.rotation;
        }
    }
}
