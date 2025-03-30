// SaveLoadManager.cs - 处理保存和加载游戏数据

using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }
    
    private const string SAVE_FILE_NAME = "towerdefense_save.json";
    private const string SAVE_KEY = "TD_GAMEDATA";
    
    private GameData _gameData;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // 加载游戏数据
    public GameData LoadGame()
    {
        if (_gameData != null)
            return _gameData;
            
        try
        {
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                string json = PlayerPrefs.GetString(SAVE_KEY);
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                
                // 转换回标准数据结构
                _gameData = new GameData
                {
                    maxUnlockedLevel = saveData.maxUnlockedLevel,
                    totalCurrency = saveData.totalCurrency,
                    totalEnemiesDefeated = saveData.totalEnemiesDefeated,
                    totalPlayTime = saveData.totalPlayTime,
                    levelStars = saveData.levelStars.ToDictionary()
                };
                
                Debug.Log("游戏数据加载成功");
                return _gameData;
            }
            else
            {
                Debug.Log("没有找到存档，创建新游戏");
                _gameData = GameData.CreateNewGame();
                SaveGame(_gameData);
                return _gameData;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("加载游戏数据时出错: " + e.Message);
            _gameData = GameData.CreateNewGame();
            return _gameData;
        }
    }
    
    // 保存游戏数据
    public void SaveGame(GameData data)
    {
        try
        {
            _gameData = data;
            
            // 转换为可序列化的格式
            SaveData saveData = new SaveData
            {
                maxUnlockedLevel = data.maxUnlockedLevel,
                totalCurrency = data.totalCurrency,
                totalEnemiesDefeated = data.totalEnemiesDefeated,
                totalPlayTime = data.totalPlayTime,
                levelStars = SerializableDictionary<int, int>.FromDictionary(data.levelStars)
            };
            
            string json = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();
            
            Debug.Log("游戏数据保存成功");
        }
        catch (System.Exception e)
        {
            Debug.LogError("保存游戏数据时出错: " + e.Message);
        }
    }
    
    // 可序列化的存档数据结构
    [System.Serializable]
    private class SaveData
    {
        public int maxUnlockedLevel = 1;
        public SerializableDictionary<int, int> levelStars = new SerializableDictionary<int, int>();
        public int totalCurrency = 0;
        public int totalEnemiesDefeated = 0;
        public float totalPlayTime = 0f;
    }
}
