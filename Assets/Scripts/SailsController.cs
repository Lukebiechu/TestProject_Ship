using UnityEngine;
using UnityEngine.InputSystem;

public class SailsController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference sailTurn;
    [SerializeField] private InputActionReference sailOpenAction;

    [Header("Trim (rotation)")]
    [SerializeField] private Transform sailPivot;
    [SerializeField] private float maxAngle;
    [SerializeField] private float turnSpeed;

    [Header("Opening (Z scale)")]
    [SerializeField] private Transform[] sails;              // the 3 sail objects
    [SerializeField, Range(0f, 100f)] private float openPercent;
    [SerializeField] private float openSpeed;          // percent per second
    [SerializeField] private float minScaleZ;        // visual scale at 0% (avoids an exact 0)

    public float SailAngle { get; private set; }
    public float OpenPercent => openPercent;                 // 0 = closed, 100 = fully open

    private Quaternion _baseRotation;
    private Vector3[] _baseScales;

    private void Awake()
    {
        _baseRotation = sailPivot.localRotation;

        _baseScales = new Vector3[sails.Length];
        for (int i = 0; i < sails.Length; i++)
            _baseScales[i] = sails[i].localScale;

        ApplyOpening();
    }

    private void OnEnable()
    {
        sailTurn.action.Enable();
        sailOpenAction.action.Enable();
    }

    private void OnDisable()
    {
        sailTurn.action.Disable();
        sailOpenAction.action.Disable();
    }

    private void Update()
    {
        // Rotation
        var turnInput = sailTurn.action.ReadValue<float>();

        if (Mathf.Abs(turnInput) > 0.01f)
            SailAngle += turnInput * turnSpeed * Time.deltaTime;

        SailAngle = Mathf.Clamp(SailAngle, -maxAngle, maxAngle);
        sailPivot.localRotation = _baseRotation * Quaternion.Euler(0f, SailAngle, 0f);

        // Opening
        var openInput = sailOpenAction.action.ReadValue<float>();
        openPercent = Mathf.Clamp(openPercent + openInput * openSpeed * Time.deltaTime, 0f, 100f);
        ApplyOpening();
    }

    private void ApplyOpening()
    {
        float factor = Mathf.Lerp(minScaleZ, 1f, openPercent / 100f);

        for (int i = 0; i < sails.Length; i++)
        {
            Vector3 s = _baseScales[i];
            s.z *= factor;
            sails[i].localScale = s;
        }
    }
}