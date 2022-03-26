import React from 'react';
import { Link } from "react-router-dom";
import "./Home.css";

const Home = () => {

return(
  <>
  <div className="container">
     <Link to="/player" className="link">Player Page</Link>
  </div>
  </>
)
}

export default Home;
