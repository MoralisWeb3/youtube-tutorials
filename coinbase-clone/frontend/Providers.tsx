import React, {useState} from 'react';
import {MoralisProvider} from 'react-moralis';
import Moralis from 'moralis/react-native';
import AsyncStorage from '@react-native-async-storage/async-storage';
// import {enableViaWalletConnect} from './Moralis/enableViaWalletConnect';
// import WalletConnectProvider, {
//   WalletConnectProviderProps,
// } from './WalletConnect';
import {Platform} from 'react-native';
//import Qrcode from "./Qrcode";
//import { expo } from "../app.json";
import {MoralisDappProvider} from './providers/MoralisDappProvider/MoralisDappProvider';
import {ApplicationProvider, Layout, Text} from '@ui-kitten/components';
import {UserContext} from './UserContext';
import * as eva from '@eva-design/eva';
import {
  REACT_APP_MORALIS_APPLICATION_ID,
  REACT_APP_MORALIS_SERVER_URL,
} from '@env';

interface ProvidersProps {
  readonly children: JSX.Element;
}

/**
 * Initialization of Moralis
 */
const appId = REACT_APP_MORALIS_APPLICATION_ID;
const serverUrl = REACT_APP_MORALIS_SERVER_URL;
const environment = 'native';
// Initialize Moralis with AsyncStorage to support react-native storage
Moralis.setAsyncStorage(AsyncStorage);
// Replace the enable function to use the react-native WalletConnect
// @ts-ignore
//Moralis.enable = enableViaWalletConnect;
// console.log(AsyncStorage.getAllKeys(), 'KEYS');

// const walletConnectOptions: WalletConnectProviderProps = {
//   storageOptions: {
//     // @ts-ignore
//     asyncStorage: AsyncStorage,
//   },
//   qrcodeModalOptions: {
//     mobileLinks: [
//       'rainbow',
//       'metamask',
//       'argent',
//       'trust',
//       'imtoken',
//       'pillar',
//     ],
//   },
//   // Uncomment to show a QR-code to connect a wallet
//   //renderQrcodeModal: Qrcode,
// };

export const Providers = ({children}: ProvidersProps) => {
  const [value, setValue] = useState();
  return (
    // <WalletConnectProvider {...walletConnectOptions}>
      <MoralisProvider
        appId={'v81C9aCLc3O13iWwDFO68acUVJkZDr0WpcefSbpN'}
        serverUrl={'https://8okcia8kqyyo.usemoralis.com:2053/server'}
        environment={environment}>
        <MoralisDappProvider>
          <ApplicationProvider {...eva} theme={eva.light}>
            <UserContext.Provider value={{value, setValue}}>
            {children}
            </UserContext.Provider>
          </ApplicationProvider>
        </MoralisDappProvider>
      </MoralisProvider>
    // </WalletConnectProvider>
  );
};
