import { useState } from "react";
import axios from "axios";
import Select from "react-select";
import styles from "../styles/Home.module.css";

export default function Main() {
  const [showResult, setShowResult] = useState(false);
  const [result, setResult] = useState([]);
  const [chainValue, setChainValue] = useState("");

  const valueOptions = [
    { value: "0xa4b1", label: "Arbitrum" },
    { value: "0x38", label: "Binance" },
    { value: "0x1", label: "Ethereum" },
    { value: "0x5", label: "Goerli" },
    { value: "0x13881", label: "Mumbai" },
  ];

  const customStyles = {
    option: (provided) => ({
      ...provided,
      color: "#000000",
      backgroundColor: "#ffffff",
      borderColor: "#00c3f8",
      width: "80",
    }),
  };

  const changeHandler = (chainValue) => {
    setChainValue(chainValue);
  };

  const handleSubmit = async () => {
    const chain = chainValue.value;

    const response = await axios.get(`http://localhost:5001/getmints`, {
      params: { chain },
    });

    console.log(response);
    setResult(response.data);
    setShowResult(true);
    setChainValue("");
  };

  return (
    <section className={styles.main}>
      <section className={styles.main_form}>
        <Select
          styles={(customStyles, { borderColor: "#00c3f8" })}
          options={valueOptions}
          value={chainValue}
          instanceId="long-value-select"
          onChange={changeHandler}
          className={styles.dropdown}
        />
        <button className={styles.submit_btn} onClick={handleSubmit}>
          Submit
        </button>
      </section>
      <section className={styles.resultContainer}>
        {showResult &&
          result.map((mint, i) => {
            return (
              <section className={styles.resultSection} key={i}>
                <section className={styles.card}>
                  <section className={styles.card_details}>
                    <section>
                      <p className={styles.card_details__key}>Time: </p>
                      <p className={styles.card_details__key}>To Wallet: </p>
                      <p className={styles.card_details__key}>Amount:</p>
                      <p className={styles.card_details__key}>
                        Contract Address:
                      </p>
                      <p className={styles.card_details__key}>
                        Transaction Hash:
                      </p>
                    </section>
                    <section>
                      <h4 className={styles.card_details__value}>
                        {
                          mint.finalResponse.response.blockTimestamp.split(
                            "T"
                          )[0]
                        }
                      </h4>
                      <p className={styles.card_details__value}>
                        {mint.finalResponse.response.toWallet.slice(0, 4)}...
                        {mint.finalResponse.response.toWallet.slice(4, 8)}
                      </p>
                      <p className={styles.card_details__value}>
                        {(
                          mint.finalResponse.response.value /
                          10 ** mint.finalResponse.tokenData.decimals
                        ).toLocaleString("sv-SE")}{" "}
                        {mint.finalResponse.tokenData.symbol}
                      </p>
                      <p className={styles.card_details__value}>
                        {mint.finalResponse.response.contractAddress.slice(
                          0,
                          4
                        )}
                        ...
                        {mint.finalResponse.response.contractAddress.slice(
                          4,
                          8
                        )}
                      </p>
                      <p className={styles.card_details__value}>
                        {mint.finalResponse.response.transactionHash.slice(
                          0,
                          8
                        )}
                        ...
                        {mint.finalResponse.response.transactionHash.slice(
                          8,
                          16
                        )}
                      </p>
                    </section>
                  </section>
                </section>
              </section>
            );
          })}
      </section>
    </section>
  );
}
