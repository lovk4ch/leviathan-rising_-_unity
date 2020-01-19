using System.Collections.Generic;
using UnityEngine;

public class UnitManager : Manager<UnitManager>
{
    public List<IPlayerObserver> Observers { get; private set; }
    private UnitController leviathan;
    private TankController tank;

    private void Awake()
    {
        Observers = new List<IPlayerObserver>();
    }

    public void Add(IPlayerObserver observer)
    {
        Observers.Add(observer);
        if (tank) {
            observer.SetTarget(tank);
        }
    }

    public void Remove(IPlayerObserver observer)
    {
        Observers.Remove(observer);
    }

    public void SetLookAt(UnitController target)
    {
        if (target.HealthPercentage != 0) {
            tank.SetToAttack(target.transform);
        }
        Gimbal.Instance.GetCinemachine(target);
    }

    public void Notify()
    {
        for (int i = 0; i < Observers.Count; i++)
        {
            Observers[i].SetTarget(tank);
        }
    }

    public void Clear()
    {
        for (int i = 0; i < Observers.Count; i++)
        {
            if (Observers[i] is UnitController enemy)
            {
                Destroy(enemy.gameObject);
                Remove(Observers[i--]);
            }
        }
    }

    public void Attack()
    {
        if (Random.Range(-1, 1) == 0)
            leviathan.Hit(0);
    }

    public void SetTarget(TankController tank)
    {
        this.tank = tank;
        Notify();
    }

    public void SetLeviathan(UnitController leviathan)
    {
        this.leviathan = leviathan;
    }
}