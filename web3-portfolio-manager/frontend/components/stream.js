import { useEffect, useState } from "react";
import axios from "axios";
import styles from "../styles/Home.module.css";
import { useAccount } from "wagmi";

import CreateStream from "./createStream";

export default function Stream() {
  const { address } = useAccount();
  const [streamExist, setStreammExist] = useState(false);

  useEffect(() => {
    async function getStreams() {
      const response = await axios.get("http://localhost:5001/getstreams");

      response.data.result.map((stream) => {
        if (stream.tag === address) {
          setStreammExist(true);
        }
      });
    }
    getStreams();
  });
  return (
    <section className={styles.result_streams}>
      <section className={styles.streamContainer}>
        {streamExist ? (
          <section>
            <iframe
              width="560"
              height="315"
              src="https://www.youtube.com/embed/WeOZnvFulNA"
              title="YouTube video player"
              frameborder="0"
              allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share"
              allowfullscreen
            ></iframe>
          </section>
        ) : (
          <section className={styles.title}>
            If you see this message it means this wallet didn't have a
            previously setup of streams. <br /> <br />
            But now you do :)
            <CreateStream />
          </section>
        )}
      </section>
    </section>
  );
}
