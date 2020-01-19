using System.Collections;
using UnityEngine;

public class LeviathanController : EnemyController
{
    private const string _grand_teleport_path_ = "teleport_big";

    private const string initialStateName = "isInitial";
    private const string actionStateName = "state";
    private const string hitStateName = "isHit";
    private const string roarState = "Roar";

    private AudioSource audioSource;
    private int attacksCount = 2;
    private Animator animator;

    private bool IsHit
    {
        get => animator.GetBool(hitStateName);
        set => animator.SetBool(hitStateName, value);
    }

    private bool IsInitial
    {
        get => animator.GetBool(initialStateName);
        set => animator.SetBool(initialStateName, value);
    }

    private int ActionState
    {
        get => animator.GetInteger(actionStateName);
        set => animator.SetInteger(actionStateName, value);
    }

    [SerializeField]
    private Transform stinger = null;

    [SerializeField]
    private GameObject shockwave = null;

    [SerializeField]
    private GameObject projectileOrb = null;

    protected override void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        base.Awake();

        if (MenuManager.IsGame)
            ActionState = 2;

        UnitManager.Instance.SetLeviathan(this);
    }

    /// <summary>
    /// Attack 1
    /// </summary>
    private void Aim()
    {
        if (target)
        {
            for (int i = 0; i < 5; i++)
            {
                GuidedProjectileMoveScript orb = Instantiate(projectileOrb, stinger.transform.position, Quaternion.LookRotation
                    (target.transform.position - stinger.transform.position)
                    * Quaternion.Euler(0, 10 * (i - 2), 0))
                    .GetComponent<GuidedProjectileMoveScript>();
                orb.Aim(target);
            }
        }
        ActionState = 0;
    }

    /// <summary>
    /// Attack 2
    /// </summary>
    private void Call()
    {
        if (IsInitial) {
            Scenario.Instance.CreateRaptors(Scenario.Instance.raptorsInitialCount);
            IsInitial = false;
        }
        else {
            Scenario.Instance.CreateRaptors(Random.Range(6, 11));
        }
        ActionState = 0;
    }

    /// <summary>
    /// Attack 3
    /// </summary>
    private void Shake()
    {
        StartCoroutine(SlowMotion(false));
        Gimbal.Instance.Shake();
        PlaySoundShake();
        Instantiate(shockwave, Consts.Horizontal(stinger.position), Quaternion.identity);

        if (target)
        {
            Vector3 direction = Consts.Horizontal(target.transform.position - stinger.position);
            float force = Mathf.Min(damage, damage * 50 / direction.sqrMagnitude);
            target.Hit(force, (direction + Vector3.up * (force / 100)).normalized * force);
            target.GetComponent<Rigidbody>().AddTorque(Vector3.forward * 50);
        }

        ActionState = 0;
    }

    private void PlaySound(string name)
    {
        if (audioSource)
        {
            audioSource.clip = AudioManager.Instance.Get(name);
            audioSource.Play();
        }
    }

    private void PlaySoundWreck()
    {
        PlaySound(AudioManager._leviathan_wreck_);
    }

    private void PlaySoundRoar()
    {
        PlaySound(AudioManager._leviathan_roar_);
    }

    private void PlaySoundAim()
    {
        PlaySound(AudioManager._leviathan_aim_);
    }

    private void PlaySoundCall()
    {
        if (Random.Range(-1, 1) == 0)
            PlaySound(AudioManager._leviathan_call_1_);
        else
            PlaySound(AudioManager._leviathan_call_2_);
    }

    private void PlaySoundShake()
    {
        if (Random.Range(-1, 1) == 0)
            PlaySound(AudioManager._leviathan_shake_1_);
        else
            PlaySound(AudioManager._leviathan_shake_2_);
    }

    private void Die()
    {
        StartCoroutine(SlowMotion(true));
    }

    public override void SetTarget(UnitController target)
    {
        if (target != null)
            base.SetTarget(target);
    }

    public void StandStill()
    {
        animator.Rebind();
        ActionState = -1;
    }

    public override void Destroy()
    {
        StartCoroutine(Annihilate());
    }

    public override void Hit(float damage)
    {
        base.Hit(damage);

        if (health < maxHealth * 0.66f && attacksCount == 2)
        {
            attacksCount++;

            UnitManager.Instance.SetLookAt(this);
            animator.Play(roarState, 0, 0);
            ActionState = 4;
            return;
        }

        if (health == 0 && ActionState != 5) {

            UnitManager.Instance.SetLookAt(this);
            animator.Play(roarState, 0, 0);
            ActionState = 5;
            return;
        }

        if (ActionState == 0) {
            IsHit = damage != 0;
            SelectAttack();
            return;
        }

        if (ActionState == -1) {
            ActionState = 2;
        }
    }

    private void SelectAttack()
    {
        ActionState = Random.Range(1, attacksCount + 1);
        if (attacksCount == 3 && ActionState != 2 && target)
        {
            Vector3 direction = Consts.Horizontal(target.transform.position - transform.position);
            if (direction.magnitude > 40)
                ActionState = 1;
            else
                ActionState = 3;
        }
    }

    private IEnumerator SlowMotion(bool isReverse)
    {
        float scale;

        if (isReverse) {
            scale = Time.timeScale / 2;
        }
        else {
            scale = Time.timeScale;
            Time.timeScale = scale / 2;
            yield return new WaitForSeconds(2);
        }

        float time = Time.time;

        float lerp = 0;
        while (Mathf.Abs(Time.timeScale - scale) > float.Epsilon)
        {
            yield return null;
            lerp += Time.deltaTime / 4;
            Time.timeScale = Mathf.Lerp(Time.timeScale, scale, lerp);
        }
    }

    private IEnumerator Annihilate()
    {
        Renderer[] renderer = GetComponentsInChildren<Renderer>();
        Color color = Color.white;
        float lerp = 0;
        yield return new WaitForSeconds(2);

        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particle in particles) {
            particle.Stop();
        }
        Scenario.Instance.GenerateGround(_grand_teleport_path_, transform.position);

        while (color != Color.black)
        {
            color = Color.Lerp(color, Color.black, lerp);

            foreach (Renderer mesh in renderer) {
                if (mesh.material.HasProperty("_Color"))
                    mesh.material.color = Color.Lerp(mesh.material.color, Color.black, lerp);
            }
            yield return null;
            lerp += Time.deltaTime / 400;
        }
    }
}