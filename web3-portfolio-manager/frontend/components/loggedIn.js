import styles from "../styles/Home.module.css";
import Image from "next/image";

import Mindmap from "../public/assets/dApp_mindmap.png";

export default function LoggedIn() {
  return (
    <section className={styles.loggedIn_container}>
      <h1 className={styles.title}>Getting Started with Web3 2023</h1>
      <Image src={Mindmap} alt="MindMap image" width="900" height="550" />
    </section>
  );
}
