import { useMoralis } from "react-moralis";
import styles from "../styles/Home.module.css";
import SignIn from "../components/SignIn";
import { SignOut } from "../components/SignOut";
export default function Home() {
  const { isAuthenticated } = useMoralis();

  return (
    <div>
      <div className={styles.backgroundParent}>
        {isAuthenticated ? <SignOut /> : <SignIn />}
      </div>
    </div>
  );
}
