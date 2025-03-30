using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 敌人对象池
public class EnemyPoolManager : MonoBehaviour
{
    public static EnemyPoolManager Instance { get; private set; }
    
    [System.Serializable]
    public class EnemyPoolItem
    {
        public EnemyType enemyType;
        public GameObject enemyPrefab;
        // public EnemyConfig config;  // 引用敌人配置
        public int poolSize = 10;
        
        [HideInInspector]
        public List<GameObject> pooledObjects = new List<GameObject>();
    }
    
    public List<EnemyPoolItem> enemyPools = new List<EnemyPoolItem>();
    
    private void Awake()
    {
        Instance = this;
        InitializePool();
        DontDestroyOnLoad(this);
    }
    
    private void InitializePool()
    {
        foreach (var item in enemyPools)
        {
            for (int i = 0; i < item.poolSize; i++)
            {
                GameObject obj = Instantiate(item.enemyPrefab, transform);
                
                // 应用配置到敌人对象
                Enemy enemyComponent = obj.GetComponent<Enemy>();
                // if (enemyComponent != null && item.config != null)
                // {
                //     enemyComponent.config = item.config;
                // }
                
                obj.SetActive(false);
                item.pooledObjects.Add(obj);
            }
        }
    }
    
    public GameObject GetEnemy(EnemyType type)
    {
        foreach (var item in enemyPools)
        {
            if (item.enemyType == type)
            {
                foreach (var obj in item.pooledObjects)
                {
                    if (!obj.activeInHierarchy)
                    {
                        return obj;
                    }
                }
                
                // 如果没有可用的对象，创建新的
                GameObject newObj = Instantiate(item.enemyPrefab, transform);
                
                // 应用配置到新创建的敌人对象
                Enemy enemyComponent = newObj.GetComponent<Enemy>();
                // if (enemyComponent != null && item.config != null)
                // {
                //     enemyComponent.config = item.config;
                // }
                
                newObj.SetActive(false);
                item.pooledObjects.Add(newObj);
                return newObj;
            }
        }
        
        return null; // 未找到对应类型的敌人
    }
}
