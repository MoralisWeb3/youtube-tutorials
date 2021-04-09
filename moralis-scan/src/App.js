import Moralis from "moralis";
import "./App.css";
import TransResults from "./components/TransResults";
import { useMoralisCloudQuery } from "./hooks/cloudQuery";

Moralis.initialize(process.env.REACT_APP_MORALIS_APPLICATION_ID);
Moralis.serverURL = process.env.REACT_APP_MORALIS_SERVER_URL;

const options = {
  params: { userAddress: "0x29781d9fca70165cbc952b8c558d528b85541f0b" }
};

function App() {
  const { data } = useMoralisCloudQuery("getTransactions", options);
  console.log(data);

  return (
    <div className="App">
      <h1>Moralis Scan</h1>
      <TransResults />
    </div>
  );
}

export default App;
