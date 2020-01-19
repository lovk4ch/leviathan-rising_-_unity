using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RaptorController : EnemyController
{
    private const int WALK = 0, RUN = 1, ATTACK = 2, HIT = 3, DEATH = 4;
    private const string actionStateName = "state";

    private readonly float
        m_map_center = (Consts._max_terrain_ + Consts._min_terrain_) / 2,
        m_map_radius = (Consts._max_terrain_ - Consts._min_terrain_) / 2;
    private bool isTurn;
    private Vector3 beacon;
    private Quaternion boundaryTurnAngle;

    private int ActionState
    {
        get => animator.GetInteger(actionStateName);
        set => animator.SetInteger(actionStateName, value);
    }

    [SerializeField]
    private float m_attentionDistance = 30;
    [SerializeField]
    private float m_attackDistance = 5;
    [SerializeField]
    private float m_tankHitSpeed = 10;

    private bool isAttack;
    public override bool IsAttack {
        get {
            if (ActionState == ATTACK && isAttack)
            {
                isAttack = false;
                return true;
            }
            return false;
        }
        set => isAttack = value;
    }

    private Animator animator;

    [SerializeField]
    private float m_speed = 2f;
    [SerializeField]
    private float m_rotationSpeed = 2f;
    [SerializeField]
    private Transform m_centerOfMass = null;

    protected override void OnChange()
    {
        base.OnChange();

        if (MenuManager.IsGame)
            ActionState = 0;
    }

    protected override void Awake()
    {
        base.Awake();

        GetComponent<AudioSource>().pitch = Random.Range(1.15f, 1.95f);
        rigidbody.centerOfMass = m_centerOfMass.position;
        animator = GetComponent<Animator>();
        StartCoroutine(CheckBounds());
    }

    protected override void Update()
    {
        base.Update();
        float delta = Time.deltaTime;

        if (ActionState == WALK)
        {
            Walk(delta);
        }
        else if (ActionState == RUN)
        {
            Run(delta);
        }
        else if (ActionState == ATTACK)
        {
            Attack(delta);
        }
    }

    public override void SetTarget(UnitController target)
    {
        base.SetTarget(target);

        if (!target) {
            ActionState = 0;
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (collision.gameObject.GetComponent<TankController>() is TankController tank
            && tank.Speed > m_tankHitSpeed
            && ActionState != HIT
            && ActionState != DEATH)
        {
            Vector3 force = tank.transform.forward * tank.Speed * 3;
            Hit(Mathf.Pow(tank.Speed, 2), force);
            ActionState = HIT;
        }
    }

    public override void Hit(float damage)
    {
        base.Hit(damage);

        if (health == 0 && ActionState != DEATH)
        {
            ActionState = DEATH;
            StartCoroutine(Die());
        }
        else if (ActionState == WALK)
        {
            ActionState = RUN;
        }
    }

    protected override void OnDestroy()
    {
        if (!Scenario.Instance || !UnitManager.Instance)
            return;

        base.OnDestroy();

        UnitManager.Instance.Attack();
        Scenario.Instance.raptorsCurrentCount--;
    }

    private void TryAttack()
    {
        if (translation.magnitude > m_attackDistance) {
            if (translation.magnitude > m_attentionDistance)
                ActionState = WALK;
            else
                ActionState = RUN;

            IsAttack = false;
        }
        else
        {
            IsAttack = true;
        }
    }

    private void Walk(float delta)
    {
        if (isTurn)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, boundaryTurnAngle, m_rotationSpeed / 4 * delta);
            if (Mathf.Abs(transform.eulerAngles.y - boundaryTurnAngle.eulerAngles.y) < 1)
            {
                isTurn = false;
            }
        }

        if (translation.magnitude < m_attentionDistance && target)
        {
            ActionState = RUN;
            return;
        }

        transform.Translate(transform.forward * m_speed * delta, Space.World);
    }

    private void Run(float delta)
    {
        transform.Translate(transform.forward * m_speed * delta * 3, Space.World);

        var rotation = Quaternion.LookRotation(translation);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, m_rotationSpeed * 3 * delta);

        if (translation.magnitude < m_attackDistance && target)
        {
            ActionState = ATTACK;
            IsAttack = true;
        }
    }

    private void Attack(float delta)
    {
        var rotation = Quaternion.LookRotation(translation);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, m_rotationSpeed * delta);
    }

    private IEnumerator Die()
    {
        rigidbody.constraints = RigidbodyConstraints.None;
        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();
        yield return new WaitForSeconds(audio.clip.length);

        Destroy();
    }

    private IEnumerator CheckBounds()
    {
        float delta = 0.1f;

        while (true)
        {
            bool isHover = Consts.Horizontal((beacon - transform.position)).magnitude < m_speed / 5 * delta;

            bool isOutOfBoundary = Mathf.Abs(m_map_center - transform.position.z) > m_map_radius
                || Mathf.Abs(m_map_center - transform.position.x) > m_map_radius;

            if (isOutOfBoundary || isHover)
            {
                if (!isTurn)
                {
                    Vector3 look = new Vector3(m_map_center + Random.Range(-30, 30), transform.position.y,
                        m_map_center + Random.Range(-30, 30));
                    boundaryTurnAngle = Quaternion.LookRotation(look - transform.position);
                    isTurn = true;
                }
            }
            beacon = transform.position;
            yield return new WaitForSeconds(delta);
        }
    }
}