using UnityEngine;

public class WindSystem : MonoBehaviour
{
    [SerializeField] private Vector3 direction;  // direction the wind blows TOWARD
    [SerializeField] private float speed;                   // m/s

    // Read-only for everyone else: wind velocity in world space, horizontal only
    public Vector3 Velocity
    {
        get
        {
            Vector3 d = direction;
            d.y = 0f;
            return d.normalized * speed;
        }
    }
}
