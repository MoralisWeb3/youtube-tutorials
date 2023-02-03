import { useState } from "react";
import axios from "axios";
import styles from "../styles/Home.module.css";

export default function LoggedIn() {
  const [message, setMessage] = useState("");

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

    const responseP = document.createElement("p");
    responseP.innerText = `AI: ${response.data.choices[0].text}`;

    chatHistory.appendChild(responseP);
    console.log(response.data.choices[0].text);
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
    </section>
  );
}
