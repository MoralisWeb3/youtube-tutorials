import { useState, useEffect } from "react";
import styles from "../styles/Home.module.css";
import GetTransactions from "./getTransactions";

import { initializeApp } from "firebase/app";
import {
  query,
  collection,
  onSnapshot,
  getFirestore,
} from "firebase/firestore";

const firebaseConfig = {
  apiKey: "AIzaSyB1zvfT4B-A0D_M9ht-4ULFthMyF0kCxmQ",
  authDomain: "moralisweb3wallettracker.firebaseapp.com",
  projectId: "moralisweb3wallettracker",
  storageBucket: "moralisweb3wallettracker.appspot.com",
  messagingSenderId: "608043583374",
  appId: "1:608043583374:web:4e4ae7f734f10c96d606df",
};

const app = initializeApp(firebaseConfig);
const db = getFirestore(app);

export default function LoggedIn() {
  const [streams, setStreams] = useState(null);

  useEffect(() => {
    const q = query(collection(db, "moralis/txs/Goerlitestnet"));
    const unsubscribe = onSnapshot(q, (querySnapshot) => {
      const tempStreams = [];
      querySnapshot.forEach((doc) => {
        tempStreams.push(doc.data());
      });
      setStreams(tempStreams);
    });
  }, []);

  return (
    <section className={styles.loggedInMain}>
      <section className={styles.loggedInAccount}>
        <section className={styles.result}>
          <p className={styles.title}>Transactions</p>
          <section>
            <GetTransactions />
          </section>
        </section>
        <section className={styles.result}>
          <p className={styles.title}>Streams</p>
          <section>
            {streams?.map((streams, i) => {
              return (
                <section className={styles.resultContainer} key={i}>
                  <section className={styles.resultContainer_data}>
                    <p>From: {streams.fromAddress}</p>
                    <p>To: {streams.toAddress}</p>
                    <p>Value: {streams.value / 1e18} GoerliETH</p>
                  </section>
                </section>
              );
            })}
          </section>
        </section>
      </section>
    </section>
  );
}
