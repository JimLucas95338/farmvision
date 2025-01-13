using UnityEngine;
using TMPro;
using System.Collections;

[ExecuteInEditMode]
public class SensorLabel : MonoBehaviour
{
    [Header("Label Settings")]
    [SerializeField] private float heightOffset = 2f;
    [SerializeField] private float fontSize = 4f;
    [SerializeField] private Vector2 backgroundSize = new Vector2(4f, 1f);

    [Header("References")]
    private TextMeshPro textMesh;
    private Transform cameraTransform;
    private GameObject labelObject;
    private Material backgroundMaterial;
    private MeshRenderer backgroundRenderer;

    [Header("GPS Data")]
    [SerializeField] private float latitude;
    [SerializeField] private float longitude;
    private float currentTemp;
    private float currentHumidity;

    private string displayFormat = "T: {0}°C H: {1}%";

    private void OnEnable()
    {
        CleanupExistingLabels();
        InitializeLabel();
        cameraTransform = Camera.main.transform;
    }

    private void OnDisable()
    {
        if (labelObject != null)
        {
            DestroyImmediate(labelObject);
        }

        if (Application.isPlaying && backgroundMaterial != null)
        {
            Destroy(backgroundMaterial);
        }
    }

    private void CleanupExistingLabels()
    {
        TextMeshPro[] existingLabels = GetComponentsInChildren<TextMeshPro>();
        foreach (TextMeshPro label in existingLabels)
        {
            DestroyImmediate(label.gameObject);
        }
    }

    private void InitializeLabel()
    {
        if (labelObject == null)
        {
            // Create main label object
            labelObject = new GameObject("SensorLabel");
            labelObject.transform.SetParent(transform);

            // Create background first (so it's behind text)
            var background = new GameObject("LabelBackground");
            background.transform.SetParent(labelObject.transform);
            background.transform.localPosition = new Vector3(0, 0, 0.01f);
            background.transform.localRotation = Quaternion.identity;

            // Add quad mesh
            var meshFilter = background.AddComponent<MeshFilter>();
            meshFilter.mesh = CreateQuad(backgroundSize.x, backgroundSize.y);

            // Setup background material
            backgroundRenderer = background.AddComponent<MeshRenderer>();
            SetupBackgroundMaterial();

            // Setup TextMeshPro component
            textMesh = labelObject.AddComponent<TextMeshPro>();
            SetupTextMesh();

            labelObject.transform.localPosition = Vector3.up * heightOffset;
        }
    }

    private void SetupBackgroundMaterial()
    {
        if (Application.isPlaying)
        {
            backgroundMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            backgroundMaterial.SetFloat("_Surface", 1);
            backgroundMaterial.SetFloat("_Blend", 0);
            backgroundMaterial.SetFloat("_Metallic", 0);
            backgroundMaterial.SetFloat("_Smoothness", 0);
            backgroundRenderer.material = backgroundMaterial;
        }
        else
        {
            backgroundRenderer.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"))
            {
                color = Color.red
            };
        }
    }

    private void SetupTextMesh()
    {
        textMesh.alignment = TextAlignmentOptions.Midline;
        textMesh.fontSize = fontSize;
        textMesh.textWrappingMode = TextWrappingModes.NoWrap;
        textMesh.overflowMode = TextOverflowModes.Overflow;
        textMesh.margin = new Vector4(5, 2, 5, 2);
        textMesh.text = string.Format(displayFormat, 0, 0);
    }

    private Mesh CreateQuad(float width, float height)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-width / 2, -height / 2, 0),
            new Vector3(width / 2, -height / 2, 0),
            new Vector3(-width / 2, height / 2, 0),
            new Vector3(width / 2, height / 2, 0)
        };
        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            0, 2, 1,
            2, 3, 1
        };
        mesh.triangles = tris;

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;

        mesh.RecalculateNormals();
        return mesh;
    }

    private void Update()
    {
        if (labelObject != null && cameraTransform != null)
        {
            UpdateLabelPosition();
        }
    }

    private void UpdateLabelPosition()
    {
        if (labelObject != null)
        {
            labelObject.transform.position = transform.position + Vector3.up * heightOffset;

            if (cameraTransform != null)
            {
                Vector3 dirToCamera = cameraTransform.position - labelObject.transform.position;
                dirToCamera.y = 0;
                if (dirToCamera != Vector3.zero)
                {
                    labelObject.transform.rotation = Quaternion.LookRotation(-dirToCamera);
                }
            }
        }
    }

    public void UpdateSensorData(string newText, float newLatitude, float newLongitude, float temperature, float humidity, Color statusColor)
    {
        currentTemp = temperature;
        currentHumidity = humidity;
        latitude = newLatitude;
        longitude = newLongitude;

        if (textMesh != null)
        {
            string displayText = string.Format(displayFormat,
                temperature.ToString("F1"),
                humidity.ToString("F1"));
            textMesh.text = displayText;
            textMesh.color = statusColor;

            if (backgroundMaterial != null)
            {
                backgroundMaterial.color = new Color(statusColor.r, statusColor.g, statusColor.b, 0.8f);
            }
        }
    }

    public Vector2 GetGPSCoordinates()
    {
        return new Vector2(latitude, longitude);
    }
}