import Button from '@mui/material/Button';
import { useState } from 'react';
import axios from 'axios';
import './App.css';

function App() {

  const [profileId, setProfileId] = useState(null);
  const [userWallet, setUserWallet] = useState(null);

  const getAptosWallet = () => {
    if ('aptos' in window) {
      return window.aptos;
    } else {
      window.open('https://petra.app/', '_blank')
    }
  }

  const connectWallet = async () => {
    const wallet = getAptosWallet();
    try {
      const response = await wallet.connect();
      const account = await wallet.account()

      const { data } = await axios.get(`http://localhost:3000/requestChallenge`, {
        params: { address: account.address, publicKey: account.publicKey }
      })

      const message = data.message;

      const { signature, fullMessage } = await wallet.signMessage({
        address: false,
        application: false,
        chainId: false,
        message: message,
        nonce: null
      });

      const verification = await axios.get(`http://localhost:3000/verifyChallenge`, {
        params: { message: fullMessage, signature: signature }
      })

      setProfileId(verification?.data?.profileId)
      setUserWallet(verification?.data?.address)

    } catch (err) { console.log(err) }
  }



  return (
    <div className="App">
      <h1>Aptos Web3 Authentication with Petra</h1>

      {profileId ? <>
        <h3>Profile ID: {profileId}</h3>
        <h3 >Wallet Address: {userWallet}</h3>
        <Button variant='outlined' onClick={() => setProfileId(null)}>Logout</Button>
      </> : <Button variant='contained' onClick={connectWallet}> Connect Wallet</Button>}
    </div>


  );
}

export default App;
