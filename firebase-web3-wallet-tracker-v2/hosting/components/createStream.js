import axios from "axios";
import { useEffect, useState } from "react";
import { useFirebase } from "./FirebaseInitializer";

export default function CreateStream() {
  const { auth } = useFirebase();
  const [address, setAddress] = useState(() => auth.currentUser?.displayName);

  useEffect(() => {
    async function getData() {
      const response = await axios.get(`http://localhost:3001/createstream`, {
        params: { address },
      });
    }
    getData();
  }, []);
}
