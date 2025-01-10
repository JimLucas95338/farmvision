import React, { useState } from 'react';
import img1 from "../assets/farm1.png"
import img2 from "../assets/farm2.png"
import './css/home.css';

const images = [
  img1, img2
];

function Home() {
  const [currentSlide, setCurrentSlide] = useState(0);

  const nextSlide = () => {
    setCurrentSlide((prevSlide) => (prevSlide + 1) % images.length);
  };

  const prevSlide = () => {
    setCurrentSlide((prevSlide) => (prevSlide - 1 + images.length) % images.length);
  };

  return (
    <div className="slider-container">
      <div className="slider">
        <img src={images[currentSlide]} alt={`Slide ${currentSlide + 1}`} />
        <button className="start-button">Start</button>
      </div>
      <button className="arrow left" onClick={prevSlide}>
        &#9664;
      </button>
      <button className="arrow right" onClick={nextSlide}>
        &#9654;
      </button>
    </div>
  );
}

export default Home;
