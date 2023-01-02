import { useAccount, useConnect } from "wagmi";
import { useState, useEffect } from "react";
import Header from "../components/header";
import LoggedIn from "../components/loggedIn";

import styles from "../styles/Home.module.css";

export default function Home() {
  const { isConnected } = useAccount();
  const { connect, connectors, error, isLoading, pendingConnector } =
    useConnect();

  const [isLoggedIn, setIsLoggedIn] = useState(false);

  useEffect(() => {
    if (!isConnected) {
      setIsLoggedIn(true);
    } else {
      setIsLoggedIn(false);
    }
  }, [isConnected]);

  return (
    <section>
      <Header />
      {isLoggedIn ? (
        <main className={styles.main}>
          <h1 className={styles.title} style={{ marginBottom: "4rem" }}>
            Connect Wallet and Display NFTs
          </h1>
          {connectors.map((connector) => (
            <button
              disabled={!connector.ready}
              key={connector.id}
              onClick={() => connect({ connector })}
            >
              {connector.name}
              {!connector.ready && " (unsupported)"}
              {isLoading &&
                connector.id === pendingConnector?.id &&
                " (connecting)"}
            </button>
          ))}
          {error && <section>{error.message}</section>}
        </main>
      ) : (
        <LoggedIn />
      )}
    </section>
  );
}
