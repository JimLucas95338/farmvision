using UnityEngine;
using System.Collections;
using TMPro;

public class SensorDataSimulator : MonoBehaviour
{
    [Header("Simulation Settings")]
    [SerializeField] private float updateInterval = 2f;
    [SerializeField] private float minTemperature = 15f;
    [SerializeField] private float maxTemperature = 35f;
    [SerializeField] private float minHumidity = 30f;
    [SerializeField] private float maxHumidity = 80f;
    [SerializeField] private float noiseAmount = 0.5f;

    [Header("Range Settings")]
    [SerializeField] private float idealTempMin = 20f;
    [SerializeField] private float idealTempMax = 25f;
    [SerializeField] private float idealHumidityMin = 40f;
    [SerializeField] private float idealHumidityMax = 60f;

    private static readonly Color goodColor = Color.green;
    private static readonly Color badColor = Color.red;

    private SensorLabel sensorLabel;
    private MeshRenderer objectRenderer;
    private Material objectMaterial;
    private float currentTemperature;
    private float currentHumidity;

    private void Start()
    {
        sensorLabel = GetComponent<SensorLabel>();
        objectRenderer = GetComponent<MeshRenderer>();

        if (Application.isPlaying && objectRenderer != null)
        {
            objectMaterial = new Material(objectRenderer.material);
            objectRenderer.material = objectMaterial;
        }

        currentTemperature = Random.Range(minTemperature, maxTemperature);
        currentHumidity = Random.Range(minHumidity, maxHumidity);

        StartCoroutine(SimulateSensorData());
    }

    private IEnumerator SimulateSensorData()
    {
        while (true)
        {
            // Update values
            currentTemperature = Mathf.Clamp(
                currentTemperature + Random.Range(-noiseAmount, noiseAmount),
                minTemperature,
                maxTemperature
            );

            currentHumidity = Mathf.Clamp(
                currentHumidity + Random.Range(-noiseAmount, noiseAmount),
                minHumidity,
                maxHumidity
            );

            // Check if values are in ideal ranges
            bool isInRange = IsInIdealRange(currentTemperature, currentHumidity);
            Color currentColor = isInRange ? goodColor : badColor;

            // Update object color
            if (objectMaterial != null)
            {
                objectMaterial.color = currentColor;
            }

            // Update label
            if (sensorLabel != null)
            {
                sensorLabel.UpdateSensorData("", 0, 0, currentTemperature, currentHumidity, currentColor);
            }

            yield return new WaitForSeconds(updateInterval);
        }
    }

    private bool IsInIdealRange(float temp, float humidity)
    {
        bool tempInRange = temp >= idealTempMin && temp <= idealTempMax;
        bool humidityInRange = humidity >= idealHumidityMin && humidity <= idealHumidityMax;
        return tempInRange && humidityInRange;
    }

    private void OnDestroy()
    {
        if (Application.isPlaying && objectMaterial != null)
        {
            Destroy(objectMaterial);
        }
    }
}