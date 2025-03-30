// 全局游戏管理器

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // 游戏状态
    public enum GameState
    {
        MainMenu,
        LevelSelection,
        InGame,
        Paused,
        GameOver,
        Victory
    }

    public GameState CurrentState { get; private set; }

    // 游戏数据
    public int CurrentHealth { get; private set; } = 15;
    public int Currency { get; set; } = 100;
    public int CurrentLevel { get; private set; } = 0;

    // 关卡解锁进度(1-10)
    public int MaxUnlockedLevel { get; private set; } = 1;

    // 关卡星级记录 (关卡ID => 星级)
    public Dictionary<int, int> LevelStars { get; private set; } = new Dictionary<int, int>();

    // 解锁指定关卡
    public void UnlockLevel(int levelId)
    {
        if (levelId > MaxUnlockedLevel)
        {
            MaxUnlockedLevel = levelId;
            Debug.Log($"已解锁关卡 {levelId}");
            SaveGameData(); // 保存解锁状态
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameData(); // 加载游戏存档

            // 注册场景加载事件
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UIManager.Instance.ShowPanel("MainMenu");
    }

    // 场景加载完成后的回调
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"场景加载完成: {scene.name}");

        // 如果是游戏场景，显示游戏UI
        if (CurrentState == GameState.InGame)
        {
            Debug.Log("显示游戏UI...");
            UIManager.Instance.ShowGameUI();
        }
    }

    public void StartLevel(int levelId)
    {
        CurrentLevel = levelId;
        CurrentState = GameState.InGame;

        // 从Resources目录加载关卡配置
        LevelConfig levelConfig = Resources.Load<LevelConfig>($"Config/level{levelId}");

        if (levelConfig != null)
        {
            // 使用关卡配置中的初始值
            CurrentHealth = levelConfig.initialHealth;
            Currency = levelConfig.initialCurrency;
            Debug.Log($"从关卡配置读取: 初始生命值 {CurrentHealth}, 初始金币 {Currency}");
        }

        // 场景加载逻辑
        string sceneName = $"Map{levelId}";
        Debug.Log($"加载{sceneName}场景...");
        SceneManager.LoadScene(sceneName);
    }


    public void LoseHealth(int amount)
    {
        CurrentHealth -= amount;
        if (CurrentHealth <= 0)
        {
            GameOver(false);
        }
    }

    public void GameOver(bool victory)
    {
        Debug.Log("游戏结束，胜利状态: " + victory);
        CurrentState = victory ? GameState.Victory : GameState.GameOver;

        int stars = 0;
        if (victory)
        {
            // 计算星级 - 根据剩余生命值判断
            stars = CalculateStars(CurrentHealth);

            // 保存星级
            if (!LevelStars.ContainsKey(CurrentLevel) || LevelStars[CurrentLevel] < stars)
            {
                LevelStars[CurrentLevel] = stars;
            }

            // 如果获得了至少1星，则解锁下一关卡
            if (stars >= 1)
            {
                int nextLevel = CurrentLevel + 1;
                if (nextLevel <= 7) // 假设最大关卡数为7
                {
                    UnlockLevel(nextLevel);
                    Debug.Log($"已解锁下一关卡: {nextLevel}");
                }
            }

            SaveGameData(); // 保存进度
        }

        // 处理游戏结束状态
        HandleGameEnd();

        // 存储当前结果用于延迟显示
        int finalStars = stars;
        bool finalVictory = victory;

        // 使用匿名方法来传递参数
        Invoke(nameof(ShowGameOverPanel), 1.5f);
    }

    void ShowGameOverPanel()
    {
        // 显示结算界面 - 传递计算好的星级
        UIManager.Instance.ShowGameOverPanel(CurrentState == GameState.Victory, CurrentHealth,
            CalculateStars(CurrentHealth));
    }


    void DelayOpenGameOver(bool victory, int stars)
    {
        // 显示结算界面 - 传递计算好的星级
        UIManager.Instance.ShowGameOverPanel(victory, CurrentHealth, stars);
    }

    // 计算星级的统一方法
    public int CalculateStars(int remainingHealth)
    {
        // 加载当前关卡配置
        LevelConfig levelConfig = Resources.Load<LevelConfig>($"Config/level{CurrentLevel}");

        if (levelConfig != null)
        {
            int initialHealth = levelConfig.initialHealth;

            // 根据剩余生命值占初始生命值的比例计算星级
            float healthPercentage = (float)remainingHealth / initialHealth;

            if (remainingHealth <= 0) return 0;
            if (healthPercentage <= 0.4f) return 1; // 剩余少于40%生命
            if (healthPercentage <= 0.7f) return 2; // 剩余少于70%生命
            return 3; // 剩余超过70%生命
        }
        else
        {
            // 如果找不到配置，使用旧的固定阈值
            if (remainingHealth <= 0) return 0;
            if (remainingHealth <= 10) return 1;
            if (remainingHealth <= 18) return 2;
            return 3;
        }
    }


// 处理游戏结束时的各种清理工作
    private void HandleGameEnd()
    {
        // 1. 停止生成新敌人
        if (LevelController.Instance != null)
        {
            LevelController.Instance.StopSpawning();
        }

        // 2. 销毁所有现有敌人
        DestroyAllEnemies();

        // 3. 停止所有防御塔的行为
        DisableAllTowers();

        // 4. 禁用建造节点
        DisableBuildNodes();

        Debug.Log("游戏结束，所有游戏对象已处理");
    }

// 销毁所有敌人
    private void DestroyAllEnemies()
    {
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in allEnemies)
        {
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                enemy.ReturnToPool();
            }
        }

        Debug.Log($"已清理 {allEnemies.Length} 个敌人");
    }

// 停止所有防御塔行为
    private void DisableAllTowers()
    {
        Tower[] allTowers = FindObjectsOfType<Tower>();
        foreach (Tower tower in allTowers)
        {
            if (tower != null)
            {
                tower.enabled = false; // 禁用塔的脚本组件
            }
        }

        Debug.Log($"已禁用 {allTowers.Length} 个防御塔");
    }

// 禁用所有建造节点
    private void DisableBuildNodes()
    {
        BuildNode[] allNodes = FindObjectsOfType<BuildNode>();
        foreach (BuildNode node in allNodes)
        {
            if (node != null)
            {
                node.enabled = false; // 禁用建造节点的脚本组件
            }
        }

        Debug.Log($"已禁用 {allNodes.Length} 个建造节点");
    }


    private void LoadGameData()
    {
        GameData gameData = SaveLoadManager.Instance.LoadGame();

        // 从存档中更新GameManager状态
        MaxUnlockedLevel = gameData.maxUnlockedLevel;
        LevelStars = gameData.levelStars;

        Debug.Log($"已加载游戏数据: 最大解锁关卡 {MaxUnlockedLevel}, 共有星级记录 {LevelStars.Count} 个");
    }

    private void SaveGameData()
    {
        GameData gameData = new GameData
        {
            maxUnlockedLevel = MaxUnlockedLevel,
            levelStars = LevelStars,
            // 可以添加统计数据
            totalCurrency = Currency,
            totalEnemiesDefeated = 0, // 这需要另外统计
            totalPlayTime = 0f // 这需要另外统计
        };

        SaveLoadManager.Instance.SaveGame(gameData);
    }

    // 当对象销毁时取消事件注册
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}