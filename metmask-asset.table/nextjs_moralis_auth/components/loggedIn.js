import styles from "../styles/Home.module.css";

import TableHeader from "./tableHeader";
import TableContent from "./tableContent";

export default function LoggedIn() {
  return (
    <section className={styles.loggedIn_container}>
      <TableHeader />
      <TableContent />
    </section>
  );
}
