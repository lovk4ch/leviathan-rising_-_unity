using UnityEngine;

public abstract class DualGun : Gun
{
    protected enum GunType { LEFT, RIGHT }
    protected GunType currentGun;

    public GameObject leftGun;
    public GameObject rightGun;

    // private new Rigidbody rigidbody;

    public void SetSpeed(Rigidbody rigidbody)
    {
        // this.rigidbody = rigidbody;
    }

    public virtual void DualShot()
    {
        if (!projectile || !leftGun || !rightGun)
            return;

        if (currentTime >= reloadTime)
        {
            if (currentGun == GunType.LEFT)
            {
                Fire(projectile, leftGun, ref currentTime);
                currentGun = GunType.RIGHT;
            }
            else
            {
                Fire(projectile, rightGun, ref currentTime);
                currentGun = GunType.LEFT;
            }
        }
    }
}