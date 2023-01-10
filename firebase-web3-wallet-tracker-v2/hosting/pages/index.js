import { signInWithMoralis } from "@moralisweb3/client-firebase-evm-auth";
import Head from "next/head";
import { useState } from "react";
import { useFirebase } from "../components/FirebaseInitializer";
import GetStreams from "../components/getStreams";

export default function Home() {
  const { auth, moralisEvmAuth } = useFirebase();
  const [currentUser, setCurrentUser] = useState(
    () => auth.currentUser?.displayName
  );

  async function signInByMetaMask() {
    await signInWithMoralis(moralisEvmAuth);
    setCurrentUser(auth.currentUser?.displayName);
  }

  async function signOut() {
    await auth.signOut();
    setCurrentUser(null);
  }

  return (
    <>
      <Head>
        <title>Firebase NextJs Demo</title>
      </Head>
      <p>
        User:{" "}
        {currentUser
          ? `${currentUser.slice(0, 4)}...${currentUser.slice(38)}`
          : "-"}
      </p>
      <p>
        {!currentUser && (
          <button onClick={signInByMetaMask}>Sign in by MetaMask</button>
        )}{" "}
        {currentUser && <button onClick={signOut}>Sign out</button>}
      </p>
      {currentUser && (
        <section>
          <GetStreams />
        </section>
      )}
    </>
  );
}
