using System.Collections;
using UnityEngine;

public class TankAutomaticController : MonoBehaviour
{
    private TankController tankController;
    private Gun gun;

    private void Start()
    {
        tankController = GetComponent<TankController>();
        gun = tankController.Gun;
        tankController.enabled = false;
        MenuManager.Instance.OnChange += OnChange;

        StartCoroutine(Intro());
    }

    private void OnChange()
    {
        if (MenuManager.IsGame) {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if (!MenuManager.Instance)
            return;

        tankController.enabled = true;
        MenuManager.Instance.OnChange -= OnChange;
    }

    private IEnumerator m_TurretUp()
    {
        float delta = Time.deltaTime;
        float remainingTime = 3;

        while (remainingTime > 0)
        {
            tankController.TurretUp();
            yield return null;
            remainingTime -= delta;
        }
    }

    private IEnumerator Intro()
    {
        float delta = Time.deltaTime;
        float remainingTime = 5;

        while (remainingTime > 0)
        {
            transform.Translate(transform.forward * delta * remainingTime);
            yield return null;
            remainingTime -= delta;
        }
    }

    public void Shot()
    {
        gun.Shot();
    }

    public void TurretUp()
    {
        StartCoroutine(m_TurretUp());
    }
}