import Head from "next/head";
import styles from "/styles/Home.module.css";

import Header from "../components/header";
import SubHeader from "../components/subHeader";
import Main from "../components/main";

export default function Home() {
  return (
    <section className={styles.container}>
      <Head>
        <title>Web3 Amazon Clone</title>
      </Head>
      <Header />
      <SubHeader />
      <Main />
    </section>
  );
}
