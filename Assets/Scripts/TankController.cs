using UnityEngine;
using static MenuManager;

[RequireComponent(typeof(Rigidbody))]
public class TankController : UnitController
{
    protected KeyAction spawn;
    protected KeyAction turretUp;
    protected KeyAction turretDown;
    protected KeyAction turretLeft;
    protected KeyAction turretRight;
    protected KeyAction turretUpSoundStop;
    protected KeyAction turretDownSoundStop;

    [SerializeField]
    protected Transform m_centerOfMass;
    [SerializeField]
    protected GameObject turret;
    [SerializeField]
    protected float turretInclineSpeed;
    [SerializeField]
    protected AudioSource turretInclineSound;

    public Gun Gun => turret.GetComponent<Gun>();
    public float Speed => rigidbody.velocity.magnitude;

    protected virtual void SetInput()
    {
        spawn = new KeyAction(KeyInputMode.KeyDown, InputManager.SPAWN_PLAYER_KEY, Destroy, Mode.Game);
        InputManager.Instance.AddKeyAction(spawn);

        turretUp = new KeyAction(KeyInputMode.KeyPressed, InputManager.TURRET_UP_KEY, TurretUp, Mode.Game);
        InputManager.Instance.AddKeyAction(turretUp);

        turretDown = new KeyAction(KeyInputMode.KeyPressed, InputManager.TURRET_DOWN_KEY, TurretDown, Mode.Game);
        InputManager.Instance.AddKeyAction(turretDown);

        turretLeft = new KeyAction(KeyInputMode.KeyPressed, InputManager.TURRET_LEFT_KEY, TurretLeft, Mode.Game);
        InputManager.Instance.AddKeyAction(turretLeft);

        turretRight = new KeyAction(KeyInputMode.KeyPressed, InputManager.TURRET_RIGHT_KEY, TurretRight, Mode.Game);
        InputManager.Instance.AddKeyAction(turretRight);

        turretUpSoundStop = new KeyAction(KeyInputMode.KeyUp, InputManager.TURRET_UP_KEY, TurretSoundStop, Mode.Game);
        InputManager.Instance.AddKeyAction(turretUpSoundStop);

        turretDownSoundStop = new KeyAction(KeyInputMode.KeyUp, InputManager.TURRET_DOWN_KEY, TurretSoundStop, Mode.Game);
        InputManager.Instance.AddKeyAction(turretDownSoundStop);
    }

    protected override void OnChange()
    {
        base.OnChange();
        switch (MenuManager.Instance._Mode)
        {
            case Mode.Game:
                UnitManager.Instance.SetTarget(this);
                return;
            case Mode.View:
                UnitManager.Instance.SetTarget(null);
                rigidbody.velocity = Vector3.zero;
                return;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        rigidbody.centerOfMass = m_centerOfMass.position;
        SetInput();
        UnitManager.Instance.SetTarget(this);
    }

    public virtual void TurretUp() { }
    public virtual void TurretDown() { }
    public virtual void TurretLeft() { }
    public virtual void TurretRight() { }

    protected void TurretSoundPlay()
    {
        if (turretInclineSound && !turretInclineSound.isPlaying) {
            turretInclineSound.Play();
        }
    }

    protected void TurretSoundStop()
    {
        if (turretInclineSound && turretInclineSound.isPlaying) {
            turretInclineSound.Stop();
        }
    }

    public void SetToAttack(Transform other)
    {
        transform.position = Consts.Horizontal(other.position + other.forward * 26.5f);
        transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
    }

    public override void Hit(float damage)
    {
        base.Hit(damage);

        if (health == 0 && Gun.enabled)
            Destroy();
    }

    public override void Destroy()
    {
        Gun.enabled = false;
        base.Destroy();
    }

    protected override void OnDestroy()
    {
        if (!InputManager.Instance || !Scenario.Instance)
            return;

        base.OnDestroy();

        if (!IsNewGame) {
            Scenario.Instance.CreatePlayer();
        }

        InputManager.Instance.RemoveKeyActions(this);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (IsProjectile(collision) is ProjectileMoveScript pms && pms.isEnemyProjectile) {
            Hit(pms);
        }
        else if (collision.gameObject.GetComponent<EnemyController>() is var enemy)
        {
            if (enemy && enemy.IsAttack && IsGame) {
                Hit(enemy.Damage);
            }
        }
    }
}