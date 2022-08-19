import { MetaMaskConnector } from "wagmi/connectors/metaMask";
import { useAccount, useConnect, useSignMessage, useDisconnect } from "wagmi";
import {useState, useEffect} from "react";
import { initializeApp } from "firebase/app";
import { getAuth, signInWithCustomToken, onAuthStateChanged, signOut} from "firebase/auth";
import axios from "axios";

const firebaseConfig = {
  xxx:"get from firebase admin panel"
};

function App() {
  initializeApp(firebaseConfig);
  const { connectAsync } = useConnect();
  const { disconnectAsync } = useDisconnect();
  const { isConnected } = useAccount();
  const { signMessageAsync } = useSignMessage();
  const auth = getAuth();
  const [wallet, setWallet] = useState("");

  async function handlesignout(){
    signOut(auth);
  }

  useEffect(() => {
    onAuthStateChanged(auth, (user) => {
      if(user){
        setWallet(user.displayName);
      }
      else {
        setWallet("");
      }
    })
  }, [auth])

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

    console.log("Requested Message From Moralis");

    const { data } = await axios.post(
      "xxx",
      userData,
      {
        headers: {
          "content-type": "application/json",
        },
      }
    );
    console.log("Received Message From Moralis");

    const message = data.result.message;

    const signature = await signMessageAsync({ message });

    const verification = {
      data: {
        message: message,
        signature: signature,
      },
    };
    
    console.log("Sent Signature to Moralis");

    await axios.post(
      "xxx",
      verification,
      {
        headers: {
          "content-type": "application/json",
        },
      }
    ).then(async (res)=>{
      try{
        await signInWithCustomToken(auth, res.data.result.token);
        console.log("Signature Verified and Received Firebase Token");
        }catch(e){
          console.log(e);
        }
    })
    

    
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
