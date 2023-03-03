import { useState } from "react";
import axios from "axios";
import styles from "@/styles/Home.module.css";
import { Bean, Beans } from "@web3uikit/icons";
import { Illustration } from "@web3uikit/core";

import SearchResults from "./searchResults.js";

export default function Search() {
  const [showResult, setShowResult] = useState(false);
  const [result, setResult] = useState([]);
  const [searchInput, setSearchInput] = useState("");

  const changeHandler = (e) => {
    setSearchInput(e.target.value);
  };

  const handleSearch = async () => {
    document.querySelector("#inputField").value = "";

    const response = await axios.get("http://localhost:5001/address", {
      params: { address: searchInput },
    });

    setResult(response.data.result);
    setShowResult(true);
  };

  return (
    <section className={styles.searchContainer}>
      <section className={styles.searchHeader}>
        <section className={styles.searchSection}>
          <h3>The Ethereum Blockchain Explorer</h3>
          <section className={styles.input_section}>
            <input
              className={styles.inputField}
              type="text"
              id="inputField"
              name="inputField"
              maxLength="120"
              placeholder="Search by Address / Txn Hash / Block / Token / Domain Name"
              required
              onChange={changeHandler}
            />
            <button className={styles.btn} onClick={handleSearch}>
              <svg
                xmlns="http://www.w3.org/2000/svg"
                viewBox="0 0 24 24"
                fill="currentColor"
                className={styles.magnifying}
              >
                <path
                  fillRule="evenodd"
                  d="M10.5 3.75a6.75 6.75 0 100 13.5 6.75 6.75 0 000-13.5zM2.25 10.5a8.25 8.25 0 1114.59 5.28l4.69 4.69a.75.75 0 11-1.06 1.06l-4.69-4.69A8.25 8.25 0 012.25 10.5z"
                  clipRule="evenodd"
                />
              </svg>
            </button>
          </section>
          <section className={styles.sponsored}>
            Sponsored:{" "}
            <span className={styles.bean}>
              <Bean fontSize="20px" />
            </span>{" "}
            500 Daily Moralis Beans for free!
            <span className={styles.claim}>Claim Them Now!</span>
          </section>
        </section>
        <section className={styles.adSection}>
          <p className={styles.adtext}>
            500 Free <br />
            Moralis Beans
          </p>
          <section>
            <Beans fontSize="50px" className={styles.float} />
            <Illustration logo="wizard" className={styles.wizard} />
          </section>
        </section>
      </section>
      {showResult && <SearchResults result={{ result, searchInput }} />}
    </section>
  );
}
