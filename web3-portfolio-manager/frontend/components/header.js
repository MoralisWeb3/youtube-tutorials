import { useState, useEffect } from "react";
import Image from "next/image";
import Link from "next/link";
import styles from "../styles/Home.module.css";
import { ConnectButton } from "@rainbow-me/rainbowkit";
import { useAccount } from "wagmi";

import Logo from "../public/assets/Moralis_logo.png";

export default function Header() {
  const { isConnected } = useAccount();
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  useEffect(() => {
    if (isConnected) {
      setIsLoggedIn(true);
    } else {
      setIsLoggedIn(false);
    }
  }, [isConnected]);

  return (
    <section className={styles.header}>
      <section className={styles.header_box}>
        <Image
          src={Logo}
          alt="Logo image"
          width="102"
          height="82"
          className={styles.float}
        />
        <section className={styles.nav}>
          <ul>
            <li className={styles.nav_li}>
              <Link href="/">Home</Link>
            </li>
            <li className={styles.nav_li}>
              <Link href="/token">Token</Link>
            </li>
            <li className={styles.nav_li}>
              <Link href="/nft">Nft</Link>
            </li>
            <li className={styles.nav_li}>
              <Link href="/transaction">transaction</Link>
            </li>
            <li className={styles.nav_li}>
              <Link href="/stream">stream</Link>
            </li>
          </ul>
          {isLoggedIn && (
            <section className={styles.header_connect}>
              <ConnectButton showBalance={false} accountStatus={"avatar"} />
            </section>
          )}
        </section>
      </section>
    </section>
  );
}
