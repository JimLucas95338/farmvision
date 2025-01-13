import { NextResponse } from 'next/server';
import type { Sensor } from '@/types/sensor';

const MOCK_SENSORS: Sensor[] = [
  {
    id: 1,
    name: "Field Sensor A1",
    latitude: 37.45545,
    longitude: -120.00904,
    temperature: 23.5,
    humidity: 65,
    isInRange: true
  },
  {
    id: 2,
    name: "Field Sensor B2",
    latitude: 37.45585,
    longitude: -120.00854,
    temperature: 24.2,
    humidity: 62,
    isInRange: true
  },
  {
    id: 3,
    name: "Field Sensor C3",
    latitude: 37.45515,
    longitude: -120.00934,
    temperature: 29.8,
    humidity: 45,
    isInRange: false
  }
];

export async function GET() {
  return NextResponse.json(MOCK_SENSORS);
}