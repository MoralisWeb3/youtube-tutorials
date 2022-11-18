import './App.css';
import axios from "axios";
import { MetaMaskConnector } from 'wagmi/connectors/metaMask';
import { useAccount, useConnect, useSignMessage, useDisconnect } from 'wagmi';
import { useState } from 'react';

function App() {

  const { connectAsync } = useConnect();
  const { disconnectAsync } = useDisconnect();
  const { isConnected } = useAccount();
  const { signMessageAsync } = useSignMessage();
  const [profileId, setProfileId] = useState(null);

  async function login(){

    if (isConnected) {
      await disconnectAsync();
    }

    const { account, chain } = await connectAsync({ connector: new MetaMaskConnector() });

    const {data} = await axios.get(`http://localhost:3000/requestChallenge`, {
      params: { address: account, chainId: chain.id },
    });

    const message = data.message;

    const signature = await signMessageAsync({ message });

    const verification = await axios.get(`http://localhost:3000/verifyChallenge`, {
      params: { message: message, signature: signature },
    });

    setProfileId(verification?.data?.profileId)

  }

  return (
    <div className="App">
      <h1>Python Web3 Authentication 🐍</h1>
      {profileId ?
      <>
      <h3>Profile ID: {profileId}</h3>
      <button onClick={()=> setProfileId(null)}>Logout</button>
      </>:
      <button onClick={login}>Login</button>
      }
    </div>
  );
}

export default App;
