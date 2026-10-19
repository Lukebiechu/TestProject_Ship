using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 targetOffset ;

    [Header("Input")]
    [SerializeField] private InputActionReference look;
    [SerializeField] private float sensitivity;

    [Header("Orbit")]
    [SerializeField] private float distance;
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    [SerializeField] private bool followShipHeading;

    private float _yaw;
    private float _pitch = 20f;

    private void OnEnable() => look.action.Enable();
    private void OnDisable() => look.action.Disable();

    private void LateUpdate()
    {
        if (target is null) return;
        
        Vector2 delta = look.action.ReadValue<Vector2>();
        
        _yaw += delta.x * sensitivity;
        _pitch = Mathf.Clamp(_pitch - delta.y * sensitivity, minPitch, maxPitch);
        // Offset from the ship's heading, so the camera turns with the ship
        float baseYaw = followShipHeading ? target.eulerAngles.y : 0f;
        Quaternion rotation = Quaternion.Euler(_pitch, baseYaw + _yaw, 0f);
        Vector3 focus = target.position + targetOffset;

        transform.position = focus - rotation * Vector3.forward * distance;
        transform.rotation = rotation;
    }
}
