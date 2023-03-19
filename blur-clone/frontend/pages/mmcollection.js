import styles from "@/styles/Home.module.css";

import CollectionHeader from "../components/collectionHeader.js";
import MMCollectionHeroSection from "../components/mmCollectionHeroSection.js";
import MMCollectionPurchaseSection from "../components/mmCollectionPurchaseSection.js";

export default function ViewCollection() {
  return (
    <section className={styles.container}>
      <section>
        <section className={styles.viewCollection_main}>
          <CollectionHeader />
          <MMCollectionHeroSection />
          <MMCollectionPurchaseSection />
        </section>
      </section>
    </section>
  );
}
