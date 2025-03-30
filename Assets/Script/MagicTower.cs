using UnityEngine;

public class MagicTower : Tower
{
    public GameObject magicPrefab; // 魔法弹预制体
    public GameObject hitEffectPrefab; // 命中效果预制体

    protected override void SetTowerProperties()
    {
        base.SetTowerProperties();
        // 魔法塔：魔法伤害，可攻击飞行目标
        dealsMagicDamage = true;
        canTargetFlying = true;
    }

    protected override void Attack()
    {
        base.Attack();

        if (target != null)
        {
            // 生成魔法弹
            GameObject magic = Instantiate(magicPrefab, transform.position, Quaternion.identity);
            MagicProjectile projectile = magic.GetComponent<MagicProjectile>();

            if (projectile == null)
            {
                projectile = magic.AddComponent<MagicProjectile>();
            }

            // 初始化魔法弹
            if (projectile != null)
            {
                projectile.hitEffectPrefab = hitEffectPrefab;
                projectile.isMagicDamage = dealsMagicDamage; // 设置伤害类型
                projectile.Initialize(target, attackDamage);
            }
        }
    }
}