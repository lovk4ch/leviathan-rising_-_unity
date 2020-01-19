using static MenuManager;

public class SmallDualGun : DualGun
{
    private KeyAction shot;

    protected override void SetInput()
    {
        shot = new KeyAction(KeyInputMode.KeyPressed, InputManager.PRIMARY_WEAPON_KEY, DualShot, Mode.Game);
        InputManager.Instance.AddKeyAction(shot);
    }

    public override void Shot()
    {
        DualShot();
    }

    private void OnEnable()
    {
        InputManager.Instance.AddKeyAction(shot);
    }

    private void OnDestroy()
    {
        if (!InputManager.Instance)
            return;

        InputManager.Instance.RemoveKeyActions(this);
    }
}