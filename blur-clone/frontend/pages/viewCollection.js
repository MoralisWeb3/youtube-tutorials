import styles from "@/styles/Home.module.css";

import CollectionHeader from "../components/collectionHeader.js";
import CollectionHeroSection from "../components/collectionHeroSection.js";
import CollectionPurchaseSection from "../components/collectionPurchaseSection.js";

export default function ViewCollection() {
  return (
    <section className={styles.container}>
      <section>
        <section className={styles.viewCollection_main}>
          <CollectionHeader />
          <CollectionHeroSection />
          <CollectionPurchaseSection />
        </section>
      </section>
    </section>
  );
}
