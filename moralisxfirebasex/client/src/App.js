import { MetaMaskConnector } from "wagmi/connectors/metaMask";
import { useAccount, useConnect, useDisconnect } from "wagmi";
import {useState } from "react";


function App() {
  const { connectAsync } = useConnect();
  const { disconnectAsync } = useDisconnect();
  const { isConnected } = useAccount();
  const [wallet, setWallet] = useState("");

  const handlesignout = async () => {
    console.log("signOut");
  }

  const handleAuth = async () => {
    if (isConnected) {
      await disconnectAsync();
    }

    const { account, chain } = await connectAsync({
      connector: new MetaMaskConnector(),
    });

    const userData = {
      data: {
        address: account,
        chain: chain.id,
      },
    };
    
    console.log(userData);    
    
  };


  return (
    <div>
      <h3>Web3 Authentication with Firebase 🔥 and Metamask 🦊</h3>
      
      {!wallet ?
      <button onClick={() => handleAuth()}>Authenticate via Metamask</button>
      :
      <>
      <button onClick={handlesignout}>SignOut</button>
      <div>{`Signed in as ${wallet}`}</div>
      </>
      }
    </div>
  );
}

export default App;
