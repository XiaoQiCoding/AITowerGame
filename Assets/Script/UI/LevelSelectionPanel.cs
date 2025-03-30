using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 关卡选择面板，用于显示和管理关卡选择UI
/// </summary>
public class LevelSelectionPanel : BasePanel
{
    [SerializeField] private Transform nodesContainer; // Nodes父对象

    private List<LevelNode> levelNodes = new List<LevelNode>();
    private int selectedLevel = -1;

    protected override void OnInit()
    {
        base.OnInit();

        // 查找所有关卡节点并初始化
        for (int i = 0; i < nodesContainer.childCount; i++)
        {
            Transform nodeTransform = nodesContainer.GetChild(i);
            LevelNode node = nodeTransform.GetComponent<LevelNode>();
            if (node != null)
            {
                node.Initialize(i + 1); // 关卡ID从1开始
                levelNodes.Add(node);
            }
        }

        // 添加调试日志
        Debug.Log($"关卡选择面板初始化完成，找到 {levelNodes.Count} 个关卡节点");
        Debug.Log($"当前最高解锁关卡: {GameManager.Instance.MaxUnlockedLevel}");
    }

    protected override void OnOpen()
    {
        base.OnOpen();

        // 获取玩家通关的最高关卡
        int highestCompletedLevel = 0;
        foreach (var levelEntry in GameManager.Instance.LevelStars)
        {
            if (levelEntry.Value >= 1 && levelEntry.Key > highestCompletedLevel)
            {
                highestCompletedLevel = levelEntry.Key;
            }
        }

        // 选择下一个要挑战的关卡（已通关关卡的下一关）
        if (highestCompletedLevel > 0 && highestCompletedLevel < GameManager.Instance.MaxUnlockedLevel)
        {
            selectedLevel = highestCompletedLevel + 1;
            Debug.Log($"自动选择下一关: {selectedLevel}");
        }
        // 如果没有通关任何关卡或已是最后一关，选择第一个解锁的关卡
        else if (selectedLevel <= 0)
        {
            selectedLevel = 1; // 默认选择第一关
        }

        UpdateLevelNodes();

        // 添加调试日志
        Debug.Log("关卡选择面板已打开，节点状态已更新");
    }


    /// <summary>
    /// 更新所有关卡节点的状态
    /// </summary>
    public void UpdateLevelNodes()
    {
        int maxUnlockedLevel = GameManager.Instance.MaxUnlockedLevel;
        Debug.Log($"更新关卡节点状态，最高解锁关卡: {maxUnlockedLevel}");
        
        // 检查是否需要解锁新关卡
        foreach (var levelEntry in GameManager.Instance.LevelStars)
        {
            // 如果有关卡至少获得了1星，解锁下一关
            if (levelEntry.Value >= 1)
            {
                int nextLevelId = levelEntry.Key + 1;
                if (nextLevelId > maxUnlockedLevel && nextLevelId <= levelNodes.Count)
                {
                    // 更新解锁状态
                    Debug.Log($"发现关卡 {levelEntry.Key} 已有 {levelEntry.Value} 星，解锁下一关 {nextLevelId}");
                    GameManager.Instance.UnlockLevel(nextLevelId);
                    maxUnlockedLevel = GameManager.Instance.MaxUnlockedLevel;
                }
            }
        }

        for (int i = 0; i < levelNodes.Count; i++)
        {
            int levelId = i + 1;
            LevelNode node = levelNodes[i];

            // 检查关卡是否已解锁
            bool isUnlocked = levelId <= maxUnlockedLevel;

            // 检查是当前选中关卡还是已通关关卡
            bool isSelected = levelId == selectedLevel;

            // 获取关卡星级（如果已通关）
            int stars = 0;
            if (GameManager.Instance.LevelStars.TryGetValue(levelId, out int levelStars))
            {
                stars = levelStars;
            }

            // 更新节点显示状态
            node.UpdateState(isUnlocked, isSelected, stars);
        }
    }


    /// <summary>
    /// 选择关卡
    /// </summary>
    public void SelectLevel(int levelId)
    {
        Debug.Log($"关卡 {levelId} 被选择");
        selectedLevel = levelId;
        UpdateLevelNodes();
        StartSelectedLevel();
    }

    /// <summary>
    /// 开始已选择的关卡
    /// </summary>
    public void StartSelectedLevel()
    {
        if (selectedLevel > 0 && selectedLevel <= GameManager.Instance.MaxUnlockedLevel)
        {
            Debug.Log($"开始游戏关卡: {selectedLevel}");
            GameManager.Instance.StartLevel(selectedLevel);
        }
        else
        {
            Debug.LogWarning($"无法开始关卡 {selectedLevel}，最高解锁关卡为 {GameManager.Instance.MaxUnlockedLevel}");
        }
    }


    /// <summary>
    /// 返回主菜单
    /// </summary>
    public void BackToMainMenu()
    {
        Debug.Log("返回主菜单");
        UIManager.Instance.ShowMainMenu();
    }
}