import { useState } from "react";
import axios from "axios";
import styles from "../styles/Home.module.css";
import { abi, MSG_CONTRACT_ADDRESS } from "../constants/index";

export default function LoggedIn() {
  const [message, setMessage] = useState("");
  const [response, setResponse] = useState("");
  const [pair, setPair] = useState({});
  const [showMintBtn, setShowMintBtn] = useState(false);

  const getMessage = (e) => {
    setMessage(e.target.value);
  };

  const sendMessage = async () => {
    document.querySelector("#inputField").value = "";
    const chatHistory = document.querySelector("#chatHistory");

    const messageP = document.createElement("p");
    messageP.innerText = `You: ${message}`;

    chatHistory.appendChild(messageP);
    const response = await axios.get("http://localhost:5001/", {
      params: { message },
    });

    setResponse(`${response.data.choices[0].text.replaceAll("\n", "")}`);
    setPair({
      message: `You: ${message}`,
      response: `AI: ${response.data.choices[0].text.replaceAll("\n", "")}`,
    });
    const responseP = document.createElement("p");
    responseP.innerText = `AI: ${response.data.choices[0].text.replaceAll(
      "\n",
      ""
    )}`;
    chatHistory.appendChild(responseP);
    setShowMintBtn(true);
  };

  const upload = async () => {
    console.log("Mint", pair);
    const response = await axios.get("http://localhost:5001/uploadtoipfs", {
      params: { pair },
    });
  };

  return (
    <section>
      <section className={styles.chat_box}>
        <section className={styles.chat_history} id="chatHistory"></section>
        <section className={styles.message_input}>
          <input
            type="text"
            id="inputField"
            placeholder="Type your message..."
            onChange={getMessage}
          />
          <button onClick={sendMessage}>Send</button>
        </section>
      </section>
      {showMintBtn && (
        <button className={styles.mint_btn} onClick={upload}>
          UPLOAD TO IPFS
        </button>
      )}
    </section>
  );
}
