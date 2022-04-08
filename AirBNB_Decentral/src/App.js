import React from 'react';
import { Routes, Route } from "react-router-dom";
import Home from "./pages/Home";
import Rentals from './pages/Rentals';
import './App.css';

const App = () => {
  return(
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/rentals" element={<Rentals />} />
    </Routes>
  )
};

export default App;
