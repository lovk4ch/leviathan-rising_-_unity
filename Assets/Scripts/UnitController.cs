using System.Collections;
using UnityEngine;

public abstract class UnitController : MonoBehaviour, IPlayerObserver
{
    protected HealthBar healthBar;
    public HealthBar HealthBar { get => healthBar; set => healthBar = value; }

    [SerializeField]
    protected float maxHealth, health;
    public float HealthPercentage { get { return health / maxHealth; } }

    protected new Rigidbody rigidbody;
    protected UnitController target;

    protected bool isInvulnerable;

    /// <summary>
    /// Component for calculating the center of an animated object
    /// and falling into the camera’s field of view
    /// </summary>
    [SerializeField]
    private Transform skeleton = null;
    public Transform _skeleton { get => skeleton; }

    protected virtual void OnChange()
    {
        isInvulnerable = !MenuManager.IsGame;
    }

    protected virtual void Awake()
    {
        health = maxHealth;

        HealthBar = Scenario.Instance.GetHealthBar(this);
        HealthBar.Initialize(this);

        UnitManager.Instance.Add(this);
        rigidbody = GetComponent<Rigidbody>();
        MenuManager.Instance.OnChange += OnChange;
    }

    protected virtual void Update()
    {
        if (transform.position.y < -3 && health > 0)
            Hit(health);
    }

    protected virtual void OnEnable()
    {
        if (HealthBar) {
            HealthBar.gameObject.SetActive(true);
        }
    }

    protected virtual void OnDisable()
    {
        if (HealthBar) {
            HealthBar.gameObject.SetActive(false);
        }
    }

    public virtual void SetTarget(UnitController target)
    {
        this.target = target;
    }

    private IEnumerator Shock(float damage, float time)
    {
        while (time > 0)
        {
            time -= Time.fixedDeltaTime;
            Hit(damage);
            yield return new WaitForFixedUpdate();
        }
    }

    public virtual void Hit(float damage)
    {
        if (isInvulnerable) {
            damage = 0;
        }

        if (health > damage)
            health -= damage;
        else
            health = 0;
    }

    public void Hit(float damage, Vector3 force)
    {
        Hit(damage);
        rigidbody.AddForce(force, ForceMode.VelocityChange);
    }

    protected void Hit(ProjectileMoveScript pms)
    {
        if (pms is GuidedProjectileMoveScript)
            StartCoroutine(Shock(pms.damage, pms.shockTime));
        else
            Hit(pms.damage);
    }

    protected virtual void OnDestroy()
    {
        if (!InputManager.Instance || !MenuManager.Instance)
            return;

        if (HealthBar) {
            Destroy(HealthBar.gameObject);
        }

        UnitManager.Instance.Remove(this);
        MenuManager.Instance.OnChange -= OnChange;
    }

    public virtual void Destroy()
    {
        Scenario.Instance.Annihilate(gameObject);
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    protected ProjectileMoveScript IsProjectile(Collision collision)
    {
        if (collision.collider.CompareTag("Projectile") && collision.gameObject.GetComponent<ProjectileMoveScript>() is var pms) {
            return pms;
        }
        return null;
    }
}