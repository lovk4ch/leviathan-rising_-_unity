using Cinemachine;
using UnityEngine;

public class CinemachineIntro : CinemachineData, IPlayerObserver
{
    [SerializeField]
    private CinemachineVirtualCamera tankCamera = null;

    private UnitController target;

    private void Awake()
    {
        UnitManager.Instance.Add(this);

        AudioManager.Instance.Stop();
        AudioManager.Instance.Play(AudioManager._shoreside_bay_);
    }

    protected override void OnDestroy()
    {
        if (!UnitManager.Instance)
            return;

        base.OnDestroy();
        UnitManager.Instance.Remove(this);
    }

    public void Initial()
    {
        Scenario.Instance.CreatePlayer();
        MenuManager.IsNewGame = false;
    }

    public void PlayMusic()
    {
        AudioManager.Instance.Play(AudioManager._main_theme_);
    }

    public void Shot()
    {
        target.GetComponent<TankAutomaticController>().Shot();
        UnitManager.Instance.SetTarget(null);
    }

    public void TurretUp()
    {
        target.GetComponent<TankAutomaticController>().TurretUp();
    }

    public void SetTarget(UnitController target)
    {
        if (!this.target)
        {
            tankCamera.LookAt = target.transform;
            tankCamera.Follow = target.transform;

            this.target = target;
            this.target.gameObject.AddComponent<TankAutomaticController>();
        }
    }

    public override void SetLookAt(GameObject target)
    {
        base.SetLookAt(target);

        if (target.GetComponent<UnitController>() is LeviathanController controller) {
            controller.StandStill();
        }
    }
}