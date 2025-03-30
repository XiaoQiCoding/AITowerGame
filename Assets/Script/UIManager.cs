using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI管理器，用于管理所有UI面板的显示和隐藏
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panel References")] [SerializeField]
    private BasePanel mainMenuPanel; // 主菜单面板

    [SerializeField] private BasePanel levelSelectionPanel; // 关卡选择面板

    [SerializeField] private BasePanel gamePanel; // 游戏内面板

    // [SerializeField] private BasePanel pausePanel; // 暂停面板
    [SerializeField] private BasePanel gameOverPanel; // 游戏结束面板
    [SerializeField] private BasePanel buildPanel; // 建造面板

    // 所有面板的字典引用，方便按名称查找和操作
    private Dictionary<string, BasePanel> panelDictionary = new Dictionary<string, BasePanel>();

    // 当前显示的面板
    private BasePanel currentPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitPanels();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 初始化所有面板并添加到字典中
    /// </summary>
    private void InitPanels()
    {
        // 注册所有面板到字典中
        RegisterPanel("MainMenu", mainMenuPanel);
        RegisterPanel("LevelSelection", levelSelectionPanel);
        RegisterPanel("Game", gamePanel);
        // RegisterPanel("Pause", pausePanel);
        RegisterPanel("GameOver", gameOverPanel);
        RegisterPanel("Build", buildPanel);

        // 隐藏所有面板
        HideAllPanels();
    }

    /// <summary>
    /// 注册面板到字典
    /// </summary>
    private void RegisterPanel(string panelName, BasePanel panel)
    {
        if (panel != null)
        {
            panelDictionary[panelName] = panel;
            panel.Hide(); // 默认隐藏
        }
    }

    /// <summary>
    /// 显示指定名称的面板
    /// </summary>
    public void ShowPanel(string panelName, bool hideOthers = true)
    {
        if (panelDictionary.TryGetValue(panelName, out BasePanel panel))
        {
            if (hideOthers)
            {
                HideAllPanels();
            }

            panel.Show();
            currentPanel = panel;
        }
        else
        {
            Debug.LogWarning($"面板 '{panelName}' 不存在！");
        }
    }

    /// <summary>
    /// 隐藏指定名称的面板
    /// </summary>
    public void HidePanel(string panelName)
    {
        if (panelDictionary.TryGetValue(panelName, out BasePanel panel))
        {
            panel.Hide();
        }
    }

    /// <summary>
    /// 显示主菜单
    /// </summary>
    public void ShowMainMenu()
    {
        ShowPanel("MainMenu");
    }

    /// <summary>
    /// 显示关卡选择界面
    /// </summary>
    public void ShowLevelSelection()
    {
        ShowPanel("LevelSelection");

        // 更新关卡选择面板
        var levelPanel = GetPanel<LevelSelectionPanel>();
        if (levelPanel != null)
        {
            levelPanel.UpdateLevelNodes();
        }
    }

    /// <summary>
    /// 显示游戏内界面
    /// </summary>
    public void ShowGameUI()
    {
        ShowPanel("Game");

        // 更新游戏面板
        var gamePnl = GetPanel<GamePanel>();
        if (gamePnl != null)
        {
            gamePnl.UpdateUI();
        }
    }

    /// <summary>
    /// 显示暂停界面
    /// </summary>
    public void ShowPausePanel()
    {
        ShowPanel("Pause", false); // 不隐藏其他面板，叠加显示
    }

    /// <summary>
    /// 显示建造面板
    /// </summary>
    /// <summary>
    /// 显示建造面板
    /// </summary>
    public void ShowBuildPanel(Vector3 position, BuildNode buildNode = null)
    {
        var buildPanelComponent = GetPanel<BuildPanel>();
        if (buildPanelComponent != null)
        {
            buildPanelComponent.SetPosition(position, buildNode);
            ShowPanel("Build", false); // 不隐藏其他面板
        }
    }


    /// <summary>
    /// 隐藏建造面板
    /// </summary>
    public void HideBuildPanel()
    {
        HidePanel("Build");
    }

    /// <summary>
    /// 显示游戏结束面板
    /// </summary>
    public void ShowGameOverPanel(bool victory, int remainingHealth, int stars)
    {
        ShowPanel("GameOver", false); // 叠加显示

        // 通过GameOverPanel脚本设置结果
        var gameOverPanelComponent = GetPanel<GameOverPanel>();
        if (gameOverPanelComponent != null)
        {
            gameOverPanelComponent.SetResult(victory, remainingHealth, stars);
        }
    }

    /// <summary>
    /// 隐藏所有面板
    /// </summary>
    private void HideAllPanels()
    {
        foreach (var panel in panelDictionary.Values)
        {
            if (panel != null)
            {
                panel.Hide();
            }
        }
    }

    /// <summary>
    /// 获取指定类型的面板
    /// </summary>
    public T GetPanel<T>() where T : BasePanel
    {
        foreach (var panel in panelDictionary.Values)
        {
            if (panel is T typedPanel)
            {
                return typedPanel;
            }
        }

        return null;
    }

    /// <summary>
    /// 获取指定名称的面板
    /// </summary>
    public BasePanel GetPanel(string panelName)
    {
        if (panelDictionary.TryGetValue(panelName, out BasePanel panel))
        {
            return panel;
        }

        return null;
    }
}