using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using static ShakeTransform;

public class Gimbal : Manager<Gimbal>, IPlayerObserver
{
    private static Vector3 smoothScale;
    private float mouseX, mouseY;
    private float aperture;
    private UnitController target;

    [SerializeField]
    private ShakeTransform shakeTransform = null;
    [SerializeField]
    private float m_rotationSpeed = 100f;
    [SerializeField]
    private float m_speed = 3f;
    [SerializeField]
    private Transform cameraHolder = null;
    [SerializeField]
    private DepthOfField depthScript = null;

    private void Awake()
    {
        smoothScale = cameraHolder.localPosition;
        aperture = depthScript.aperture;

        Reset();
        UnitManager.Instance.Add(this);
    }

    public void GetCinemachine(UnitController target)
    {
        depthScript.aperture = 0;
        Scenario.Instance.GenerateCinemachine(target);
    }

    public void Shake()
    {
        shakeTransform.AddShakeEvent(ShakeType.Game);
    }

    public void SetTarget(UnitController target)
    {
        if (target && this.target != target) {
            StartCoroutine(Move(target));
        }
        shakeTransform.ChangeMode(ShakeType.Game);
    }

    public void Reset()
    {
        depthScript.transform.localPosition = Vector3.zero;
        depthScript.transform.localRotation = Quaternion.identity;
        depthScript.aperture = aperture;
    }

    // spike
    private IEnumerator Move(UnitController target)
    {
        smoothScale = new Vector3(smoothScale.x, smoothScale.y, -1);
        mouseX = 0;
        mouseY = 0;

        float lerp = 0;
        while ((transform.position - target.transform.position).magnitude > 1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, target.transform.rotation * Quaternion.Euler(mouseY, mouseX, 0), lerp);
            transform.position = Vector3.Lerp(transform.position, target.transform.position, lerp);
            cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, smoothScale, lerp);
            lerp += Time.deltaTime / 2;
            yield return new WaitForEndOfFrame();
        }
        this.target = target;
    }

    private void LateUpdate()
    {
        float delta = Time.deltaTime;

        if (target)
        {
            if (shakeTransform.shakeType == ShakeType.Game)
            {
                mouseX += InputManager.Instance.GetAxis(InputManager.MOUSE_X) * m_rotationSpeed * delta;
                mouseY -= InputManager.Instance.GetAxis(InputManager.MOUSE_Y) * m_rotationSpeed * delta;

                transform.position = target.transform.position;
                /*if (mouseY < -30 - smoothScale.z * 3)
                    mouseY = -30 - smoothScale.z * 3;
                else
                if (mouseY > 60)
                    mouseY = 60;*/
                depthScript.focalLength = (depthScript.transform.position - target.transform.position).magnitude;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(mouseY, mouseX, 0),
                    m_speed * delta);

                float change = InputManager.Instance.GetWheel() / 2;
                if (change != 0 && smoothScale.z + change <= 0 && smoothScale.z + change >= -10)
                {
                    smoothScale += new Vector3(0, 0, change);
                }
                cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, smoothScale, m_speed / 5 * delta);
            }
        }
    }
}