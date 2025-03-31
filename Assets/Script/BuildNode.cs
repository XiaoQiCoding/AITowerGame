using UnityEngine;

/// <summary>
/// 建造节点，标识场景中可以建造防御塔的位置
/// </summary>
public class BuildNode : MonoBehaviour
{
    [Header("节点设置")] public bool isOccupied = false; // 该位置是否已经被占用
    // public GameObject currentTower;  // 当前建造的塔

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void Start()
    {
        // 确保有碰撞器用于点击检测
        if (GetComponent<BoxCollider2D>() == null)
        {
            Debug.LogWarning("BuildNode没有BoxCollider2D，将无法点击");
        }
    }

    private void OnMouseDown()
    {
        if (UIManager.Instance.GetPanel<BuildPanel>().gameObject.activeInHierarchy)
            return;

        // 如果节点未被占用，显示建造面板
        if (!isOccupied)
        {
            ShowBuildPanel();
        }
        else
        {
            return;
            // 如果已有塔，可以添加升级/出售的逻辑
            // Debug.Log($"该位置已有塔: {currentTower?.name}");
            // TODO: 显示升级/出售面板
        }
    }

    /// <summary>
    /// 显示建造面板
    /// </summary>
    private void ShowBuildPanel()
    {
        UIManager.Instance.ShowBuildPanel(transform.position, this);
    }

    /// <summary>
    /// 在该节点建造塔
    /// </summary>
    public bool BuildTower(TowerType towerType)
    {
        if (isOccupied)
        {
            Debug.LogWarning("该位置已被占用，无法建造");
            return false;
        }

        // 使用TowerManager来创建实际的塔
        bool success = TowerManager.Instance.BuildTower(towerType, transform.position);
        if (success)
        {
            // 更新节点状态
            isOccupied = true;
            // 查找刚创建的塔（在当前位置）
            // Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);
            // foreach (var hit in hits)
            // {
            //     Tower tower = hit.GetComponent<Tower>();
            //     if (tower != null && tower.towerType == towerType)
            //     {
            //         currentTower = hit.gameObject;
            //         break;
            //     }
            // }

            // 可以隐藏节点的视觉效果
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }

            Debug.Log($"在节点 {gameObject.name} 成功建造了 {towerType} 塔");
            return true;
        }

        return false;
    }

    /// <summary>
    /// 清除该节点上的塔
    /// </summary>
    // public void ClearTower()
    // {
    //     if (currentTower != null)
    //     {
    //         Destroy(currentTower);
    //     }
    //     
    //     isOccupied = false;
    //     currentTower = null;
    //     
    //     // 恢复节点的视觉效果
    //     if (spriteRenderer != null)
    //     {
    //         spriteRenderer.enabled = true;
    //         spriteRenderer.color = originalColor;
    //     }
    // }

    // 在编辑器中绘制可视化表示
    private void OnDrawGizmos()
    {
        Gizmos.color = isOccupied ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}