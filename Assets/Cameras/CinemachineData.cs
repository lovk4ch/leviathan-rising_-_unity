using Cinemachine;
using UnityEngine;

public class CinemachineData : MonoBehaviour
{
    [SerializeField]
    protected CinemachineVirtualCamera[] virtualCameras = null;

    public void DisableCamera()
    {
        Destroy(gameObject);
    }

    public void SmallPartTime()
    {
        Time.timeScale = 0.05f;
    }

    public void TenthPartTime()
    {
        Time.timeScale = 0.1f;
    }

    public void HalfPartTime()
    {
        Scenario.Instance.TimeScale(0.5f);
    }

    protected virtual void OnDestroy()
    {
        if (!MenuManager.Instance)
            return;

        Time.timeScale = 1;
        Gimbal.Instance?.Reset();
        MenuManager.Instance._Mode = MenuManager.Mode.Game;
    }

    public virtual void SetLookAt(GameObject target)
    {
        foreach (CinemachineVirtualCamera virtualCamera in virtualCameras)
        {
            virtualCamera.LookAt = target.transform;
            virtualCamera.Follow = target.transform;
        }
    }
}