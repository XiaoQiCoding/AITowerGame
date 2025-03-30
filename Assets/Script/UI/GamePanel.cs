using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏内界面控制器，负责显示游戏中的关键信息（金币、生命值、波次）
/// </summary>
public class GamePanel : BasePanel
{
    [Header("Coin Panel")]
    [SerializeField] private GameObject coinPnl;
    [SerializeField] private Text coinNumText;
    
    [Header("Life Panel")]
    [SerializeField] private GameObject lifePnl;
    [SerializeField] private Text lifeNumText;
    
    [Header("Wave Panel")]
    [SerializeField] private GameObject wavePnl;
    [SerializeField] private Text waveNumText;
    
    // 上次显示的值，用于检测变化
    private int lastCoinValue = -1;
    private int lastLifeValue = -1;
    private int lastWaveValue = -1;
    private int totalWaves = 20; // 默认值，实际应从关卡配置中获取
    
    protected override void OnInit()
    {
        base.OnInit();
        
        // 如果在Inspector中未指定，尝试查找UI组件
        if (coinNumText == null && coinPnl != null)
            coinNumText = coinPnl.transform.Find("Num")?.GetComponent<Text>();
            
        if (lifeNumText == null && lifePnl != null)
            lifeNumText = lifePnl.transform.Find("Num")?.GetComponent<Text>();
            
        if (waveNumText == null && wavePnl != null)
            waveNumText = wavePnl.transform.Find("Num")?.GetComponent<Text>();
    }
    
    protected override void OnOpen()
    {
        base.OnOpen();
        
        // 重置上次显示的值，强制更新所有UI
        lastCoinValue = -1;
        lastLifeValue = -1;
        lastWaveValue = -1;
        
        // 立即更新显示
        UpdateUI();
    }
    
    private void Update()
    {
        // 仅在游戏运行状态下更新UI
        if (GameManager.Instance.CurrentState == GameManager.GameState.InGame)
        {
            UpdateUI();
        }
    }
    
    /// <summary>
    /// 更新所有UI元素
    /// </summary>
    public void UpdateUI()
    {
        UpdateCoinUI();
        UpdateLifeUI();
        UpdateWaveUI();
    }
    
    /// <summary>
    /// 更新金币显示
    /// </summary>
    private void UpdateCoinUI()
    {
        int currentCoins = GameManager.Instance.Currency;
        
        // 仅在值变化时更新UI
        if (currentCoins != lastCoinValue)
        {
            if (coinNumText != null)
            {
                coinNumText.text = currentCoins.ToString();
                
                // 可选：添加金币变化动画效果
                if (lastCoinValue != -1)
                {
                    bool isIncrease = currentCoins > lastCoinValue;
                    AnimateCoinChange(isIncrease);
                }
            }
            
            lastCoinValue = currentCoins;
        }
    }
    
    /// <summary>
    /// 更新生命值显示
    /// </summary>
    private void UpdateLifeUI()
    {
        int currentLife = GameManager.Instance.CurrentHealth;
        
        // 仅在值变化时更新UI
        if (currentLife != lastLifeValue)
        {
            if (lifeNumText != null)
            {
                lifeNumText.text = currentLife.ToString();
                
                // 可选：添加生命值变化动画效果
                if (lastLifeValue != -1)
                {
                    bool isIncrease = currentLife > lastLifeValue;
                    AnimateLifeChange(isIncrease);
                }
            }
            
            lastLifeValue = currentLife;
        }
    }
    
    /// <summary>
    /// 更新波次显示
    /// </summary>
    private void UpdateWaveUI()
    {
        // 从关卡控制器获取当前波次信息
        int currentWave = 0;
        
        // 尝试获取LevelController的波次信息
        if (LevelController.Instance != null)
        {
            // 假设LevelController有currentWave和levelConfig.waves.Count属性
            // 实际代码需要根据LevelController的实现调整
            currentWave = LevelController.Instance.currentWave; // +1因为通常从0开始索引
            
            if (LevelController.Instance.levelConfig != null && 
                LevelController.Instance.levelConfig.waves != null)
            {
                totalWaves = LevelController.Instance.levelConfig.waves.Count;
            }
        }
        
        // 仅在值变化时更新UI
        if (currentWave != lastWaveValue)
        {
            if (waveNumText != null)
            {
                waveNumText.text = $"波次：{currentWave}/{totalWaves}";
                
                // 可选：添加波次变化动画效果
                if (lastWaveValue != -1 && currentWave > lastWaveValue)
                {
                    AnimateWaveChange();
                }
            }
            
            lastWaveValue = currentWave;
        }
    }
    
    /// <summary>
    /// 金币变化动画效果
    /// </summary>
    private void AnimateCoinChange(bool isIncrease)
    {
        // TODO: 实现金币变化的视觉反馈
        // 例如：颜色闪烁、缩放动画等
        
        if (coinNumText != null)
        {
            // 设置临时颜色以指示增加或减少
            coinNumText.color = isIncrease ? Color.green : Color.red;
            
            // 使用协程恢复正常颜色
            StartCoroutine(ResetTextColor(coinNumText));
        }
    }
    
    /// <summary>
    /// 生命值变化动画效果
    /// </summary>
    private void AnimateLifeChange(bool isIncrease)
    {
        // TODO: 实现生命值变化的视觉反馈
        // 例如：颜色闪烁、缩放动画、屏幕闪红等
        
        if (lifeNumText != null)
        {
            // 设置临时颜色以指示增加或减少
            lifeNumText.color = isIncrease ? Color.green : Color.red;
            
            // 使用协程恢复正常颜色
            StartCoroutine(ResetTextColor(lifeNumText));
        }
    }
    
    /// <summary>
    /// 波次变化动画效果
    /// </summary>
    private void AnimateWaveChange()
    {
        // TODO: 实现波次变化的视觉反馈
        // 例如：波次文本放大效果、闪烁等
        
        if (waveNumText != null)
        {
            // 简单的缩放动画示例
            waveNumText.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            
            // 使用协程恢复正常大小
            StartCoroutine(ResetTextScale(waveNumText.transform));
        }
    }
    
    /// <summary>
    /// 重置文本颜色的协程
    /// </summary>
    private System.Collections.IEnumerator ResetTextColor(Text text)
    {
        yield return new WaitForSeconds(0.3f); // 等待0.3秒
        text.color = Color.white; // 恢复为原始颜色
    }
    
    /// <summary>
    /// 重置Transform缩放的协程
    /// </summary>
    private System.Collections.IEnumerator ResetTextScale(Transform textTransform)
    {
        yield return new WaitForSeconds(0.3f); // 等待0.3秒
        textTransform.localScale = Vector3.one; // 恢复正常大小
    }
    
    /// <summary>
    /// 显示暂停菜单
    /// </summary>
    public void ShowPauseMenu()
    {
        UIManager.Instance.ShowPausePanel();
    }
}
