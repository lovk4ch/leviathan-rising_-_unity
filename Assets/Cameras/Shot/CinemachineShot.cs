using Cinemachine;
using UnityEngine;
using static MenuManager;

public class CinemachineShot : CinemachineData
{
    private KeyAction shot;

    private void SetInput()
    {
        shot = new KeyAction(KeyInputMode.KeyPressed, InputManager.SECONDARY_WEAPON_KEY, DisableCamera, Mode.View);
        InputManager.Instance.AddKeyAction(shot);
    }

    private void Awake()
    {
        SetInput();
    }

    protected override void OnDestroy()
    {
        if (!InputManager.Instance)
            return;

        base.OnDestroy();
        InputManager.Instance.RemoveKeyActions(this);
    }

    private void Update()
    {
        if (virtualCameras.Length > 0 && !virtualCameras[0].Follow)
            Destroy(gameObject);
    }

    public override void SetLookAt(GameObject target)
    {
        foreach (CinemachineVirtualCamera virtualCamera in virtualCameras)
        {
            virtualCamera.Follow = target.transform;
        }
    }
}