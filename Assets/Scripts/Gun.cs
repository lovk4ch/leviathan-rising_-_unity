using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    [SerializeField]
    protected float reloadTime = .5f;
    protected float currentTime;

    public GameObject projectile;

    protected virtual void Awake()
    {
        currentTime = reloadTime;
        SetInput();
    }

    protected virtual void Update()
    {
        currentTime += Time.deltaTime;
    }

    public abstract void Shot();

    protected abstract void SetInput();

    protected GameObject Fire(GameObject projectile, GameObject barrel, ref float reloadTime)
    {
        GameObject prj = Instantiate(projectile, barrel.transform.position, barrel.transform.rotation);
        if (prj.GetComponent<ProjectileMoveScript>() is ProjectileMoveScript pms)
        {
            if (pms is GuidedProjectileMoveScript guidedPms)
                Aim(guidedPms);

            pms.SetMuzzle(barrel.transform);
        }
        reloadTime = 0;
        return prj;
    }

    protected void Aim(GuidedProjectileMoveScript projectile)
    {
        List<IPlayerObserver> enemies = UnitManager.Instance.Observers.FindAll(enemy => enemy is EnemyController);
        Vector3 position = transform.position;
        float distance = Mathf.Infinity;
        List<EnemyController> targets = new List<EnemyController>();

        foreach (EnemyController enemy in enemies)
        {
            Vector3 diff = enemy.transform.position - position;
            float curDistance = diff.magnitude;
            if (curDistance < distance)
            {
                distance = curDistance;
            }
        }
        foreach (EnemyController enemy in enemies)
        {
            Vector3 diff = enemy.transform.position - position;
            float curDistance = diff.magnitude;
            if (curDistance < distance + 10)
            {
                if (enemy.HealthPercentage > 0) {
                    targets.Add(enemy);
                }
            }
        }
        if (targets.Count > 0)
            projectile.Aim(targets[Random.Range(0, targets.Count)]);
    }
}