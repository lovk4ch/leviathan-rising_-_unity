using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SimpleHealthBar : HealthBar
{
    private float alphaZero = 0, alphaCurrent = 0, alphaFull = 1, twinkleSpeed = 3;

    protected Image barImage;
    protected Image brighten;

    public RectTransform healthBar;
    public RectTransform healthBrighten;

    protected virtual void Awake()
    {
        brighten = healthBrighten.GetComponent<Image>();
        barImage = healthBar.GetComponent<Image>();
        barImage.color = fullColor;
        brighten.gameObject.SetActive(true);
        brighten.color = new Color(brighten.color.r, brighten.color.g, brighten.color.b, alphaCurrent);

        twinkle = Twinkle.Stay;
        StartCoroutine(Expand());
    }

    public override void Initialize(UnitController unitController)
    {
        base.Initialize(unitController);

        if (unitController is TankController) {
            barImage.color = fullColor;
            Offset = 0;
        }
    }

    protected IEnumerator Expand()
    {
        transform.localScale = new Vector3(0, 1, 1);
        yield return new WaitForEndOfFrame();

        float timer = 0;

        while (transform.localScale.x < 1)
        {
            timer += Time.deltaTime / 5;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, timer);
            yield return new WaitForEndOfFrame();
        }
    }

    protected IEnumerator Narrow()
    {
        Vector3 endScale = new Vector3(0, 1, 1);
        yield return new WaitForEndOfFrame();

        float timer = 0;

        while (transform.localScale.x > 0)
        {
            timer += Time.deltaTime / 15;
            transform.localScale = Vector3.Lerp(transform.localScale, endScale, timer);
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnDisable()
    {
        if (unitController.HealthPercentage == 0)
            transform.localScale = Vector3.zero;
        else
            transform.localScale = Vector3.one;
    }

    private void Update()
    {
        if (Math.Round(unitHealth - unitController.HealthPercentage, 2) != 0)
        {
            unitHealth = Mathf.Lerp(unitHealth, unitController.HealthPercentage,
                smoothSpeed * Time.deltaTime);

            if (unitController.HealthPercentage > 0)
            {
                healthBar.localScale = new Vector3(unitHealth, 1, 1);
                healthBrighten.localScale = healthBar.localScale;
                if (twinkle != Twinkle.Increase) {
                    twinkle = Twinkle.Increase;
                }
            }
            else
            {
                StartCoroutine(Narrow());
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
                else
                {
                    twinkle = Twinkle.Stay;
                    alphaCurrent = alphaZero;
                }
            }
            if (unitHealth > 0.5f)
                barImage.color = Color.Lerp(middColor, fullColor, (unitHealth - 0.5f) * 2);
            else
                barImage.color = Color.Lerp(zeroColor, middColor, unitHealth * 2);

            brighten.color = new Color(brighten.color.r, brighten.color.g, brighten.color.b, alphaCurrent);
        }
    }

    private void LateUpdate() {
        float delta = Time.deltaTime * 50;

        transform.position = Vector3.Lerp(transform.position, Camera.main.WorldToScreenPoint(unitController.transform.position
            + Vector3.up * Offset), delta);
    }
}