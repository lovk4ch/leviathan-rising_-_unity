using UnityEngine;
using static MenuManager;

[RequireComponent(typeof(Rigidbody))]
public class SmallTankController : TankController
{
    private enum WeaponType { Laser, Heavy, Shock }
    private WeaponType weapon;

    private KeyAction changeWeapon;

    [SerializeField]
    private GameObject model = null;

    public GameObject turretHeavy;
    public GameObject turretShock;
    public GameObject turretLaser;

    private float turretAngle;
    private static float smoothTurretAngle;

    protected override void SetInput()
    {
        base.SetInput();

        changeWeapon = new KeyAction(KeyInputMode.KeyDown, InputManager.WEAPON_CHANGE_KEY, SetWeapon, Mode.Game);
        InputManager.Instance.AddKeyAction(changeWeapon);
    }

    protected override void Awake()
    {
        base.Awake();
        SetWeapon();
    }

    protected override void OnDestroy()
    {
        if (!InputManager.Instance)
            return;

        base.OnDestroy();
        InputManager.Instance.RemoveKeyActions(this);
    }

    private void SetWeapon()
    {
        DestroyImmediate(turret);
        switch (weapon)
        {
            case WeaponType.Laser:
                turret = Instantiate(turretHeavy, model.transform) as GameObject;

                if (turretAngle == default)
                    turretAngle = turret.transform.localRotation.eulerAngles.x;
                SetTurretAngle();
                weapon = WeaponType.Heavy;
                break;
            case WeaponType.Heavy:
                turret = Instantiate(turretShock, model.transform) as GameObject;
                weapon = WeaponType.Shock;
                break;
            case WeaponType.Shock:
                turret = Instantiate(turretLaser, model.transform) as GameObject;
                weapon = WeaponType.Laser;
                break;
        }
    }

    public override void TurretUp()
    {
        if (weapon == WeaponType.Heavy && smoothTurretAngle > -10)
        {
            smoothTurretAngle -= Time.deltaTime * turretInclineSpeed;
            TurretSoundPlay();
            SetTurretAngle();
            return;
        }
        TurretSoundStop();
    }

    public override void TurretDown()
    {
        if (weapon == WeaponType.Heavy && smoothTurretAngle < 0)
        {
            smoothTurretAngle += Time.deltaTime * turretInclineSpeed;
            TurretSoundPlay();
            SetTurretAngle();
            return;
        }
        TurretSoundStop();
    }

    private void SetTurretAngle()
    {
        turret.transform.localRotation = Quaternion.Euler(turretAngle + smoothTurretAngle, 0, 0);
    }
}