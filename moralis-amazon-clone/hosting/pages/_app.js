import "../styles/globals.css";
import Head from "next/head";
import { FirebaseInitializer } from "../components/FirebaseInitializer";

export default function App({ Component, pageProps }) {
  return (
    <>
      <Head>
        <link rel="shortcut icon" href="favicon.ico" />
      </Head>
      <FirebaseInitializer
        initializing={() => <AppLoading />}
        initialized={() => <Component {...pageProps} />}
      />
    </>
  );
}

function AppLoading() {
  return <p>Loading app...</p>;
}
