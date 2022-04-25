import React from "react";
import { Link } from "react-router-dom";
import "./Home.css";

const Home = () => {
  return (
    <>
      <Link to="/">
        <div>Home</div>
      </Link>
      <Link to="/profile">
        <div>Profile</div>
      </Link>

      <Link to="/settings">
        <div>Settings</div>
      </Link>
    </>
  );
};

export default Home;
