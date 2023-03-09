import { useState } from "react";
import axios from "axios";
import Image from "next/image";
import { Card, Illustration } from "@web3uikit/core";
import {
  Accordion,
  AccordionSummary,
  AccordionDetails,
  Typography,
} from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import styles from "@/styles/Home.module.css";

import MoralisLogo from "../public/assets/moralis.png";
import AptosLogo from "../public/assets/aptos_white.png";

export default function Main() {
  const [walletAddress, setWalletAddress] = useState("");
  const [result, setResult] = useState([]);
  const [showResult, setShowResult] = useState(false);

  const handleChange = (e) => {
    setWalletAddress(e.target.value);
  };

  const handleSubmit = async () => {
    document.querySelector("#inputField").value = "";

    const response = await axios.get(
      `http://localhost:5001/getaccounttransactions`,
      {
        params: { address: walletAddress },
      }
    );

    setResult(response.data);
    setShowResult(true);
  };

  return (
    <section className={styles.container}>
      <section className={styles.header}>
        <section className={styles.logo_section}>
          <Image src={MoralisLogo} alt="Logo image" width="102" height="82" />
          <svg
            xmlns="http://www.w3.org/2000/svg"
            fill="none"
            viewBox="0 0 24 24"
            strokeWidth="1.5"
            stroke="currentColor"
            className={styles.mxa}
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              d="M6 18L18 6M6 6l12 12"
            />
          </svg>
          <Image src={AptosLogo} alt="Logo image" width="82" height="82" />
        </section>
        <section className={styles.input_section}>
          <input
            className={styles.inputField}
            type="text"
            id="inputField"
            name="inputField"
            maxLength="120"
            required
            onChange={handleChange}
          />
          <button className={styles.form_btn} onClick={handleSubmit}>
            Submit
          </button>
        </section>
      </section>
      <section className={styles.transactionContainer}>
        {showResult &&
          result.map((transaction) => {
            return (
              <section
                className={styles.transactionRow}
                key={transaction.version}
              >
                <section className={styles.transactionSection}>
                  <Card
                    onClick={function noRefCheck() {}}
                    setIsSelected={function noRefCheck() {}}
                    description={`Sender: ${transaction.sender.slice(
                      0,
                      10
                    )}...${transaction.sender.slice(26, 36)}`}
                    className={styles.card}
                  >
                    <Illustration height="80px" logo="wallet" width="100%" />
                  </Card>
                  <section className={styles.transactionSection_right}>
                    <section>
                      <p>
                        Hash:{" "}
                        <a
                          href={`https://explorer.aptoslabs.com/txn/${transaction.hash}`}
                          target="_blank"
                        >
                          {transaction.hash.slice(0, 10)}...
                          {transaction.hash.slice(26, 36)}
                        </a>
                      </p>
                      <p>Gas Used: {transaction.gas_used / 10 ** 8}</p>
                      <p>
                        Time:{" "}
                        {
                          new Date(transaction.timestamp / 1000)
                            .toISOString()
                            .split("T")[0]
                        }
                      </p>
                      <p>Status: {transaction.vm_status}</p>
                    </section>
                  </section>
                </section>
                <Accordion className={styles.accordion}>
                  <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                    <Typography sx={{ color: "black", fontWeight: "bold" }}>
                      Events
                    </Typography>
                  </AccordionSummary>
                  {transaction.events.map((e, i) => {
                    return (
                      <AccordionDetails key={i}>
                        {e.type
                          .split("::")
                          .slice(-1)[0]
                          .replace(/([A-Z])/g, " $1")}
                      </AccordionDetails>
                    );
                  })}
                </Accordion>
              </section>
            );
          })}
      </section>
    </section>
  );
}
