import axios from "axios";
import { useEffect, useState } from "react";
import { useAccount } from "wagmi";
import Card from "./card.js";

export default function GetWalletTokens() {
  const [tokens, setTokens] = useState([]);
  const { address } = useAccount();

  useEffect(() => {
    let response;
    async function getData() {
      response = await axios
        .get(`http://localhost:5001/gettokens`, {
          params: { address },
        })
        .then((response) => {
          console.log(response.data);
          setTokens(response.data);
        });
    }
    getData();
  }, []);

  return (
    <section>
      {tokens.map((token) => {
        return (
          token.usdPrice && (
            <Card
              token={token}
              total={tokens[3]}
              key={token.walletBalance?.symbol}
            />
          )
        );
      })}
    </section>
  );
}
