import React from 'react';
import "./Player.css";
import { useLocation } from 'react-router';
import { Link } from 'react-router-dom';
import { Icon } from "web3uikit";

const Player = () => {
  
  const {state: currentlyPLaying} = useLocation();
  return (
  <>
  <div className="playerPage">
  <video autoPlay controls className="videoPlayer">
    <source
      src={currentlyPLaying}
      type="video/mp4"
    >
    </source>
  </video>

  <div className="backHome">
    <Link to="/">
    <Icon 
            className="backButton" 
            fill="rgba(255,255,255,0.25)" 
            size={60} 
            svg="arrowCircleLeft" 
    />
    </Link>

  </div>
  </div>
  </>
)
}

export default Player;
