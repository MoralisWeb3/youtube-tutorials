import { useEffect } from "react";
import axios from "axios";
import { useAccount } from "wagmi";

export default function CreateStream() {
  const { address } = useAccount();

  useEffect(() => {
    async function getData() {
      const response = await axios.get(`http://localhost:5001/createstream`, {
        params: { address },
      });
    }
    getData();
  }, []);
}
