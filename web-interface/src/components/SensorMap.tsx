'use client';

import React, { useEffect, useState } from 'react';
import { MapContainer, TileLayer, Marker, Popup } from 'react-leaflet';
import type { LatLngTuple } from 'leaflet';
import 'leaflet/dist/leaflet.css';
import L from 'leaflet';
import { QRCodeSVG } from 'qrcode.react';
import * as DialogPrimitive from "@radix-ui/react-dialog";
import { Button } from "@/components/ui/button";
import { Sensor, ARModalProps } from '@/types/sensor';
import { Thermometer, Droplets, Radio, AlertTriangle, QrCode } from 'lucide-react';

const Navbar: React.FC = () => {
  return (
    <nav className="bg-black text-white sticky top-0 z-50">
      <div className="container mx-auto px-4 py-3 flex justify-between items-center">
      <div
  className="text-5xl font-bold tracking-wide"
  style={{
    fontFamily: 'var(--font-farmvision)',
    color: 'var(--color-farmvision-green)',
    textShadow: '0 2px 4px rgba(0, 0, 0, 0.8)',
  }}
>
  FARM VISION
</div>
        <ul className="flex gap-6">
          <li><a href="#home" className="hover:text-green-500">Home</a></li>
          <li><a href="#about" className="hover:text-green-500">About</a></li>
          <li><a href="#start" className="hover:text-green-500">Start</a></li>
          <li><a href="#contact" className="hover:text-green-500">Contact Us</a></li>
        </ul>
      </div>
    </nav>
  );
};

const ARModal: React.FC<ARModalProps> = ({ isOpen, onClose, sensorId, sensorName }) => {
  const arAppURL = `farmvision://sensor/${sensorId}`;

  return (
    <DialogPrimitive.Root open={isOpen} onOpenChange={onClose}>
      <DialogPrimitive.Portal>
        <DialogPrimitive.Overlay className="fixed inset-0 bg-black/50 backdrop-blur-sm z-[1050]" />
        <DialogPrimitive.Content className="fixed top-[50%] left-[50%] translate-x-[-50%] translate-y-[-50%] w-[90vw] max-w-[425px] rounded-xl bg-white p-6 shadow-2xl z-[1100]">
          <div className="absolute top-3 right-3 bg-slate-100 rounded-full p-3">
            <QrCode className="w-6 h-6 text-slate-600" />
          </div>
          <DialogPrimitive.Title className="text-xl font-semibold text-slate-900 mt-2">
            View Sensor {sensorName}
          </DialogPrimitive.Title>
          <DialogPrimitive.Description className="text-sm text-slate-600 mt-1">
            Scan this QR code with your mobile device to view the sensor in augmented reality.
          </DialogPrimitive.Description>
          <div className="mt-6 space-y-6">
            <div className="flex justify-center bg-slate-50 rounded-lg p-4">
              <QRCodeSVG 
                value={arAppURL} 
                size={256}
                level="H"
                includeMargin
              />
            </div>
            <div className="space-y-3 bg-blue-50 rounded-lg p-4">
              <h4 className="font-medium text-blue-900 flex items-center gap-2">
                <Radio className="w-4 h-4" />
                New to FarmVision AR?
              </h4>
              <ol className="list-decimal ml-4 text-sm text-blue-800 space-y-2">
                <li>Download FarmVision AR from your device&apos;s app store</li>
                <li>Open the app and sign in with your farm credentials</li>
                <li>Scan this QR code or tap the button below</li>
                <li>Point your device at the sensor location</li>
              </ol>
            </div>
            <div className="flex justify-between gap-3 pt-2">
              <Button 
                variant="outline" 
                onClick={onClose}
                className="flex-1"
              >
                Close
              </Button>
              <Button 
                onClick={() => {
                  console.log("Opening AR app with URL:", arAppURL); // Debugging log
                  window.location.href = arAppURL;
                }}
                className="flex-1 bg-blue-600 hover:bg-blue-700"
              >
                Open in AR App
              </Button>
            </div>
          </div>
        </DialogPrimitive.Content>
      </DialogPrimitive.Portal>
    </DialogPrimitive.Root>
  );
};

const SensorMap: React.FC = () => {
  const [sensors, setSensors] = useState<Sensor[]>([]);
  const [selectedSensor, setSelectedSensor] = useState<Sensor | null>(null);
  
  const center: LatLngTuple = [37.45545, -120.00904];

  const launchAR = (sensor: Sensor) => {
    console.log("Launching AR for sensor:", sensor); // Debugging log
    setSelectedSensor(sensor);
  };

  useEffect(() => {
    const MOCK_SENSORS: Sensor[] = [
      { id: 1, name: "Field Sensor A1", latitude: 37.45545, longitude: -120.00904, temperature: 23.5, humidity: 65, isInRange: true },
      { id: 2, name: "Field Sensor B2", latitude: 37.45585, longitude: -120.00854, temperature: 24.2, humidity: 62, isInRange: true },
      { id: 3, name: "Field Sensor C3", latitude: 37.45515, longitude: -120.00934, temperature: 29.8, humidity: 45, isInRange: false }
    ];

    setSensors(MOCK_SENSORS);
  }, []);

  return (
    <>
      <Navbar />
      <div className="flex flex-col h-[calc(100vh-2rem)] max-h-[900px] bg-white rounded-xl shadow-sm overflow-hidden">
        <div className="flex-1 relative">
          <MapContainer 
            center={center} 
            zoom={18} 
            className="h-full w-full"
            scrollWheelZoom={true}
          >
            <TileLayer
              url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
              attribution='&copy; OpenStreetMap contributors'
            />
            
            {sensors.map(sensor => (
              <Marker
                key={sensor.id}
                position={[sensor.latitude, sensor.longitude] as LatLngTuple}
                icon={L.divIcon({
                  className: 'custom-marker',
                  html: `<div class="relative">
                    <div style="
                      background-color: ${sensor.isInRange ? '#22c55e' : '#ef4444'}; 
                      width: 24px; 
                      height: 24px; 
                      border-radius: 50%;
                      border: 3px solid white;
                      box-shadow: 0 2px 4px rgba(0,0,0,0.2);
                    "></div>
                    ${!sensor.isInRange ? `
                      <div class="absolute -top-1 -right-1 w-3 h-3 bg-red-500 rounded-full border-2 border-white"></div>
                    ` : ''}
                  </div>`
                })}
              >
                <Popup>
                  <div className="min-w-[240px]">
                    <div className="flex items-start justify-between gap-2 mb-3">
                      <h3 className="text-lg font-semibold text-slate-900">{sensor.name}</h3>
                      {!sensor.isInRange && (
                        <div className="flex items-center gap-1 text-xs font-medium text-red-600 bg-red-50 px-2 py-1 rounded-full">
                          <AlertTriangle className="w-3 h-3" />
                          Warning
                        </div>
                      )}
                    </div>
                    
                    <div className="space-y-2 mb-4">
                      <div className="flex items-center justify-between text-sm">
                        <div className="flex items-center gap-2 text-slate-600">
                          <Thermometer className="w-4 h-4" />
                          Temperature
                        </div>
                        <span className="font-medium text-slate-900">{sensor.temperature}°C</span>
                      </div>
                      <div className="flex items-center justify-between text-sm">
                        <div className="flex items-center gap-2 text-slate-600">
                          <Droplets className="w-4 h-4" />
                          Humidity
                        </div>
                        <span className="font-medium text-slate-900">{sensor.humidity}%</span>
                      </div>
                    </div>

                    <Button 
                      onClick={() => launchAR(sensor)}
                      className="w-full bg-blue-600 hover:bg-blue-700"
                    >
                      View in AR
                    </Button>
                  </div>
                </Popup>
              </Marker>
            ))}
          </MapContainer>
        </div>

        <ARModal
          isOpen={!!selectedSensor}
          onClose={() => {
            console.log("Closing modal"); // Debugging log
            setSelectedSensor(null);
          }}
          sensorId={selectedSensor?.id}
          sensorName={selectedSensor?.name}
        />
      </div>
    </>
  );
};

export default SensorMap;
