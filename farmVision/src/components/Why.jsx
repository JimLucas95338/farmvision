import React from 'react';
import './css/why.css';

function WhySection() {
  return (
    <div className="why-section">
      <h2>Why?</h2>
      <div className="why-cards">
        <div className="card">
          <img src="https://via.placeholder.com/50" alt="Climate Change" />
          <h3>Climate Change</h3>
          <p>Variability in temperature is making past data unreliable to forecast maximum yield.</p>
        </div>
        <div className="card">
          <img src="https://via.placeholder.com/50" alt="Data Accessibility" />
          <h3>Data Accessibility</h3>
          <p>Environmental data is often hidden in complex dashboards and spreadsheets.</p>
        </div>
        <div className="card">
          <img src="https://via.placeholder.com/50" alt="New Crops" />
          <h3>New Crops</h3>
          <p>Modified crops might require different types of monitoring for optimal growing conditions.</p>
        </div>
      </div>
    </div>
  );
}

export default WhySection;
