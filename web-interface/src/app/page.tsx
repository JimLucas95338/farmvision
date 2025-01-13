'use client';

import dynamic from 'next/dynamic';

const SensorMap = dynamic(
  () => import('@/components/SensorMap'),
  { ssr: false }
);

export default function Home() {
  return (
    <main className="p-4">
      <SensorMap />
    </main>
  );
}