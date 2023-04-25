import { useEffect, useState } from "react";
import axios from "axios";
import Select from "react-select";
import moment from "moment";
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
} from "recharts";
import styles from "../styles/Home.module.css";

export default function Main() {
  const [showResult, setShowResult] = useState(false);
  const [result, setResult] = useState([]);
  const [latestBlockNumbers, setLatestBlockNumbers] = useState([]);
  const [addressValue, setAddressValue] = useState("");

  const valueOptions = [
    { value: "0x111111111117dc0aa78b770fa6a738034120c302", label: "1INCH" },
    { value: "0x7Fc66500c84A76Ad7e9c93437bFc5Ac33E2DDaE9", label: "AAVE" },
    { value: "0x5B7533812759B45C2B44C19e320ba2cD2681b542", label: "AGIX" },
    { value: "0x4Fabb145d64652a948d72533023f6E7A623C7C53", label: "BUSD" },
    { value: "0xc02aaa39b223fe8d0a0e5c4f27ead9083c756cc2", label: "ETH" },
    { value: "0x5a98fcbea516cf06857215779fd812ca3bef1b32", label: "LDO" },
    { value: "0x514910771af9ca656af840dff83e8264ecf986ca", label: "LINK" },
    { value: "0x7D1AfA7B718fb893dB30A3aBc0Cfc608AaCfeBB0", label: "MATIC" },
    { value: "0x95ad61b0a150d79219dcf64e1e6cc01f0b64c4ce", label: "SHIB" },
    { value: "0x1f9840a85d5af5bf1d1762f925bdaddc4201f984", label: "UNI" },
    { value: "0xa0b86991c6218b36c1d19d4a2e9eb0ce3606eb48", label: "USDC" },
    { value: "0xdac17f958d2ee523a2206206994597c13d831ec7", label: "USDT" },
    { value: "0xD13cfD3133239a3c73a9E535A5c4DadEE36b395c", label: "VAI" },
  ];

  useEffect(() => {
    let last7Days = [];
    for (let i = 0; i < 7; i++) {
      const day = moment().subtract(i, "days").format("x");
      last7Days.push(day);
    }

    async function getBlockNumber() {
      await axios
        .get("http://localhost:5001/getdateblock", {
          params: { dates: last7Days },
        })
        .then((response) => {
          setLatestBlockNumbers(response.data.blockNumbers);
        });
    }

    getBlockNumber();
  }, []);

  const changeHandler = async (addressValue) => {
    setAddressValue(addressValue);
    await axios
      .get("http://localhost:5001/gettokenprice", {
        params: {
          address: addressValue.value,
          blockNumbers: latestBlockNumbers,
        },
      })
      .then((response) => {
        setResult(response.data.tokenPrices);
        setShowResult(true);
      });
  };

  return (
    <section className={styles.main}>
      <section className={styles.dropdownSection}>
        <Select
          options={valueOptions}
          value={addressValue}
          instanceId="long-value-select"
          className={styles.dropdown}
          onChange={changeHandler}
        />
      </section>
      <section>
        {showResult && (
          <section>
            <LineChart
              width={800}
              height={500}
              data={[
                {
                  name: moment().subtract(6, "days").format("YYYY-MM-DD"),
                  price: result[6],
                },
                {
                  name: moment().subtract(5, "days").format("YYYY-MM-DD"),
                  price: result[5],
                },
                {
                  name: moment().subtract(4, "days").format("YYYY-MM-DD"),
                  price: result[4],
                },
                {
                  name: moment().subtract(3, "days").format("YYYY-MM-DD"),
                  price: result[3],
                },
                {
                  name: moment().subtract(2, "days").format("YYYY-MM-DD"),
                  price: result[2],
                },
                {
                  name: moment().subtract(1, "days").format("YYYY-MM-DD"),
                  price: result[1],
                },
                {
                  name: moment().subtract(0, "days").format("YYYY-MM-DD"),
                  price: result[0],
                },
              ]}
            >
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="name" />
              <YAxis />
              <Tooltip />
              <Line
                type="monotone"
                dataKey="price"
                stroke="#8884d8"
                activeDot={{ r: 8 }}
              />
            </LineChart>
          </section>
        )}
      </section>
    </section>
  );
}
