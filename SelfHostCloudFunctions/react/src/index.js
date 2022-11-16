import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';
import {MoralisProvider} from "react-moralis"


const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <MoralisProvider appId={'001'} serverUrl={'http://localhost:1337/server'}>
    <App />
    </MoralisProvider>
  </React.StrictMode>
);

