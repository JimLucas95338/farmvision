# FarmVision Web Interface

A web-based dashboard for monitoring and controlling AR-based farm sensors.

## Features

- Real-time sensor data visualization
- Interactive map showing sensor locations
- Temperature and humidity monitoring
- QR code generation for AR view transition
- Responsive design for desktop and mobile web browsers

## Tech Stack

- React (Create React App)
- WebSocket for real-time updates
- Chart.js for data visualization
- QR code generation for AR handoff

## Setup Instructions

1. Install dependencies:
```bash
npm install
```

2. Start development server:
```bash
npm start
```

3. Build for production:
```bash
npm run build
```

## Project Structure

```
src/
├── components/         # React components
│   ├── Map/           # Map visualization
│   ├── SensorList/    # Sensor monitoring
│   └── QRCode/        # AR transition
├── services/          # API and WebSocket services
├── utils/             # Helper functions
└── App.js            # Main application component
```

## Integration Points

- Matches Unity sensor data format
- Synchronized update intervals with AR application
- Consistent coordinate system with GPS anchors

## Development Guidelines

1. Follow the existing sensor data structure
2. Maintain consistent styling with AR interface
3. Test on both desktop and mobile browsers
4. Consider performance for real-time updates

## API Documentation

Api Endpoints (https://67819d9dc51d092c3dccff30.mockapi.io/api/v1/sensors)
