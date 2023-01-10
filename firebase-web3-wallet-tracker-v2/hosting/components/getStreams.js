import axios from "axios";
import { useEffect, useState } from "react";
import { useFirebase } from "./FirebaseInitializer";

import GetTransactions from "./getTransactions";
import ListenToStream from "./listenToStream";

export default function GetStreams() {
  const { auth } = useFirebase();
  const [address, setAddress] = useState(() => auth.currentUser?.displayName);
  const [streamExist, setStreammExist] = useState(false);

  useEffect(() => {
    async function getStreams() {
      const response = await axios.get("http://localhost:3001/getstreams");

      response.data.result.map((stream) => {
        if (stream.tag === address) {
          setStreammExist(true);
        }
      });
    }
    getStreams();
  });

  return (
    <section>{streamExist ? <ListenToStream /> : <GetTransactions />}</section>
  );
}
