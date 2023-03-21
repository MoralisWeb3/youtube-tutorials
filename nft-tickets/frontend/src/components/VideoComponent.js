import React, { useEffect, useRef } from "react";
import { useLocation } from "react-router-dom";
import styles from "../styles/Home.module.css";

const VideoComponent = () => {
  const location = useLocation();
  const hasAccess = location.state.hasAccess;
  const videoRef = useRef();
  const videoSrc =
    "https://www.youtube.com/embed/L6kvLnHJHZA?autoplay=1&controls=0&modestbranding=1&rel=0&showinfo=0";

  useEffect(() => {
    if (hasAccess && videoRef.current) {
      videoRef.current.play();
    }
  }, [hasAccess]);

  return (
    <div>
      {hasAccess ? (
        <div className={styles.video_container}>
          <iframe
            title="YouTube Video"
            width="100%"
            height="100%"
            src={videoSrc}
            allowTransparency
            allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
            allowFullScreen
            style={{
              position: "absolute",
              top: 0,
              left: 0,
              width: "100%",
              height: "100%",
            }}
          />
        </div>
      ) : (
        <div className={styles.video_component}>
          <h1>Access Denied</h1>
          <p>
            You do not have access to this video. Please obtain the required
            NFT.
          </p>
        </div>
      )}
    </div>
  );
};

export default VideoComponent;
