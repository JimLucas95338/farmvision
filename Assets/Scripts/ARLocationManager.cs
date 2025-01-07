using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Android;

[RequireComponent(typeof(XROrigin))]
public class ARLocationManager : MonoBehaviour
{
    [Header("AR Components")]
    private XROrigin xrOrigin;
    private ARSession arSession;

    [Header("Location Settings")]
    private bool isTrackingLocation = false;
    private float locationUpdateInterval = 1.0f;
    private LocationInfo lastLocation;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    private string debugText = "";

    private void Awake()
    {
        // Get AR components
        xrOrigin = GetComponent<XROrigin>();
        arSession = FindFirstObjectByType<ARSession>();

        // Request permissions
        StartCoroutine(RequestLocationPermission());
    }

    private IEnumerator RequestLocationPermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitForSeconds(0.1f);
        }

        // Start location services
        Input.location.Start(1f, 0.1f);
        yield return new WaitWhile(() => Input.location.status == LocationServiceStatus.Initializing);

        if (Input.location.status == LocationServiceStatus.Running)
        {
            isTrackingLocation = true;
            StartCoroutine(UpdateSensorPositions());
        }
        else
        {
            Debug.LogError("Failed to start location services!");
        }
    }

    private IEnumerator UpdateSensorPositions()
    {
        while (isTrackingLocation)
        {
            lastLocation = Input.location.lastData;

            // Update all sensors
            GPSSensorAnchor[] sensors = FindObjectsByType<GPSSensorAnchor>(FindObjectsSortMode.None);
            foreach (var sensor in sensors)
            {
                UpdateSensorPosition(sensor);
            }

            if (showDebugInfo)
            {
                debugText = $"Current Location: {lastLocation.latitude:F6}, {lastLocation.longitude:F6}\n" +
                           $"Accuracy: {lastLocation.horizontalAccuracy}m";
            }

            yield return new WaitForSeconds(locationUpdateInterval);
        }
    }

    private void UpdateSensorPosition(GPSSensorAnchor sensor)
    {
        // Calculate distance and bearing to sensor
        float distance = CalculateDistance(
            (float)lastLocation.latitude,
            (float)lastLocation.longitude,
            sensor.GetCurrentLatitude(),
            sensor.GetCurrentLongitude()
        );

        float bearing = CalculateBearing(
            (float)lastLocation.latitude,
            (float)lastLocation.longitude,
            sensor.GetCurrentLatitude(),
            sensor.GetCurrentLongitude()
        );

        // Convert to local position
        float x = distance * Mathf.Sin(bearing * Mathf.Deg2Rad);
        float z = distance * Mathf.Cos(bearing * Mathf.Deg2Rad);

        // Update sensor position
        Vector3 newPosition = new Vector3(x, sensor.transform.position.y, z);
        sensor.transform.position = newPosition;
    }

    private float CalculateDistance(float lat1, float lon1, float lat2, float lon2)
    {
        const float R = 6371000f; // Earth's radius in meters
        float dLat = (lat2 - lat1) * Mathf.Deg2Rad;
        float dLon = (lon2 - lon1) * Mathf.Deg2Rad;
        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
                 Mathf.Cos(lat1 * Mathf.Deg2Rad) * Mathf.Cos(lat2 * Mathf.Deg2Rad) *
                 Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        return R * c;
    }

    private float CalculateBearing(float lat1, float lon1, float lat2, float lon2)
    {
        float dLon = (lon2 - lon1) * Mathf.Deg2Rad;
        float lat1Rad = lat1 * Mathf.Deg2Rad;
        float lat2Rad = lat2 * Mathf.Deg2Rad;

        float y = Mathf.Sin(dLon) * Mathf.Cos(lat2Rad);
        float x = Mathf.Cos(lat1Rad) * Mathf.Sin(lat2Rad) -
                 Mathf.Sin(lat1Rad) * Mathf.Cos(lat2Rad) * Mathf.Cos(dLon);
        return Mathf.Atan2(y, x) * Mathf.Rad2Deg;
    }

    private void OnGUI()
    {
        if (showDebugInfo)
        {
            GUI.Label(new Rect(10, 10, 300, 100), debugText);
        }
    }

    private void OnDisable()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            Input.location.Stop();
        }
    }
}