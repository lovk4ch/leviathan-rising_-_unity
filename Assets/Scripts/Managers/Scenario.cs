using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scenario : Manager<Scenario>
{
    private const int raptorsMaxCount = 30;
    public int raptorsInitialCount = 5;
    public int raptorsCurrentCount = 0;

    public GameObject teleport;
    public Canvas canvas;

    public bool isIntro;
    public bool isBigTank;
    public bool isSimsHealthBar;

    private void Awake()
    {
        MenuManager.Instance.OnChange += () => {
            canvas.gameObject.SetActive(MenuManager.IsGame);
        };
        canvas = Instantiate(canvas).GetComponent<Canvas>();
        canvas.gameObject.SetActive(false);

        Prefs.FirstPass = isIntro;
    }

    private IEnumerator m_TimeScale(float scale)
    {
        float initScale = Time.timeScale;
        float lerp = 0;

        while (Time.timeScale != scale)
        {
            lerp += Time.deltaTime;
            Time.timeScale = Mathf.Lerp(initScale, scale, lerp);
            yield return null;
            Debug.Log(Time.timeScale);
        }
    }

    /// <summary>
    /// Scale time to given value for 1 second
    /// </summary>
    /// <param name="scale">Scale coefficient</param>
    public void TimeScale(float scale)
    {
        StartCoroutine(m_TimeScale(scale));
    }

    private IEnumerator m_CreatePlayer()
    {
        Instantiate(teleport, Consts._player_start_position_ + Vector3.forward, Quaternion.identity);
        yield return new WaitForSeconds(Consts._teleport_spawn_time_);
        string path;

        if (isBigTank)
            path = Consts._big_tank_path_;
        else
            path = Consts._tank_path_;
        Generate(path, Consts._player_start_position_, Quaternion.identity);
    }

    public void CreatePlayer()
    {
        StartCoroutine(m_CreatePlayer());
    }

    public IEnumerator m_SpawnRaptors(int count)
    {
        for (int i = 0; i < count; i++)
        {
            StartCoroutine(m_CreateRaptor());
            yield return new WaitForSeconds(0.4f);

            raptorsCurrentCount++;
        }
    }

    private IEnumerator m_CreateRaptor()
    {
        Vector3 point = GenerateSpawnPoint();
        Instantiate(teleport, point, Quaternion.identity);

        yield return new WaitForSeconds(Consts._teleport_spawn_time_);

        Generate(Consts._raptor_enemy_path_, point, Quaternion.Euler(0, Random.Range(0, 360), 0));
    }

    public void CreateRaptors(int count)
    {
        count = Mathf.Min(count, raptorsMaxCount - raptorsCurrentCount);
        StartCoroutine(m_SpawnRaptors(count));
    }

    private IEnumerator m_Init()
    {
        Generate(Consts._leviathan_path_, Consts._leviathan_start_position_, Quaternion.AngleAxis(180, Vector3.up));
        yield return new WaitForSeconds(1f);

        CreatePlayer();

        if (!MenuManager.IsNewGame)
            yield break;
        MenuManager.IsNewGame = false;
    }

    public void Clear()
    {
        StopAllCoroutines();
        UnitManager.Instance.Clear();

        StartCoroutine(m_Init());
    }

    public void Annihilate(GameObject obj)
    {
        Instantiate(teleport, Consts.Horizontal(obj.transform.position + Vector3.forward), Quaternion.identity);
        Destroy(obj, Consts._teleport_spawn_time_);
    }

    public void GenerateCinemachine(UnitController target)
    {
        MenuManager.Instance._Mode = MenuManager.Mode.View;
        string path;

        switch (target.HealthPercentage)
        {
            case 1:
                path = Consts._cinemachine_initial_;
                break;
            case 0:
                path = Consts._cinemachine_death_;
                break;
            default:
                path = Consts._cinemachine_attack_;
                break;
        }

        CinemachineData data = Generate(path, Vector3.zero).GetComponent<CinemachineData>();
        data.SetLookAt(target.gameObject);
    }

    public void GenerateCinemachineShot(GameObject target)
    {
        MenuManager.Instance._Mode = MenuManager.Mode.View;

        CinemachineData data = Generate(Consts._cinemachine_shot_, Vector3.zero).GetComponent<CinemachineData>();
        data.SetLookAt(target);
    }

    public bool CheckSpawnPoint(Vector3 point)
    {
        List<IPlayerObserver> units = UnitManager.Instance.Observers.FindAll(unit => unit is UnitController);
        foreach (UnitController unit in units)
        {
            float radius = 7.5f;
            if (unit is LeviathanController) radius = 19.9f;

            if ((unit.transform.position - point).magnitude < radius)
                return false;
        }
        return true;
    }

    public Vector3 GenerateSpawnPoint()
    {
        Vector3 point = new Vector3(Random.Range(Consts._min_terrain_, Consts._max_terrain_), Consts._spawn_height_,
            Random.Range(Consts._min_terrain_, Consts._max_terrain_));

        if (CheckSpawnPoint(point))
            return point;
        else
            return GenerateSpawnPoint();
    }

    public GameObject GenerateGround(string name, Vector3 position)
    {
        return Generate(name, new Vector3(position.x, Consts._spawn_height_, position.z));
    }

    public GameObject Generate(string name, Vector3 position)
    {
        return Generate(name, position, Quaternion.identity);
    }

    public GameObject Generate(string name, Vector3 position, Quaternion rotation)
    {
        return Instantiate(Resources.Load(name), position, rotation) as GameObject;
    }

    public HealthBar GetHealthBar(UnitController unitController)
    {
        string path;

        if (isSimsHealthBar)
            path = Consts._health_crystal_path_;
        else
            path = Consts._health_bar_path_;

        return (Instantiate(Resources.Load(path), canvas.transform) as GameObject).GetComponent<HealthBar>();
    }
}