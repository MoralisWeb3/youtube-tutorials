import { MoralisProvider } from "react-moralis";
import "../styles/globals.css";

function MyApp({ Component, pageProps }) {
  return (
    <MoralisProvider
      serverUrl="https://joeqw7xxmpq7.usemoralis.com:2053/server"
      appId="t8ilvsG7SWBncSDRCQ5iE5sw7BLR46By3ckhAp9i"
    >
      <Component {...pageProps} />
    </MoralisProvider>
  );
}

export default MyApp;
