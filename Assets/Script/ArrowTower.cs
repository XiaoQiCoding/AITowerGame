// 箭塔

using UnityEngine;

public class ArrowTower : Tower
{
    public GameObject arrowPrefab;

    protected override void Attack()
    {
        base.Attack();

        // 生成箭矢
        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        Projectile projectile = arrow.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.Initialize(target, attackDamage);
        }
    }

    protected override void SetTowerProperties()
    {
        base.SetTowerProperties();
        // 箭塔：物理伤害，可攻击飞行目标
        dealsMagicDamage = false;
        canTargetFlying = true;
    }
}