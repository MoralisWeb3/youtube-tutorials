import React from "react";
import axios from "axios";
import { Table } from "@web3uikit/core";
import { Reload } from "@web3uikit/icons";

function Tokens({ wallet, chain, tokens, setTokens }) {

  async function getTokenBalances() {
    const response = await axios.get("http://localhost:8080/tokenBalances", {
      params: {
        address: wallet,
        chain: chain,
      },
    });

    if (response.data) {
      tokenProcessing(response.data);
    }
  }

  function tokenProcessing(t) {

    
    for (let i = 0; i < t.length; i++) {
      t[i].bal = (Number(t[i].balance) / Number(`1E${t[i].decimals}`)).toFixed(3); //1E18
      t[i].val = ((Number(t[i].balance) / Number(`1E${t[i].decimals}`)) *Number(t[i].usd)).toFixed(2);
    }

    setTokens(t);

    
  }

  return (
    <>
      <div className="tabHeading">ERC20 Tokens <Reload onClick={getTokenBalances}/></div>

      {tokens.length > 0 && (
        <Table
          pageSize={6}
          noPagination={true}
          style={{ width: "900px" }}
          columnsConfig="300px 300px 250px"
          data={tokens.map((e) => [e.symbol, e.bal, `$${e.val}`] )}
          header={[
            <span>Currency</span>,
            <span>Balance</span>,
            <span>Value</span>,
          ]}
        />
      )}
    </>
  );
}

export default Tokens;
