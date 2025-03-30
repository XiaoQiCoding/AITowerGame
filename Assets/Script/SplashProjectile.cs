// 溅射伤害弹射物 - 重写为抛物线运动

using UnityEngine;

public class SplashProjectile : Projectile
{
    public float speed = 10f;
    public float arcHeight = 2f; // 抛物线高度
    public float lifetime = 3f;
    public GameObject explosionEffectPrefab; // 爆炸效果预制件

    private Transform target;
    private Vector3 targetPosition;
    private int damage;
    private float splashRadius;
    private Vector3 startPos;
    private float journeyLength;
    private float startTime;
    private bool targetLost = false;
    [HideInInspector] public bool isMagicDamage = false;


    public void Initialize(Transform target, int damage, float splashRadius)
    {
        this.target = target;
        this.damage = damage;
        this.splashRadius = splashRadius;

        startPos = transform.position;

        if (target != null)
        {
            targetPosition = target.position;
            journeyLength = Vector3.Distance(startPos, targetPosition);
        }
        else
        {
            targetLost = true;
            Destroy(gameObject);
            return;
        }

        startTime = Time.time;
        Destroy(gameObject, lifetime); // 安全机制，确保最终被销毁
    }

    protected override void Update()
    {
        // 如果目标丢失但我们有记录的位置，继续飞向该位置
        if (target == null && !targetLost)
        {
            targetLost = true;
            // 继续使用最后记录的targetPosition
        }
        else if (!targetLost)
        {
            // 更新目标位置
            targetPosition = target.position;
            // 重新计算距离
            journeyLength = Vector3.Distance(startPos, targetPosition);
        }

        // 计算飞行进度
        float distCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distCovered / journeyLength;

        if (fractionOfJourney >= 1.0f)
        {
            // 到达目标位置，执行爆炸
            HitTarget();
            return;
        }

        // 计算当前位置（加入抛物线轨迹）
        Vector3 currentPos = Vector3.Lerp(startPos, targetPosition, fractionOfJourney);

        // 添加抛物线弧度 - 抛物线公式，在中间位置达到最大高度
        currentPos.y += arcHeight * Mathf.Sin(fractionOfJourney * Mathf.PI);

        // 设置位置
        transform.position = currentPos;

        // 调整炮弹方向，使它看起来是沿着抛物线飞行的
        if (fractionOfJourney < 0.5f)
        {
            // 上升阶段，炮弹朝上
            float angle = Vector2.SignedAngle(Vector2.right, new Vector2(targetPosition.x - startPos.x, arcHeight));
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // 下降阶段，炮弹朝下
            float angle = Vector2.SignedAngle(Vector2.right,
                new Vector2(targetPosition.x - currentPos.x, targetPosition.y - currentPos.y));
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    protected override void HitTarget()
    {
        // 产生爆炸效果
        if (explosionEffectPrefab != null)
        {
            GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            explosion.GetComponent<Animator>().SetTrigger("Boom");
        }

        // 产生溅射伤害
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, splashRadius);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage, isMagicDamage);
                }
            }
        }

        // 在场景中绘制爆炸范围（仅编辑器可见）
        Debug.DrawRay(transform.position, Vector3.up * splashRadius, Color.red, 1.0f);
        Debug.DrawRay(transform.position, Vector3.down * splashRadius, Color.red, 1.0f);
        Debug.DrawRay(transform.position, Vector3.left * splashRadius, Color.red, 1.0f);
        Debug.DrawRay(transform.position, Vector3.right * splashRadius, Color.red, 1.0f);

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        // 在场景视图中显示溅射范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, splashRadius);
    }
}