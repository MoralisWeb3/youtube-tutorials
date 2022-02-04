import Head from "next/head";
import Image from "next/image";
import { useMoralis, useWeb3Transfer } from "react-moralis";
import styles from "../styles/Home.module.css";
import Logo from "../Assets/MoralisLogo.png";
import { useEffect, useState } from "react";
import SignIn from "../Components/SignIn";
import { SignOut } from "../Components/SignOut";

export default function Home() {
  const { authenticate, isAuthenticated, logout, authError, isAuthenticating } =
    useMoralis();

  const [email, setEmail] = useState("rayyan5926439@gmail.com");

  const handleCustomLogin = async () => {
    console.log("This is SIckkkkkk");
    await authenticate({
      provider: "magicLink",
      email: email,
      apiKey: "pk_test_7967AF810E630E08",
      network: "kovan",
    });
  };

  return (
    <div className={styles.container}>
      <div className={styles.backgroundParent}>
        {isAuthenticated ? <SignOut /> : <SignIn />}
      </div>
    </div>
  );
}
