using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// 防御塔建造节点，挂载在每个塔节点上
/// </summary>
public class TowerBuildNode : MonoBehaviour
{
    [Header("塔信息")] public TowerType towerType; // 塔的类型

    [Header("节点组件")] [SerializeField] private GameObject lightState; // 可建造状态
    [SerializeField] private GameObject darkState; // 不可建造状态
    [SerializeField] private Text costText; // 花费文本

    private TowerManager.TowerData towerData
    {
        get { return TowerManager.Instance.GetTowerData(towerType); }
    }

    private Button button; // 按钮组件

    // 点击事件
    public UnityAction<TowerType, int> OnNodeClicked;

    private void Awake()
    {
        // 获取按钮组件
        button = GetComponent<Button>();
        // towerData = TowerManager.Instance.GetTowerData(towerType);

        // 查找未指定的组件
        if (lightState == null)
            lightState = transform.Find("Light")?.gameObject;

        if (darkState == null)
            darkState = transform.Find("Dark")?.gameObject;

        if (costText == null)
            costText = transform.Find("Cost")?.GetComponent<Text>();

        // 更新成本文本
        if (costText != null)
            costText.text = towerData.cost.ToString();

        // 添加按钮点击事件
        if (button != null)
            button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        // 触发点击事件回调
        OnNodeClicked?.Invoke(towerType, towerData.cost);
    }

    /// <summary>
    /// 更新节点状态
    /// </summary>
    public void UpdateState(int playerCurrency)
    {
        bool canAfford = playerCurrency >= towerData.cost;

        // 更新Light/Dark状态
        if (lightState != null)
            lightState.SetActive(canAfford);

        if (darkState != null)
            darkState.SetActive(!canAfford);

        // 更新按钮可交互性
        if (button != null)
            button.interactable = canAfford;
    }
}