import React from 'react';
import { Routes, Route } from "react-router-dom";
import Home from "./pages/Home";
import Player from './pages/Player';
import './App.css';

const App = () => {
  return(
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/player" element={<Player />} />
    </Routes>
  )
};

export default App;
