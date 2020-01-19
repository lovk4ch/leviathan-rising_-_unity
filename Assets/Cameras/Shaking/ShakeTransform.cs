using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShakeEvent
{
    float duration;
    float timeRemaining;

    ShakeTransformEvent data;
    public ShakeTransformEvent.Target Target => data.target;

    Vector3 noiseOffset;
    public Vector3 noise;

    public ShakeEvent(ShakeTransformEvent data)
    {
        this.data = data;
        duration = data.duration;
        timeRemaining = data.duration;

        float random = 30f;

        noiseOffset = new Vector3(Random.Range(0, random), Random.Range(0, random), Random.Range(0, random));
    }

    public void Update()
    {
        float delta = Time.deltaTime;
        timeRemaining -= delta;
        float noiseOffsetDelta = delta * data.frequency;

        noiseOffset += new Vector3(noiseOffsetDelta, noiseOffsetDelta, noiseOffsetDelta);

        noise = new Vector3(Mathf.PerlinNoise(noiseOffset.x, 0.0f), Mathf.PerlinNoise(noiseOffset.x, 1.0f), Mathf.PerlinNoise(noiseOffset.x, 2.0f));
        noise -= Vector3.one * 0.5f;
        noise *= data.amplitude;

        float agePercent = 1.0f - (timeRemaining / duration);
        noise *= data.blendOverLifetime.Evaluate(agePercent);
    }

    public bool IsAlive => timeRemaining > 0;
}

public class ShakeTransform : MonoBehaviour
{
    [SerializeField]
    private List<ShakeEvent> shakeEvents = new List<ShakeEvent>();
    private Vector3 localPosition;
    private Vector3 localRotation;

    public enum ShakeType { View, Game }
    public ShakeType shakeType;

    public ShakeTransformEvent[] data;

    private void Awake()
    {
        localPosition = transform.localPosition;
        localRotation = transform.localEulerAngles;

        AddShakeEvent(shakeType);
    }

    private IEnumerator AddView()
    {
        while (shakeEvents.Count < 3)
        {
            if (shakeType == ShakeType.View)
            {
                shakeEvents.Add(new ShakeEvent(data[0]));
                yield return new WaitForSeconds(data[0].duration / 3f);
            }
            else yield break;
        }
    }

    public void AddShakeEvent(ShakeType shakeType)
    {
        switch (shakeType)
        {
            case ShakeType.View:
                StartCoroutine(AddView());
                return;
            default:
                shakeEvents.Add(new ShakeEvent(data[1]));
                return;
        }
    }

    public void ChangeMode(ShakeType type)
    {
        shakeType = type;
        shakeEvents.Clear();

        if (shakeType == ShakeType.View)
            AddShakeEvent(type);
    }

    public void AddShakeEvent(float amplitude, float frequency, float duration, AnimationCurve blendOverLifetime, ShakeTransformEvent.Target target)
    {
        ShakeTransformEvent data = ScriptableObject.CreateInstance<ShakeTransformEvent>();
        data.Init(amplitude, frequency, duration, blendOverLifetime, target);
        shakeEvents.Add(new ShakeEvent(data));
    }

    private void LateUpdate()
    {
        Vector3 positionOffset = Vector3.zero, rotationOffset = Vector3.zero;

        for (int i = shakeEvents.Count - 1; i > -1; i--)
        {
            ShakeEvent shakeEvent = shakeEvents[i];
            shakeEvent.Update();

            if (shakeEvent.Target == ShakeTransformEvent.Target.Position)
                positionOffset += shakeEvent.noise;
            else
                rotationOffset += shakeEvent.noise;

            if (!shakeEvent.IsAlive) {
                shakeEvents.RemoveAt(i);

                if (shakeType == ShakeType.View) {
                    AddShakeEvent(shakeType);
                }
            }
        }

        if (shakeType == ShakeType.View)
            transform.localPosition = localPosition + positionOffset;

        transform.localEulerAngles = rotationOffset;
    }
}