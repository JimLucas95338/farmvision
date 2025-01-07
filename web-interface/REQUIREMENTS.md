# Technical Requirements for Web Interface

## Data Structures to Match

### Sensor Data Format
```typescript
interface SensorData {
    temperature: number;    // Range: 15°C to 35°C
    humidity: number;       // Range: 30% to 80%
    updateInterval: number; // 2 seconds (matching SensorDataSimulator.cs)
    displayFormat: string;  // Format: "Temp: {0:F1}°C\nHumidity: {1:F1}%"
}
```

### GPS Location Format
```typescript
interface LocationData {
    latitude: number;      // From GPSSensorAnchor.cs
    longitude: number;     // From GPSSensorAnchor.cs
    accuracy: number;      // From ARLocationManager.cs
    timestamp: number;
}
```

## Core Features Required

1. Sensor Management
   - Display list of all active sensors
   - Show real-time temperature and humidity readings
   - Update data every 2 seconds (matching Unity update interval)
   - Color-code sensors based on temperature ranges:
     * Above 30°C: Red
     * Below 20°C: Blue
     * Between: Green

2. Map Visualization
   - Show sensor locations on a map
   - Center coordinates: 37.45545247454799, -120.00904196548811 (from GPSSensorAnchor.cs)
   - Allow clicking sensors to view details

3. AR Transition
   - Generate QR code or link containing:
     * Current sensor states
     * GPS coordinates
     * Configuration settings
   - Enable seamless handoff to AR view

## Integration Points

1. Unity Data Matching
   - Use exact same data ranges as SensorDataSimulator.cs
   - Match GPSSensorAnchor.cs coordinate system
   - Follow SensorLabel.cs display formatting

2. Real-time Updates
   - Maintain 2-second update interval
   - Handle sensor state changes
   - Sync with AR view state

## Design Requirements

1. User Interface
   - Clean, minimal design
   - Mobile-responsive layout
   - Clear sensor status indicators
   - Easy-to-read data displays

2. Performance
   - Handle multiple sensor updates
   - Smooth real-time data visualization
   - Efficient map rendering

## Technical Stack Suggestions

- React (Web Framework)
- Map Library (Your choice - e.g., Leaflet)
- Real-time Updates (WebSocket or Polling)
- Data Visualization (Your preferred charting library)

## Repository Structure
```
farmvision/
├── Unity AR Project/
└── web-interface/       <- Your workspace
```
