import { getSession, signOut } from "next-auth/react";
import LoggedIn from "../components/loggedIn";
import styles from "../styles/Home.module.css";

function User({ user }) {
  return (
    <section className={styles.main}>
      <section className={styles.header}>
        <section className={styles.header_section}>
          <h1>MetaMask Portfolio</h1>
          <button
            className={styles.connect_btn}
            onClick={() => signOut({ redirect: "/" })}
          >
            Sign out
          </button>
        </section>
        <LoggedIn />
      </section>
    </section>
  );
}

export async function getServerSideProps(context) {
  const session = await getSession(context);

  if (!session) {
    return {
      redirect: {
        destination: "/",
        permanent: false,
      },
    };
  }

  return {
    props: { user: session.user },
  };
}

export default User;
