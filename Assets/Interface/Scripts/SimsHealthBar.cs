using System;
using UnityEngine;

public class SimsHealthBar : HealthBar
{
    private float alphaZero = 0, alphaCurrent = 0, alphaFull = 1, twinkleSpeed = 3;

    [SerializeField]
    private new Renderer renderer = null;
    [SerializeField]
    private ParticleSystem sparks = null;
    [SerializeField]
    private Color deadColor = Color.black;

    private ParticleSystem.MainModule mainModule;

    private void Awake()
    {
        mainModule = sparks.main;
    }

    private void Start()
    {
        SetColor(fullColor);
    }

    public override void Initialize(UnitController unitController)
    {
        base.Initialize(unitController);
        transform.position = unitController.transform.position + Vector3.up * Offset;

        if (unitController is LeviathanController) {
            Offset = 20;
            transform.localScale *= 3;
            sparks.transform.localScale *= 3;
        }
    }

    private void Update()
    {
        if (Math.Round(unitHealth - unitController.HealthPercentage, 2) != 0)
        {
            unitHealth = Mathf.Lerp(unitHealth, unitController.HealthPercentage,
                smoothSpeed * Time.deltaTime);

            if (twinkle != Twinkle.Increase) {
                twinkle = Twinkle.Increase;
            }
        }

        if (twinkle != Twinkle.Stay)
        {
            if (twinkle == Twinkle.Increase)
            {
                if (alphaCurrent < alphaFull) {
                    alphaCurrent += Time.deltaTime * twinkleSpeed;
                }
                else {
                    twinkle = Twinkle.Decrease;
                }
            }
            else
            {
                if (alphaCurrent > alphaZero) {
                    alphaCurrent -= Time.deltaTime * twinkleSpeed;
                }
                else {
                    twinkle = Twinkle.Stay;
                    alphaCurrent = alphaZero;
                }
            }
            if (unitHealth > 0.5f)
                SetColor(Color.Lerp(middColor, fullColor, (unitHealth - 0.5f) * 2));
            else if (unitHealth > 0.1f)
                SetColor(Color.Lerp(zeroColor, middColor, (unitHealth - 0.1f) * 2.5f));
            else
                SetColor(Color.Lerp(deadColor, zeroColor, unitHealth * 10));
        }
    }

    private void SetColor(Color color)
    {
        renderer.material.SetColor("_EmissionColor", color);
        mainModule.startColor = color;
    }

    private void LateUpdate() {
        float delta = Time.deltaTime * 50;

        transform.position = Vector3.Lerp(transform.position, unitController.transform.position + Vector3.up * Offset, delta);
        transform.Rotate(Vector3.up * delta);
    }
}