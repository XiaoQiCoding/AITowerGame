using UnityEngine;

/// <summary>
/// 所有UI面板的基类，提供基础功能和生命周期管理
/// </summary>
public abstract class BasePanel : MonoBehaviour
{
    protected bool isInitialized = false;

    protected virtual void Awake()
    {
        if (!isInitialized)
        {
            OnInit();
            isInitialized = true;
        }
    }

    protected virtual void OnEnable()
    {
        OnOpen();
    }

    protected virtual void OnDisable()
    {
        OnClose();
    }

    /// <summary>
    /// 初始化面板，只在第一次调用时执行
    /// </summary>
    protected virtual void OnInit()
    {
        // 子类重写此方法以进行初始化
    }

    /// <summary>
    /// 面板打开时调用
    /// </summary>
    protected virtual void OnOpen()
    {
        // 子类重写此方法以处理面板打开逻辑
    }

    /// <summary>
    /// 面板关闭时调用
    /// </summary>
    protected virtual void OnClose()
    {
        // 子类重写此方法以处理面板关闭逻辑
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}
