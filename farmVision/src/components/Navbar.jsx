import React, { useState } from 'react';
import './css/Navbar.css';

const Navbar = () => {
  const [isMenuOpen, setIsMenuOpen] = useState(false);

  const toggleMenu = () => {
    setIsMenuOpen(!isMenuOpen);
  };

  return (
    <nav className="navbar">
      <div className="navbar-container">
        <h1 className="navbar-logo">FARM VISION</h1>
        <ul className={`nav-menu ${isMenuOpen ? 'active' : ''}`}>
          <li className="nav-item"><a href="#" className="nav-link">Home</a></li>
          <li className="nav-item"><a href="#about" className="nav-link">About</a></li>
          <li className="nav-item"><a href="#start" className="nav-link">Start</a></li>
          <li className="nav-item"><a href="#contact" className="nav-link">Contact Us</a></li>
        </ul>
        <div className="hamburger-menu" onClick={toggleMenu}>
          <span className="bar"></span>
          <span className="bar"></span>
          <span className="bar"></span>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;
