using UnityEngine;

public class SpriteDirection : MonoBehaviour
{
    private Animator anim;
    private Camera cam;

    void Start()
    {
        anim = GetComponent<Animator>();
        cam = Camera.main;
    }

    void Update()
    {
        if (cam == null) return;

        // 获取相机的前向和右向（忽略高度 Y 轴，只算水平面方向）
        Vector3 camForward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
        Vector3 camRight = new Vector3(cam.transform.right.x, 0, cam.transform.right.z).normalized;

        // 获取玩家胶囊体（父物体）实际面朝的物理方向
        Vector3 playerForward = transform.parent.forward; 

        // 核心数学魔法：计算玩家面朝方向在相机视角中的投影
        float dirY = Vector3.Dot(playerForward, camForward);
        float dirX = Vector3.Dot(playerForward, camRight);

        // 将算出的 X 和 Y 传递给动画机
        anim.SetFloat("DirX", dirX);
        anim.SetFloat("DirY", dirY);
    }
}
