import Image from "next/image";
import styles from "../styles/Home.module.css";
import { useAccount, useDisconnect } from "wagmi";
import { useEffect, useState } from "react";

import Logo from "../public/assets/Moralis_logo.png";

export default function Header() {
  const { address, isConnected } = useAccount();
  const { disconnect } = useDisconnect();
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
      <Image src={Logo} alt="Logo image" width="102" height="82" />
      {isLoggedIn && (
        <section className={styles.headerConnectBtn}>
          <section>
            {isConnected
              ? `${address.slice(0, 4)}...${address.slice(38)}`
              : address}
          </section>
          <button onClick={disconnect}>Disconnect</button>
        </section>
      )}
    </section>
  );
}
