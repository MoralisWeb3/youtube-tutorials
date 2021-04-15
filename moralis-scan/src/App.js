import Moralis from "moralis";
import { BrowserRouter as Router, Switch, Route } from "react-router-dom";

import "./App.css";
import Home from "./components/Home";
import Header from "./Header";
import Transactions from "./components/Transactions";

Moralis.initialize(process.env.REACT_APP_MORALIS_APPLICATION_ID);
Moralis.serverURL = process.env.REACT_APP_MORALIS_SERVER_URL;

function App() {
  return (
    <div className="container">
      <Router>
        <Header />
        <Switch>
          <Route path="/address/:address" component={Transactions} />
          <Route exact path="/"component={Home} />
        </Switch>
      </Router>
    </div>
  );
}

export default App;
