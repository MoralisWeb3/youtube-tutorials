import { useMoralisCloudFunction } from 'react-moralis';
import './App.css';
import {useState} from "react"


function App() {
  const [suggestion,setSuggestion] = useState(null)
  const { fetch } = useMoralisCloudFunction("secretCloudCode");

  async function runCloudCode(){
    
    const res = await fetch();
    console.log(res);
    setSuggestion(res);
  }

  return (
    <div className="App">
      <h1>
        What Should I Buy Today? 🤔
      </h1>
      <button onClick={runCloudCode}>
        Let The ☁️ Decide
      </button>

      {suggestion && 
      <>
      <h2>{suggestion.token}</h2> 
      <h4>Current Price ${suggestion.price.toFixed(2)}</h4>
      </>}
    </div>
  );
}

export default App;
