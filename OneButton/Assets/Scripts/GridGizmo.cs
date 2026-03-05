using UnityEngine;

public class GridGizmo : MonoBehaviour
{
    [Header("网格设置")]
    public float gridSize = 2f;      // 格子大小（2米）
    public int gridCount = 20;       // 网格的数量（比如画 20x20 个格子）
    public Color lineColor = Color.cyan; // 网格线的颜色（默认青色，比较显眼）

    // 这个方法专门用于在 Unity 编辑器的 Scene 窗口画辅助线
    void OnDrawGizmos()
    {
        Gizmos.color = lineColor;
        Vector3 center = transform.position;

        // 计算网格的总宽度的一半，方便居中画线
        float halfWidth = (gridCount * gridSize) / 2f;

        for (int i = 0; i <= gridCount; i++)
        {
            float offset = -halfWidth + (i * gridSize);

            // 画横线 (平行于 X 轴)
            Gizmos.DrawLine(center + new Vector3(-halfWidth, 0, offset), center + new Vector3(halfWidth, 0, offset));
            
            // 画竖线 (平行于 Z 轴)
            Gizmos.DrawLine(center + new Vector3(offset, 0, -halfWidth), center + new Vector3(offset, 0, halfWidth));
        }
    }
}
