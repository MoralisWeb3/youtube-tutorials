import React from 'react'
import axios from "axios";
import { useState, useEffect } from "react";

function Nfts({chain, wallet, filteredNfts, setFilteredNfts, nfts, setNfts}) {
  const [nameFilter, setNameFilter] = useState("");
  const [idFilter, setIdFilter] = useState("");


  async function getUserNfts() {
    const response = await axios.get("http://localhost:8080/nftBalance", {
      params: {
        address: wallet,
        chain: chain,
      },
    });

    if (response.data.result) {
      nftProcessing(response.data.result);
    }
  }

  function nftProcessing(t) {
    for (let i = 0; i < t.length; i++) {
      let meta = JSON.parse(t[i].metadata);
      if (meta && meta.image) {
        if (meta.image.includes(".")) {
          t[i].image = meta.image;
        } else {
          t[i].image = "https://ipfs.moralis.io:2053/ipfs/" + meta.image;
        }
      }
    }
    setNfts(t);
    setFilteredNfts(t);
  }


  useEffect(() => {
    if (idFilter.length === 0 && nameFilter.length === 0) {
      return setFilteredNfts(nfts);
    }

    let filNfts = [];

    for (let i = 0; i < nfts.length; i++) {
      if (
        nfts[i].name.toLowerCase().includes(nameFilter) &&
        idFilter.length === 0
      ) {
        filNfts.push(nfts[i]);
      } else if (
        nfts[i].token_id.includes(idFilter) &&
        nameFilter.length === 0
      ) {
        filNfts.push(nfts[i]);
      } else if (
        nfts[i].token_id.includes(idFilter) &&
        nfts[i].name.toLowerCase().includes(nameFilter)
      ) {
        filNfts.push(nfts[i]);
      }
    }

    setFilteredNfts(filNfts);
  }, [nameFilter, idFilter]);

  return (
    <><h1>Portfolio NFTs</h1>
    <div>
      <button onClick={getUserNfts}>Fetch NFTs</button>
      <p>
        <span> Name Filter </span>
        <input
          onChange={(e) => setNameFilter(e.target.value)}
          value={nameFilter}
        ></input>
      </p>
      <span> Id Filter </span>
      <input
        onChange={(e) => setIdFilter(e.target.value)}
        value={idFilter}
      ></input>
      <br />
      {filteredNfts.length > 0 &&
        filteredNfts.map((e) => {
          return (
            <>
              {e.image && <img src={e.image} width={200} />}
              <span>Name: {e.name}, </span>
              <span>(ID: {e.token_id})</span>
              <br />
            </>
          );
        })}
    </div></>
  )
}

export default Nfts