using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 关卡节点控制器，用于管理单个关卡节点的UI状态
/// </summary>
[RequireComponent(typeof(Button))]
public class LevelNode : MonoBehaviour
{
    [SerializeField] private GameObject lockObject;           // 锁定状态对象
    [SerializeField] private GameObject nowLevelObject;       // 当前关卡状态对象
    [SerializeField] private GameObject passLevelObject;      // 已通关状态对象
    [SerializeField] private Transform starsContainer;        // 星级容器
    
    private Button button;
    private int levelId;
    
    public void Initialize(int id)
    {
        levelId = id;
        button = GetComponent<Button>();
        
        // 设置按钮点击事件
        button.onClick.AddListener(() => OnLevelSelected());
        
        // 确保星级对象都被禁用
        for (int i = 0; i < starsContainer.childCount; i++)
        {
            starsContainer.GetChild(i).gameObject.SetActive(false);
        }
    }
    
    public void UpdateState(bool isUnlocked, bool isSelected, int stars)
    {
        // 更新按钮交互状态 - 确保解锁的关卡可以点击
        button.interactable = isUnlocked;
        
        // 更新显示状态
        lockObject.SetActive(!isUnlocked);
        nowLevelObject.SetActive(isUnlocked && isSelected);
        passLevelObject.SetActive(isUnlocked && stars > 0);
        
        // 打印调试信息
        // Debug.Log($"关卡 {levelId} 状态更新: 已解锁={isUnlocked}, 已选中={isSelected}, 星级={stars}");
        
        // 更新星级显示
        if (isUnlocked && stars > 0)
        {
            // 禁用所有星级对象
            for (int i = 0; i < starsContainer.childCount; i++)
            {
                starsContainer.GetChild(i).gameObject.SetActive(false);
            }
            
            // 激活对应星级的对象
            if (stars >= 0 && stars <= 3 && stars < starsContainer.childCount)
            {
                starsContainer.GetChild(stars).gameObject.SetActive(true);
            }
        }
    }
    
    private void OnLevelSelected()
    {
        // 添加调试日志
        Debug.Log($"关卡 {levelId} 被点击!");
        
        // 通知关卡选择面板
        LevelSelectionPanel panel = GetComponentInParent<LevelSelectionPanel>();
        if (panel != null)
        {
            panel.SelectLevel(levelId);
        }
    }
}
