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
    [SerializeField] private float locationUpdateInterval = 1.0f;
    [SerializeField] private float minAccuracyThreshold = 20f; // Meters
    [SerializeField] private int locationSmoothingBufferSize = 5;
    private bool isTrackingLocation = false;
    private LocationInfo lastLocation;
    private Queue<LocationData> locationBuffer;

    [Header("Compass Settings")]
    [SerializeField] private bool useCompassForRotation = true;
    [SerializeField] private float compassUpdateInterval = 0.1f;
    private bool isCompassEnabled = false;
    private float lastCompassAngle = 0f;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private bool showLocationAccuracy = true;
    [SerializeField] private bool showCompassAccuracy = true;
    private string debugText = "";

    // Class to hold location data for smoothing
    private class LocationData
    {
        public float latitude;
        public float longitude;
        public float accuracy;
        public float timestamp;

        public LocationData(float lat, float lon, float acc, float time)
        {
            latitude = lat;
            longitude = lon;
            accuracy = acc;
            timestamp = time;
        }
    }

    private class SmoothedLocation
    {
        public float latitude;
        public float longitude;
        public float accuracy;

        public SmoothedLocation(float lat, float lon, float acc)
        {
            latitude = lat;
            longitude = lon;
            accuracy = acc;
        }
    }

    private void Awake()
    {
        xrOrigin = GetComponent<XROrigin>();
        arSession = FindFirstObjectByType<ARSession>();
        locationBuffer = new Queue<LocationData>();

        StartCoroutine(InitializeLocationServices());
    }

    private IEnumerator InitializeLocationServices()
    {
        // Request location permission
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitForSeconds(0.1f);

            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Debug.LogError("Location permission denied!");
                yield break;
            }
        }

        // Start location services
        bool initializationSuccessful = false;
        try
        {
            Input.location.Start(1f, 0.1f);
            initializationSuccessful = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error initializing location services: {e.Message}");
        }

        if (initializationSuccessful)
        {
            yield return new WaitWhile(() => Input.location.status == LocationServiceStatus.Initializing);

            switch (Input.location.status)
            {
                case LocationServiceStatus.Running:
                    isTrackingLocation = true;
                    StartCoroutine(UpdateSensorPositions());
                    if (useCompassForRotation) StartCoroutine(UpdateCompass());
                    break;
                case LocationServiceStatus.Failed:
                    Debug.LogError("Location services failed to initialize!");
                    break;
                case LocationServiceStatus.Stopped:
                    Debug.LogError("Location services are disabled!");
                    break;
            }
        }
    }

    private IEnumerator UpdateCompass()
    {
        if (!Input.compass.enabled)
        {
            Input.compass.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        isCompassEnabled = true;
        while (isCompassEnabled)
        {
            lastCompassAngle = Input.compass.trueHeading;
            if (xrOrigin != null)
            {
                xrOrigin.Camera.transform.rotation = Quaternion.Euler(0, lastCompassAngle, 0);
            }
            yield return new WaitForSeconds(compassUpdateInterval);
        }
    }

    private IEnumerator UpdateSensorPositions()
    {
        while (isTrackingLocation)
        {
            if (Input.location.status == LocationServiceStatus.Running)
            {
                LocationInfo newLocation = Input.location.lastData;

                // Only update if accuracy is good enough
                if (newLocation.horizontalAccuracy <= minAccuracyThreshold)
                {
                    // Add to smoothing buffer
                    LocationData locationData = new LocationData(
                        newLocation.latitude,
                        newLocation.longitude,
                        newLocation.horizontalAccuracy,
                        Time.time
                    );

                    AddToLocationBuffer(locationData);
                    SmoothedLocation smoothedLocation = GetSmoothedLocation();
                    lastLocation = newLocation; // Keep original LocationInfo for reference

                    // Update sensors using smoothed location
                    UpdateAllSensors(smoothedLocation);

                    // Update debug info
                    UpdateDebugInfo(smoothedLocation);
                }
                else
                {
                    debugText = $"Poor GPS accuracy: {newLocation.horizontalAccuracy}m\nWaiting for better signal...";
                }
            }

            yield return new WaitForSeconds(locationUpdateInterval);
        }
    }

    private void AddToLocationBuffer(LocationData location)
    {
        locationBuffer.Enqueue(location);
        if (locationBuffer.Count > locationSmoothingBufferSize)
        {
            locationBuffer.Dequeue();
        }
    }

    private SmoothedLocation GetSmoothedLocation()
    {
        if (locationBuffer.Count == 0)
            return new SmoothedLocation(
                lastLocation.latitude,
                lastLocation.longitude,
                lastLocation.horizontalAccuracy
            );

        float sumLat = 0, sumLon = 0, sumAcc = 0;
        float weightSum = 0;

        foreach (var loc in locationBuffer)
        {
            float weight = 1f / (loc.accuracy + 1f);
            sumLat += loc.latitude * weight;
            sumLon += loc.longitude * weight;
            sumAcc += loc.accuracy;
            weightSum += weight;
        }

        return new SmoothedLocation(
            sumLat / weightSum,
            sumLon / weightSum,
            sumAcc / locationBuffer.Count
        );
    }

    private void UpdateAllSensors(SmoothedLocation smoothedLocation)
    {
        GPSSensorAnchor[] sensors = FindObjectsByType<GPSSensorAnchor>(FindObjectsSortMode.None);
        foreach (var sensor in sensors)
        {
            UpdateSensorPosition(sensor, smoothedLocation);
        }
    }

    private void UpdateSensorPosition(GPSSensorAnchor sensor, SmoothedLocation smoothedLocation)
    {
        float distance = CalculateDistance(
            smoothedLocation.latitude,
            smoothedLocation.longitude,
            sensor.GetCurrentLatitude(),
            sensor.GetCurrentLongitude()
        );

        float bearing = CalculateBearing(
            smoothedLocation.latitude,
            smoothedLocation.longitude,
            sensor.GetCurrentLatitude(),
            sensor.GetCurrentLongitude()
        );

        float x = distance * Mathf.Sin(bearing * Mathf.Deg2Rad);
        float z = distance * Mathf.Cos(bearing * Mathf.Deg2Rad);

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

    private void UpdateDebugInfo(SmoothedLocation smoothedLocation)
    {
        if (!showDebugInfo) return;

        string accuracyInfo = showLocationAccuracy ?
            $"\nAccuracy: {smoothedLocation.accuracy:F1}m" : "";
        string compassInfo = showCompassAccuracy && isCompassEnabled ?
            $"\nHeading: {lastCompassAngle:F1}°" : "";

        debugText = $"Location: {smoothedLocation.latitude:F6}, {smoothedLocation.longitude:F6}" +
                   accuracyInfo +
                   compassInfo;
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
        StopAllCoroutines();
        if (Input.location.status == LocationServiceStatus.Running)
        {
            Input.location.Stop();
        }
        if (Input.compass.enabled)
        {
            Input.compass.enabled = false;
            isCompassEnabled = false;
        }
    }
}