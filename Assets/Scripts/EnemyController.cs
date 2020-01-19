using UnityEngine;

public class EnemyController : UnitController
{
    [SerializeField]
    protected float damage;
    protected Vector3 translation;

    public float Damage { get => damage; }
    public virtual bool IsAttack { get; set; }
    public virtual Collider SkeletonCollider { get; set; }

    protected override void Awake()
    {
        base.Awake();
        SkeletonCollider = _skeleton.GetComponent<Collider>();
    }

    protected override void Update()
    {
        // fixed update
        base.Update();

        if (target) {
            translation = Consts.Horizontal(target.transform.position - transform.position);
        }
        else {
            translation = transform.forward;
        }

        // update
        if (SkeletonCollider && HealthBar is SimpleHealthBar)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            HealthBar.gameObject.SetActive(GeometryUtility.TestPlanesAABB(planes, SkeletonCollider.bounds));
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (IsProjectile(collision) is ProjectileMoveScript pms && !pms.isEnemyProjectile) {
            Hit(pms);
        }
    }
}