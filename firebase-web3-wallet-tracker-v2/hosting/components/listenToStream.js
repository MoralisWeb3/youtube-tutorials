import { useEffect, useState } from "react";
import styles from "../styles/Home.module.css";
import { useFirebase } from "./FirebaseInitializer";
import { app } from "./Firebase";
import {
  query,
  collection,
  onSnapshot,
  getFirestore,
} from "firebase/firestore";

import GetTransactions from "./getTransactions";

const db = getFirestore(app);

export default function ListenToStream() {
  const { auth } = useFirebase();
  const [address, setAddress] = useState(() => auth.currentUser?.displayName);
  const [streams, setStreams] = useState(null);

  useEffect(() => {
    const q = query(collection(db, `moralis/txs/${address.toLowerCase()}`));
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
          <GetTransactions />
        </section>
      </section>
    </section>
  );
}
