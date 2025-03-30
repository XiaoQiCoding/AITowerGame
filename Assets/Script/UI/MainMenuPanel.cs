using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 主菜单面板控制器
/// </summary>
public class MainMenuPanel : BasePanel
{
    [SerializeField] private Button btnStart;  // 开始游戏按钮
    [SerializeField] private Button btnQuit;   // 退出游戏按钮
    
    protected override void OnInit()
    {
        base.OnInit();
        
        // 查找按钮组件（如果未在Inspector中分配）
        if (btnStart == null)
            btnStart = transform.Find("BtnStart")?.GetComponent<Button>();
            
        if (btnQuit == null)
            btnQuit = transform.Find("BtnQuit")?.GetComponent<Button>();
        
        // 添加按钮事件监听
        if (btnStart != null)
            btnStart.onClick.AddListener(OnStartButtonClicked);
            
        if (btnQuit != null)
            btnQuit.onClick.AddListener(OnQuitButtonClicked);
    }
    
    protected override void OnOpen()
    {
        base.OnOpen();
        
        // 可以在这里添加面板打开时的动画或音效
        // 例如：播放背景音乐、执行入场动画等
    }
    
    /// <summary>
    /// 开始按钮点击处理
    /// </summary>
    private void OnStartButtonClicked()
    {
        // 调用UIManager显示关卡选择面板
        UIManager.Instance.ShowLevelSelection();
        
        // 可选：播放按钮点击音效
        PlayButtonSound();
    }
    
    /// <summary>
    /// 退出按钮点击处理
    /// </summary>
    private void OnQuitButtonClicked()
    {
        // 可选：播放按钮点击音效
        PlayButtonSound();
        
        // 显示确认对话框（如果有）或直接退出
        QuitGame();
    }
    
    /// <summary>
    /// 播放按钮音效
    /// </summary>
    private void PlayButtonSound()
    {
        // 如果有音频管理器，可以调用它播放音效
        // 例如：AudioManager.Instance.PlaySound("ButtonClick");
    }
    
    /// <summary>
    /// 退出游戏
    /// </summary>
    private void QuitGame()
    {
#if UNITY_EDITOR
        // 在Unity编辑器中停止播放
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 在实际构建的应用程序中退出
        Application.Quit();
#endif
    }
}
