import "@/styles/globals.css";
import { useState } from "react";
import TxnContext from "../components/txnContext.js";

export default function App({ Component, pageProps }) {
  const [hash, setHash] = useState("");
  const [version, setVersion] = useState("");
  const [txnStatus, setTxnStatus] = useState("");
  const [sender, setSender] = useState("");
  const [txnFunction, setTxnFunction] = useState("");
  const [amount, setAmount] = useState(0);

  return (
    <TxnContext.Provider
      value={{
        hash,
        setHash,
        version,
        setVersion,
        txnStatus,
        setTxnStatus,
        sender,
        setSender,
        txnFunction,
        setTxnFunction,
        amount,
        setAmount,
      }}
    >
      <Component {...pageProps} />
    </TxnContext.Provider>
  );
}
