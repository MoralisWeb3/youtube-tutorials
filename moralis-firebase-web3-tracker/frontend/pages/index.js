import { useState, useEffect } from "react";
import styles from "../styles/Home.module.css";
import { ConnectButton } from "@rainbow-me/rainbowkit";
import { useAccount } from "wagmi";

import Header from "../components/header";
import LoggedIn from "../components/loggedIn";

export default function Home() {
  const { isConnected } = useAccount();
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  useEffect(() => {
    if (!isConnected) {
      setIsLoggedIn(true);
    } else {
      setIsLoggedIn(false);
    }
  }, [isConnected]);
  return (
    <section className={styles.container}>
      <Header />
      {isLoggedIn ? (
        <main className={styles.main}>
          <h1 className={styles.title} style={{ marginBottom: "4rem" }}>
            Firebase Wallet Tracker
          </h1>
          <ConnectButton />
        </main>
      ) : (
        <LoggedIn />
      )}
    </section>
  );
}
