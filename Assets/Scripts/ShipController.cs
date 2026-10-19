using UnityEngine;
using UnityEngine.InputSystem;

// Require that this GameObject has a Rigidbody and PlayerInput component
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class ShipController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference move;

    [Header("Propulsion")]
    [SerializeField] private float thrust;
    [SerializeField] private float turnAccel;

    [Header("Steering")] 
    [SerializeField] private float refSpeed; // The speed at which the steering is fully effective

    [Header("Water drag")]
    [SerializeField] private float forwardDrag;   // along the hull 
    [SerializeField] private float lateralDrag;   // sideways
    
    [Header("Wind")]
    [SerializeField] private WindSystem wind;
    [SerializeField] private float sideWindCoef;
    [SerializeField] private float frontWindCoef;
    

    private Rigidbody _rb;
    private Vector2 _input;

    private void Awake() => _rb = GetComponent<Rigidbody>();

    private void OnEnable() => move.action.Enable();
    private void OnDisable() => move.action.Disable();

    private void Update()
    {
        _input = move.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector3 windVelocity = wind?.Velocity ?? Vector3.zero;
        Vector3 relWind = transform.InverseTransformDirection(windVelocity - _rb.linearVelocity);
        // Velocity in the ship's own axes: x = sideways, z = forward
        var v = transform.InverseTransformDirection(_rb.linearVelocity);

        // Drag grows with the square of speed and always opposes motion
        var dragLocal = new Vector3(
            -v.x * Mathf.Abs(v.x) * lateralDrag,
            0f,
            -v.z * Mathf.Abs(v.z) * forwardDrag);
        
        var windLocal = new Vector3(
            relWind.x * Mathf.Abs(relWind.x) * sideWindCoef,
            0f,
            relWind.z * Mathf.Abs(relWind.z) * frontWindCoef);

        _rb.AddForce(transform.TransformDirection(dragLocal), ForceMode.Acceleration);
        _rb.AddForce(transform.TransformDirection(windLocal), ForceMode.Acceleration);
        _rb.AddForce(transform.forward * (_input.y * thrust), ForceMode.Acceleration);
        
        float rudderAuthority = Mathf.Clamp(v.z / refSpeed, -1f, 1f);
        _rb.AddTorque(Vector3.up * (_input.x * turnAccel * rudderAuthority), ForceMode.Acceleration);
    }
}