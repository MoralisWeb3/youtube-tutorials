import styles from "./styles/Home.module.css";
import Logo from "./img/Moralis_logo.png";
import Tickets from "./img/tickets.png";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import VideoComponent from "./components/VideoComponent";
import "./App.css";
import MainComponent from "./components/MainComponent";

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<MainComponent />} />
        <Route path="/video" element={<VideoComponent />} />
      </Routes>
    </Router>
  );
}

export default App;
