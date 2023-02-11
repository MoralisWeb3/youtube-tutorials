import Link from "next/link";
import Image from "next/image";
import { useState } from "react";
import styles from "../styles/Home.module.css";

import { signInWithMoralis } from "@moralisweb3/client-firebase-evm-auth";
import { useFirebase } from "../components/FirebaseInitializer";

import Logo from "../public/assets/amazon_logo.png";
import Usa from "../public/assets/usa.png";

export default function Header() {
  const { auth, moralisAuth } = useFirebase();
  const [currentUser, setCurrentUser] = useState(
    () => auth.currentUser?.displayName
  );

  async function signInByMetaMask() {
    await signInWithMoralis(moralisAuth);
    setCurrentUser(auth.currentUser?.displayName);
  }

  async function signOut() {
    await auth.signOut();
    setCurrentUser(null);
  }

  return (
    <section className={styles.header}>
      <section className={styles.header_box}>
        <Link href="/">
          <Image src={Logo} alt="Logo image" width="110" height="30" />
        </Link>
        <section className={styles.searchContainer}>
          <input className={styles.searchField} type="text" />
          <section className={styles.searchIcon}>
            <svg
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 24 24"
              fill="currentColor"
              className="w-6 h-6"
            >
              <path
                fillRule="evenodd"
                d="M10.5 3.75a6.75 6.75 0 100 13.5 6.75 6.75 0 000-13.5zM2.25 10.5a8.25 8.25 0 1114.59 5.28l4.69 4.69a.75.75 0 11-1.06 1.06l-4.69-4.69A8.25 8.25 0 012.25 10.5z"
                clipRule="evenodd"
              />
            </svg>
          </section>
        </section>
        <section className={styles.headerInfo}>
          <section className={`${styles.headerInfo_items} ${styles.language}`}>
            <Image src={Usa} alt="Logo image" width="25" height="15" />
            <span>EN</span>
            <span className={styles.arrow}></span>
          </section>
          {!currentUser ? (
            <button
              className={`${styles.headerInfo_items} ${styles.headerInfo_btn}`}
              onClick={signInByMetaMask}
            >
              Login
            </button>
          ) : (
            <button
              className={`${styles.headerInfo_items} ${styles.headerInfo_btn}`}
              onClick={signOut}
            >
              Sign out
            </button>
          )}
          <section className={styles.headerInfo_items}>
            <p>Returns</p>
            <p>& Orders</p>
          </section>
          <section className={`${styles.headerInfo_items} ${styles.cartItem}`}>
            <span className={styles.cartCount}>0</span>
            <section className={styles.cartIcon}>
              <svg
                xmlns="http://www.w3.org/2000/svg"
                viewBox="0 0 24 24"
                fill="currentColor"
                className="w-6 h-6"
              >
                <path d="M2.25 2.25a.75.75 0 000 1.5h1.386c.17 0 .318.114.362.278l2.558 9.592a3.752 3.752 0 00-2.806 3.63c0 .414.336.75.75.75h15.75a.75.75 0 000-1.5H5.378A2.25 2.25 0 017.5 15h11.218a.75.75 0 00.674-.421 60.358 60.358 0 002.96-7.228.75.75 0 00-.525-.965A60.864 60.864 0 005.68 4.509l-.232-.867A1.875 1.875 0 003.636 2.25H2.25zM3.75 20.25a1.5 1.5 0 113 0 1.5 1.5 0 01-3 0zM16.5 20.25a1.5 1.5 0 113 0 1.5 1.5 0 01-3 0z" />
              </svg>
            </section>
            <p>Cart</p>
          </section>
        </section>
      </section>
    </section>
  );
}
