// 塔建造管理器

using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance { get; private set; }

    [System.Serializable]
    public class TowerData
    {
        public TowerType towerType;
        public GameObject towerPrefab;
        public int cost;
        public Sprite icon;
    }

    public List<TowerData> availableTowers = new List<TowerData>();

    private void Awake()
    {
        Instance = this;
    }

    public bool CanBuildTower(TowerType type)
    {
        TowerData towerData = GetTowerData(type);
        return towerData != null && GameManager.Instance.Currency >= towerData.cost;
    }

    public bool BuildTower(TowerType type, Vector3 position)
    {
        if (!CanBuildTower(type))
            return false;

        TowerData towerData = GetTowerData(type);
        GameManager.Instance.Currency -= towerData.cost;

        GameObject towerObj = Instantiate(towerData.towerPrefab, position, Quaternion.identity);
        return true;
    }

    public TowerData GetTowerData(TowerType type)
    {
        return availableTowers.Find(t => t.towerType == type);
    }
}
