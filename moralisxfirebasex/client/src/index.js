import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';
import { WagmiConfig, createClient } from 'wagmi'
import { getDefaultProvider } from 'ethers'

const client = createClient({
  autoConnect: true,
  provider: getDefaultProvider(),
})

const root = ReactDOM.createRoot(document.getElementById('root'));

root.render(
  <WagmiConfig client={client}>
  <React.StrictMode>
    <App />
  </React.StrictMode>
  </WagmiConfig>
);
