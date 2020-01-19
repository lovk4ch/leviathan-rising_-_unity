using UnityEngine;
using static MenuManager;

public class CannonGun : DualGun
{
    private float currentCannonTime;
    public float cannonReloadTime = 4f;

    private KeyAction cannonShot;
    private KeyAction shot;

    public GameObject cannon;
    public GameObject cannonProjectile;

    protected override void SetInput()
    {
        cannonShot = new KeyAction(KeyInputMode.KeyPressed, InputManager.PRIMARY_WEAPON_KEY, Shot, Mode.Game);
        InputManager.Instance.AddKeyAction(cannonShot);

        shot = new KeyAction(KeyInputMode.KeyPressed, InputManager.SECONDARY_WEAPON_KEY, DualShot, Mode.Game);
        InputManager.Instance.AddKeyAction(shot);
    }

    protected override void Awake()
    {
        base.Awake();
        currentCannonTime = cannonReloadTime;
    }

    protected override void Update()
    {
        base.Update();
        currentCannonTime += Time.deltaTime;
    }

    private void OnEnable()
    {
        InputManager.Instance.AddKeyAction(shot);
        InputManager.Instance.AddKeyAction(cannonShot);
    }

    private void OnDisable()
    {
        if (!InputManager.Instance)
            return;

        InputManager.Instance.RemoveKeyActions(this);
    }

    private void OnDestroy()
    {
        if (!InputManager.Instance)
            return;

        InputManager.Instance.RemoveKeyActions(this);
    }

    public override void Shot()
    {
        if (!cannonProjectile || !cannon)
            return;

        if (currentCannonTime >= cannonReloadTime)
        {
            GameObject prj = Fire(cannonProjectile, cannon, ref currentCannonTime);
            if (IsGame) {
                Scenario.Instance.GenerateCinemachineShot(prj);
            }
        }
    }
}