export interface Sensor {
    id: number;
    name: string;
    latitude: number;
    longitude: number;
    temperature: number;
    humidity: number;
    isInRange: boolean;
  }
  
  export interface ARModalProps {
    isOpen: boolean;
    onClose: () => void;
    sensorId?: number;
    sensorName?: string;
  }