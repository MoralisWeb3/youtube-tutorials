import axios from "axios";
import { useState } from "react";
import { Card } from "antd";
import styles from "./styles/Home.module.css";
import Logo from "./img/Moralis_logo.png";
import "./App.css";
import dayjs from "dayjs";
import Stack from "@mui/material/Stack";
import TextField from "@mui/material/TextField";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { DateTimePicker } from "@mui/x-date-pickers/DateTimePicker";

function App() {
  const [blockByDate, setBlockByDate] = useState(null);
  const [date, setDate] = useState("");
  const [value, setValue] = useState(dayjs("2021-08-18T21:11:54"));
  const [params, setParams] = useState({
    chain: "eth",
  });

  const refreshBlockByDate = async () => {
    await axios
      .get(`/get_block?chain=${params.chain}&date=${date}`)
      .then((res) => {
        setBlockByDate(res.data);
      })
      .catch((err) => console.log(err));
  };

  const handleParamsChange = (e) => {
    setParams({ ...params, [e.target.name]: e.target.value });
  };
  const handleChange = (newValue) => {
    setValue(newValue);
    const newDate = Math.floor(new Date(newValue).getTime() / 1000);
    setDate(newDate);
  };

  const renderedBlock = blockByDate && (
    <Card
      hoverable
      className="result-card"
      title={`Date: ${new Date(blockByDate.date).toDateString()}`}
      extra={
        <p>
          <b>Block Number:</b> {blockByDate.block}
        </p>
      }
      style={{ width: 300 }}
    >
      <div className="result-element">
        <p className="result-begin">Hash: </p>
        <p className="result-end">{blockByDate.hash}</p>
      </div>
      <div className="result-element">
        <p className="result-begin">Block Time Stamp:</p>
        <p className="result-end">{blockByDate.block_timestamp}</p>
      </div>
    </Card>
  );

  return (
    <div>
      <div className={styles.header}>
        <div className={styles.moralis_logo}>
          <img src={Logo} alt="Logo image" width="102" height="82" />
        </div>
        <h1 className={styles.title}>Get any Block by Date </h1>
      </div>
      <section className={styles.main}>
        <div className={styles.getTokenForm}>
          <label
            className={styles.label}
            value={params.chain}
            onChange={handleParamsChange}
          >
            Date
          </label>
          <LocalizationProvider
            dateAdapter={AdapterDayjs}
            className={styles.walletAddress}
          >
            <Stack spacing={3} className={styles.datePicker}>
              <DateTimePicker
                value={value}
                onChange={handleChange}
                renderInput={(params) => <TextField {...params} />}
              />
            </Stack>
          </LocalizationProvider>
        </div>

        <div className={styles.getTokenForm}>
          <label
            className={styles.label}
            value={params.chain}
            onChange={handleParamsChange}
          >
            Chain
          </label>
          <select className={styles.walletAddress} name="chain">
            <option value="eth">Ethereum</option>
            <option value="goerli">Goerli</option>
            <option value="polygon">Polygon</option>
            <option value="mumbai">Mumbai</option>
            <option value="bsc">Binance</option>
          </select>
        </div>

        <button className={styles.form_btn} onClick={refreshBlockByDate}>
          Get Block
        </button>
        <div className="results">
          {blockByDate && <div className="nfts">{renderedBlock}</div>}
        </div>
      </section>
    </div>
  );
}

export default App;
