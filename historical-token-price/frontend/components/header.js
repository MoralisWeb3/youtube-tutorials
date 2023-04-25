import Image from "next/image";
import styles from "../styles/Home.module.css";

import Logo from "../public/assets/Moralis_logo.png";

export default function Header() {
  return (
    <section className={styles.header}>
      <Image src={Logo} alt="Logo image" width="50" height="40" />
      <h1 className={styles.title}>Historical Token Price</h1>
    </section>
  );
}
