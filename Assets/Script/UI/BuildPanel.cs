using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 建造面板控制器
/// </summary>
public class BuildPanel : BasePanel
{
    [Header("位置设置")]
    [SerializeField] private RectTransform rectTransform; // 面板的RectTransform
    [SerializeField] private GameObject mainCloseButton; // 关闭按钮对象
    
    // 建造位置（世界坐标）
    private Vector3 buildPosition;
    // 所有塔节点
    private List<TowerBuildNode> towerNodes = new List<TowerBuildNode>();
    // 当前选中的建造节点
    private BuildNode currentBuildNode;
    
    protected override void OnInit()
    {
        base.OnInit();
        
        // 查找RectTransform
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        
        // 查找所有TowerBuildNode脚本
        towerNodes.Clear();
        TowerBuildNode[] nodes = GetComponentsInChildren<TowerBuildNode>(true);
        towerNodes.AddRange(nodes);
        
        // 为每个节点设置点击事件
        foreach (var node in towerNodes)
        {
            node.OnNodeClicked += HandleTowerNodeClicked;
        }
        
        // 设置主关闭按钮点击事件
        if (mainCloseButton != null)
        {
            UnityEngine.UI.Button closeButton = mainCloseButton.GetComponent<UnityEngine.UI.Button>();
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(CancelBuild);
            }
        }
    }
    
    protected override void OnOpen()
    {
        base.OnOpen();
        
        // 更新所有节点状态
        UpdateNodesState();
    }
    
    /// <summary>
    /// 设置面板位置并关联建造节点
    /// </summary>
    public void SetPosition(Vector3 worldPosition, BuildNode buildNode = null)
    {
        buildPosition = worldPosition;
        currentBuildNode = buildNode;
        
        // 将世界坐标转换为屏幕坐标
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        
        // 设置面板位置
        if (rectTransform != null)
        {
            rectTransform.position = screenPosition;
        }
        else
        {
            transform.position = screenPosition;
        }
    }
    
    /// <summary>
    /// 更新所有节点状态
    /// </summary>
    private void UpdateNodesState()
    {
        int playerCurrency = GameManager.Instance.Currency;
        
        foreach (var node in towerNodes)
        {
            if (node != null)
            {
                node.UpdateState(playerCurrency);
            }
        }
    }
    
    /// <summary>
    /// 处理塔节点点击
    /// </summary>
    private void HandleTowerNodeClicked(TowerType towerType, int cost)
    {
        // 检查金币是否足够
        if (GameManager.Instance.Currency >= cost)
        {
            // 尝试建造塔
            if (BuildTower(towerType, cost))
            {
                // 建造成功后关闭面板
                Hide();
            }
        }
        else
        {
            Debug.LogWarning($"金币不足，需要 {cost} 金币");
        }
    }
    
    /// <summary>
    /// 建造防御塔
    /// </summary>
    private bool BuildTower(TowerType towerType, int cost)
    {
        
        // 如果有关联的建造节点，使用它来建造塔
        if (currentBuildNode != null)
        {
            bool success = currentBuildNode.BuildTower(towerType);
            if (success)
            {
                // Debug.Log($"在位置 {buildPosition} 成功建造了 {towerTypeStr} 塔");
                return true;
            }
        }
        else
        {
            // 否则使用TowerManager直接在位置上建造
            bool success = TowerManager.Instance.BuildTower(towerType, buildPosition);
            if (success)
            {
                // Debug.Log($"在位置 {buildPosition} 成功建造了 {towerTypeStr} 塔");
                return true;
            }
        }
        
        // Debug.LogWarning($"建造 {towerTypeStr} 塔失败");
        return false;
    }
    
    /// <summary>
    /// 取消建造
    /// </summary>
    public void CancelBuild()
    {
        Debug.Log("取消建造");
        Hide();
    }
}
