import Head from "next/head";
import styles from "/styles/Home.module.css";

import Header from "../components/header";
import SubHeader from "../components/subHeader";
import ProductComp from "../components/productComp";

export default function Product() {
  return (
    <section className={styles.container}>
      <Head>
        <title>Web3 Amazon Clone</title>
      </Head>
      <main>
        <Header />
        <SubHeader />
        <ProductComp />
      </main>
    </section>
  );
}
