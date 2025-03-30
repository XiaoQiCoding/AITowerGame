// GameData.cs - 定义需要保存的游戏数据

using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    // 玩家进度
    public int maxUnlockedLevel = 1;
    public Dictionary<int, int> levelStars = new Dictionary<int, int>();
    
    // 扩展：可以添加其他需要保存的数据
    public int totalCurrency = 0;
    public int totalEnemiesDefeated = 0;
    public float totalPlayTime = 0f;
    
    // 构造新游戏存档
    public static GameData CreateNewGame()
    {
        GameData data = new GameData
        {
            maxUnlockedLevel = 1,
            levelStars = new Dictionary<int, int>(),
            totalCurrency = 0,
            totalEnemiesDefeated = 0,
            totalPlayTime = 0f
        };
        return data;
    }
}

// 由于Dictionary不能直接被JSON序列化，创建辅助类
[System.Serializable]
public class SerializableDictionary<TKey, TValue>
{
    [System.Serializable]
    public class KeyValuePair
    {
        public TKey Key;
        public TValue Value;
    }

    public List<KeyValuePair> List = new List<KeyValuePair>();

    // 转换为Dictionary
    public Dictionary<TKey, TValue> ToDictionary()
    {
        Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
        foreach (var pair in List)
        {
            dict[pair.Key] = pair.Value;
        }
        return dict;
    }

    // 从Dictionary创建
    public static SerializableDictionary<TKey, TValue> FromDictionary(Dictionary<TKey, TValue> dict)
    {
        SerializableDictionary<TKey, TValue> serDict = new SerializableDictionary<TKey, TValue>();
        foreach (var kvp in dict)
        {
            serDict.List.Add(new KeyValuePair { Key = kvp.Key, Value = kvp.Value });
        }
        return serDict;
    }
}
