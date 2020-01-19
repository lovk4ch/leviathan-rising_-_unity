using UnityEngine;

public class HealthBar : MonoBehaviour
{
    protected enum Twinkle { Stay, Increase, Decrease };
    protected Twinkle twinkle = Twinkle.Stay;

    [SerializeField]
    protected Color fullColor = Color.green,
        middColor = Color.yellow,
        zeroColor = Color.red;

    [SerializeField]
    protected UnitController unitController;

    protected float offset = 4;
    protected float unitHealth = 1;
    protected float smoothSpeed = 3f;

    public float Offset { get => offset; set => offset = value; }

    public virtual void Initialize(UnitController unitController)
    {
        if (unitController is TankController)
        {
            fullColor = new Color(0f, 190/255f, 1f, 1f);
            middColor = new Color(0.5f, 73/255f, 1, 1);
            zeroColor = new Color(1f, 0f, 0f, 1f);
        }
        this.unitController = unitController;
    }
}