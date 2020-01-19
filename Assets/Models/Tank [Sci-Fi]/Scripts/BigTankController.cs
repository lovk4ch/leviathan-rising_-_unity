using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BigTankController : TankController
{
    private void RotateTurret(Vector3 direction)
    {
        turret.transform.Rotate(direction * Time.deltaTime * turretInclineSpeed);
        TurretSoundPlay();
    }

    public override void TurretUp()
    {
        if (turret.transform.localRotation.z < 0.073f)
        {
            RotateTurret(Vector3.forward);
            return;
        }
        TurretSoundStop();
    }

    public override void TurretDown()
    {
        if (turret.transform.localRotation.z > 0)
        {
            RotateTurret(Vector3.back);
            return;
        }
        TurretSoundStop();
    }

    public override void TurretLeft()
    {
        RotateTurret(Vector3.down);
    }

    public override void TurretRight()
    {
        RotateTurret(Vector3.up);
    }
}