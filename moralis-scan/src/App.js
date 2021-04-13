import Moralis from "moralis";
import { BrowserRouter as Router, Switch, Route } from "react-router-dom";

import "./App.css";
import TransResults from "./components/TransResults";
import Home from "./components/Home";
import Header from "./Header";

Moralis.initialize(process.env.REACT_APP_MORALIS_APPLICATION_ID);
Moralis.serverURL = process.env.REACT_APP_MORALIS_SERVER_URL;

function App() {
  return (
    <div className="App">
      <h1>Moralis Scan</h1>
      <Router>
        <Header />
        <Switch>
          <Route path="/address/:address" component={TransResults} />
          <Route exact path="/"component={Home} />
        </Switch>
      </Router>
    </div>
  );
}

export default App;
