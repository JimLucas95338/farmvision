using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class SensorLabel : MonoBehaviour
{
    [Header("Label Settings")]
    [SerializeField] private string labelText = "Sensor";
    [SerializeField] private float heightOffset = 2f;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private float fontSize = 4f;

    [Header("References")]
    private TextMeshPro textMesh;
    private Transform cameraTransform;
    private GameObject labelObject;

    [Header("GPS Data")]
    [SerializeField] private float latitude;
    [SerializeField] private float longitude;

    private void OnEnable()
    {
        // Clean up any existing labels first
        CleanupExistingLabels();
        InitializeLabel();
        cameraTransform = Camera.main.transform;
    }

    private void OnDisable()
    {
        // Clean up when the component is disabled
        if (labelObject != null)
        {
            DestroyImmediate(labelObject);
        }
    }

    private void CleanupExistingLabels()
    {
        // Find and destroy any existing labels
        TextMeshPro[] existingLabels = GetComponentsInChildren<TextMeshPro>();
        foreach (TextMeshPro label in existingLabels)
        {
            DestroyImmediate(label.gameObject);
        }
    }

    private void InitializeLabel()
    {
        // Create label if it doesn't exist
        if (labelObject == null)
        {
            labelObject = new GameObject("SensorLabel");
            labelObject.transform.SetParent(transform);

            // Add TextMeshPro component
            textMesh = labelObject.AddComponent<TextMeshPro>();
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.text = labelText;
            textMesh.color = textColor;
            textMesh.fontSize = fontSize;

            // Position the label above the object
            labelObject.transform.localPosition = Vector3.up * heightOffset;
        }
    }

    private void Update()
    {
        if (labelObject != null && cameraTransform != null)
        {
            // Make the label face the camera
            labelObject.transform.LookAt(cameraTransform);
            labelObject.transform.Rotate(0, 180, 0);

            // Update position
            labelObject.transform.position = transform.position + Vector3.up * heightOffset;
        }
    }

    // Method to update sensor data
    public void UpdateSensorData(string newText, float newLatitude, float newLongitude)
    {
        labelText = newText;
        latitude = newLatitude;
        longitude = newLongitude;

        if (textMesh != null)
        {
            textMesh.text = labelText;
        }
    }

    // Method to get current GPS coordinates
    public Vector2 GetGPSCoordinates()
    {
        return new Vector2(latitude, longitude);
    }
}