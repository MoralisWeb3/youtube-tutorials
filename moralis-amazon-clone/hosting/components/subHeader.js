import styles from "../styles/Home.module.css";

export default function SubHeader() {
  return (
    <section className={styles.subHeader}>
      <section>
        <section className={styles.allCategories}>
          <section className={styles.menuBars}>
            <svg
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 24 24"
              fill="currentColor"
              className="w-6 h-6"
            >
              <path
                fillRule="evenodd"
                d="M3 6.75A.75.75 0 013.75 6h16.5a.75.75 0 010 1.5H3.75A.75.75 0 013 6.75zM3 12a.75.75 0 01.75-.75h16.5a.75.75 0 010 1.5H3.75A.75.75 0 013 12zm0 5.25a.75.75 0 01.75-.75h16.5a.75.75 0 010 1.5H3.75a.75.75 0 01-.75-.75z"
                clipRule="evenodd"
              />
            </svg>
          </section>
          <p>All</p>
        </section>
        <p className={styles.subHeader_items}>Today's Deals</p>
        <p className={styles.subHeader_items}>Customer Service</p>
        <p className={styles.subHeader_items}>Registry</p>
        <p className={styles.subHeader_items}>Gift Cards</p>
        <p className={styles.subHeader_items}>Sell</p>
      </section>
    </section>
  );
}
