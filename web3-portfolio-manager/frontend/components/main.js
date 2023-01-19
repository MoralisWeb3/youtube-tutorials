import { useEffect, useState } from "react";
import Image from "next/image";
import styles from "../styles/Home.module.css";
import { ConnectButton } from "@rainbow-me/rainbowkit";
import { useAccount } from "wagmi";

import LoggedIn from "../components/loggedIn";
import QuestionMark from "../public/assets/question.png";

export default function Header() {
  const { isConnected } = useAccount();
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [showModal, setShowModal] = useState(false);

  useEffect(() => {
    if (!isConnected) {
      setIsLoggedIn(true);
    } else {
      setIsLoggedIn(false);
    }
  }, [isConnected]);

  function modal() {
    setShowModal(!showModal);
  }

  return (
    <section className={styles.main_container}>
      <section className={styles.main_helper}>
        <a onClick={modal}>
          <Image
            src={QuestionMark}
            alt="Question Mark image"
            width="35"
            height="35"
            className={styles.questionMark}
            id="questionMark"
          />
        </a>
      </section>
      {isLoggedIn ? (
        <section className={styles.ConnectButton_section}>
          <h1 className={styles.title}>Portfolio Manager</h1>
          <ConnectButton />
        </section>
      ) : (
        <LoggedIn />
      )}
      <section
        className={`${styles.popup} ${showModal ? styles.show : styles.hide}`}
      >
        <h1 className={styles.title}>HELP</h1>
        <section>
          <p>If the user doesn't connect their wallet no data will be shown.</p>
          <p>
            In the <b>TOKEN</b> tab you see all the tokens owned by this wallet
          </p>
          <p>
            In the <b>NFT</b> tab you see all the NFTs owned by this wallet
          </p>
          <p>
            In the <b>TRANSACTION</b> tab you see all the transactions made by
            this wallet
          </p>
          <p>
            In the <b>STREAM</b> tab you either setup a new stream for every
            transaction of this wallet, or you get a video recommendation
          </p>
        </section>
      </section>
    </section>
  );
}
