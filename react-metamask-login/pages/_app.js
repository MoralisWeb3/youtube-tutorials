import "../styles/globals.css";
import { MoralisProvider } from "react-moralis";
function MyApp({ Component, pageProps }) {
  return (
    <MoralisProvider
      appId="mvyRs8xlnowuBXGIunYtEA0aAkiP8fGoJRuxDsb1"
      serverUrl="https://4j2uzgeb6j4x.usemoralis.com:2053/server"
    >
      {<Component {...pageProps} />}
    </MoralisProvider>
  );
}

export default MyApp;
