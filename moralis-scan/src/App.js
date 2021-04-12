import Moralis from "moralis";
import { useState } from "react";
import "./App.css";
import Search from "./components/Search";
import TransResults from "./components/TransResults";

Moralis.initialize(process.env.REACT_APP_MORALIS_APPLICATION_ID);
Moralis.serverURL = process.env.REACT_APP_MORALIS_SERVER_URL;

function App() {
  const [userAddress, setUserAddress] = useState("");
  const handleSearch = (address) => {
    setUserAddress(address);
  }
  return (
    <div className="App">
      <h1>Moralis Scan</h1>
      <Search handleSearch={handleSearch} />
      <TransResults userAddress={userAddress} />
    </div>
  );
}

export default App;
