using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class SailHud : MonoBehaviour
{
    [SerializeField] private SailsController sailsController;

    private Label _angleLabel;
    private Label _openLabel;

    private void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        _angleLabel = root.Q<Label>("SailAngleLabel");
        _openLabel = root.Q<Label>("SailOpenLabel");
    }

    private void Update()
    {
        if (sailsController is null || _angleLabel is null || _openLabel is null) return;

        _angleLabel.text = $"Sail angle: {sailsController.SailAngle:0}°";
        _openLabel.text = $"Sail open: {sailsController.OpenPercent:0}%";
    }
}
