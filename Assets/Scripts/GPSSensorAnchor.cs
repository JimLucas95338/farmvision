using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class GPSSensorAnchor : MonoBehaviour
{
    [Header("Location Settings")]
    [SerializeField] private string address = "4126 Our Lady Lane, Mariposa, CA";
    [SerializeField] private double latitude = 37.45545247454799;  // Default coordinates for your address
    [SerializeField] private double longitude = -120.00904196548811;

    [Header("Mapping Settings")]
    [SerializeField] private double centerLatitude = 37.45545247454799;  // Center of your farm
    [SerializeField] private double centerLongitude = -120.00904196548811;
    [SerializeField] private float scaleFactor = 100f;  // Meters per Unity unit

    // Add these methods for ARLocationManager
    public float GetCurrentLatitude() => (float)latitude;
    public float GetCurrentLongitude() => (float)longitude;

    private void Start()
    {
        // Position the sensor based on its GPS coordinates relative to farm center
        PositionSensorRelativeToCenter();
    }

    private void PositionSensorRelativeToCenter()
    {
        // Calculate distance from center point in meters
        float[] distances = CalculateDistanceFromCenter();
        float xDistance = distances[0];
        float zDistance = distances[1];

        // Convert to Unity units and set position
        transform.position = new Vector3(
            xDistance / scaleFactor,
            transform.position.y,  // Maintain current height
            zDistance / scaleFactor
        );
    }

    private float[] CalculateDistanceFromCenter()
    {
        // Earth's radius in meters
        const double earthRadius = 6371000;

        // Convert to radians
        double lat1 = centerLatitude * Mathf.Deg2Rad;
        double lon1 = centerLongitude * Mathf.Deg2Rad;
        double lat2 = latitude * Mathf.Deg2Rad;
        double lon2 = longitude * Mathf.Deg2Rad;

        // Calculate differences
        double deltaLat = lat2 - lat1;
        double deltaLon = lon2 - lon1;

        // Calculate approximate distances
        float xDistance = (float)(earthRadius * deltaLon * Math.Cos((lat1 + lat2) / 2));
        float zDistance = (float)(earthRadius * deltaLat);

        return new float[] { xDistance, zDistance };
    }

    public void SetGPSCoordinates(double newLatitude, double newLongitude)
    {
        latitude = newLatitude;
        longitude = newLongitude;
        PositionSensorRelativeToCenter();
    }

    // For visualizing the sensor's position in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawLine(new Vector3(0, 0, 0), transform.position);
    }
}