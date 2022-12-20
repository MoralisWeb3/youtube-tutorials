import Image from "next/image";
import styles from "../styles/Home.module.css";
import { ConnectButton } from "@rainbow-me/rainbowkit";
import { useAccount } from "wagmi";
import { useEffect, useState } from "react";

import Logo from "../public/assets/Moralis_logo.png";

export default function Header() {
  const { isConnected } = useAccount();
  const [isLoggedIn, logIn] = useState(false);

  useEffect(() => {
    if (isConnected) {
      logIn(true);
    } else {
      logIn(false);
    }
  }, [isConnected]);
  return (
    <section className={styles.header}>
      <Image src={Logo} alt="Logo image" width="102" height="82" />
      {isLoggedIn && (
        <ul className={styles.headerConnectBtn}>
          <li>
            <ConnectButton />
          </li>
        </ul>
      )}
    </section>
  );
}
