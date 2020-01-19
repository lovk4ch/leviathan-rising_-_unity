using UnityEngine;

public class GuidedProjectileMoveScript : ProjectileMoveScript {
    protected UnitController target;

    [Tooltip("if range = 0, projectile will always find a target")]
    [SerializeField]
    [Range(0, 100)]
    protected float locationRange;

    [SerializeField]
    [Range(0, 0.5f)]
    protected float angleScatter;

    [Range(0, 5)]
    public float angleSmooth = 1f;

    public virtual void Aim(UnitController target)
    {
        angleSmooth = angleSmooth * Random.Range(angleSmooth - angleScatter, angleSmooth + angleScatter);
        this.target = target;

        if (target is EnemyController)
            transform.forward = Consts.Horizontal(transform.forward);
    }

    protected override void Update()
    {
        float delta = Time.deltaTime;
        if (target != null)
        {
            var translation = target._skeleton.position - transform.position;
            if (locationRange == 0 || (locationRange > translation.magnitude && transform.position.y > 0)) {

                angleSmooth *= (1 + 1 / translation.magnitude);
                var rotation = Quaternion.LookRotation(translation);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, angleSmooth * delta);
            }
        }
        transform.Translate(transform.forward * speed * delta, Space.World);
    }
}