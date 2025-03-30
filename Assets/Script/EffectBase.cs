using UnityEngine;

public class EffectBase : MonoBehaviour
{
    public void DestroyEffect()
    {
        GameObject.Destroy(this.gameObject);
    }
}