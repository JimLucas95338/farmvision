using UnityEngine;
using System.Collections;
using TMPro;

public class SensorDataSimulator : MonoBehaviour  // Added MonoBehaviour inheritance here
{
    [Header("Simulation Settings")]
    [SerializeField] private float updateInterval = 2f;
    [SerializeField] private float minTemperature = 15f;
    [SerializeField] private float maxTemperature = 35f;
    [SerializeField] private float minHumidity = 30f;
    [SerializeField] private float maxHumidity = 80f;

    [Header("Display Format")]
    [SerializeField] private string displayFormat = "Temp: {0:F1}°C\nHumidity: {1:F1}%";

    private SensorLabel sensorLabel;
    private float currentTemperature;
    private float currentHumidity;
    private float noiseAmount = 0.5f;

    private void Start()
    {
        sensorLabel = GetComponent<SensorLabel>();
        if (sensorLabel == null)
        {
            Debug.LogError("SensorLabel component not found!");
            return;
        }

        // Initialize with random values
        currentTemperature = Random.Range(minTemperature, maxTemperature);
        currentHumidity = Random.Range(minHumidity, maxHumidity);

        // Start the update coroutine
        StartCoroutine(SimulateSensorData());
    }

    private IEnumerator SimulateSensorData()
    {
        while (true)
        {
            // Add some random variation to current values
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

            // Update the label
            string newText = string.Format(displayFormat, currentTemperature, currentHumidity);
            sensorLabel.UpdateSensorData(newText, 0, 0); // GPS coordinates set to 0 for now

            // Color-code based on temperature
            if (currentTemperature > 30f)
            {
                GetComponent<Renderer>().material.color = Color.red;
            }
            else if (currentTemperature < 20f)
            {
                GetComponent<Renderer>().material.color = Color.blue;
            }
            else
            {
                GetComponent<Renderer>().material.color = Color.green;
            }

            yield return new WaitForSeconds(updateInterval);
        }
    }

    // Public method to get current sensor readings
    public (float temperature, float humidity) GetCurrentReadings()
    {
        return (currentTemperature, currentHumidity);
    }
}