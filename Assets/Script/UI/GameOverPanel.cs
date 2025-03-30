using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 游戏结束面板控制器
/// </summary>
public class GameOverPanel : BasePanel
{
    [SerializeField] private Text resultText; // 结果文本（胜利/失败）
    [SerializeField] private Text starsText; // 星级文本
    [SerializeField] private GameObject[] starObjects; // 星星对象数组
    public Button btnSure;

    private int earnedStars = 0;
    private bool isVictory = false;

    protected override void OnInit()
    {
        base.OnInit();
        btnSure.onClick.AddListener(ReturnToMainMenu);
    }

    protected override void OnOpen()
    {
        base.OnOpen();
        UpdateStarsDisplay();
    }

    /// <summary>
    /// 设置游戏结果
    /// </summary>
    public void SetResult(bool victory, int remainingHealth, int stars)
    {
        isVictory = victory;
        earnedStars = stars; // 直接使用传入的星级，确保和GameManager计算一致

        // 更新UI显示
        if (resultText != null)
        {
            resultText.text = victory ? "胜利!" : "失败!";
        }

        if (starsText != null)
        {
            starsText.text = $"{stars}";
        }

        UpdateStarsDisplay();
    }

    /// <summary>
    /// 更新星星显示
    /// </summary>
    private void UpdateStarsDisplay()
    {
        if (starObjects == null || starObjects.Length == 0) return;

        // 激活对应数量的星星
        for (int i = 0; i < starObjects.Length; i++)
        {
            if (starObjects[i] != null)
            {
                starObjects[i].SetActive(false);
            }
        }
        starObjects[earnedStars].SetActive(true);
    }

    /// <summary>
    /// 返回主菜单
    /// </summary>
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Menu");
        UIManager.Instance.ShowLevelSelection();
    }

    /// <summary>
    /// 重玩当前关卡
    /// </summary>
    public void RestartLevel()
    {
        GameManager.Instance.StartLevel(GameManager.Instance.CurrentLevel);
    }

    /// <summary>
    /// 进入下一关卡
    /// </summary>
    public void NextLevel()
    {
        int nextLevel = GameManager.Instance.CurrentLevel + 1;
        if (nextLevel <= GameManager.Instance.MaxUnlockedLevel)
        {
            GameManager.Instance.StartLevel(nextLevel);
        }
        else
        {
            UIManager.Instance.ShowLevelSelection();
        }
    }
}