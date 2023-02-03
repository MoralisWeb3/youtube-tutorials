import { getSession, signOut } from "next-auth/react";
import LoggedIn from "../components/loggedIn";
import styles from "../styles/Home.module.css";

function User({ user }) {
  return (
    <section className={styles.header}>
      <section className={styles.header_section}>
        <h1>Chat with Chat GPT</h1>
        <button
          className={styles.connect_btn}
          onClick={() => signOut({ redirect: "/signin" })}
        >
          Sign out
        </button>
      </section>
      <LoggedIn />
    </section>
  );
}

export async function getServerSideProps(context) {
  const session = await getSession(context);

  if (!session) {
    return {
      redirect: {
        destination: "/signin",
        permanent: false,
      },
    };
  }

  return {
    props: { user: session.user },
  };
}

export default User;
