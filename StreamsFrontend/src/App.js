import "./App.css";
import { useState, useEffect } from "react";
import { initializeApp } from "firebase/app";
import {
  query,
  collection,
  onSnapshot,
  getFirestore,
} from "firebase/firestore";

const firebaseConfig = {
  get: "from firebase admin dashboard"
};

const app = initializeApp(firebaseConfig);
const db = getFirestore(app);

function App() {
  const [txs, setTxs] = useState(null);

  useEffect(() => {
    console.log("start");
    const q = query(collection(db, "moralis/txs/Polygontestnet"));
    console.log("query has been setup");
    const unsubscribe = onSnapshot(q, (querySnapshot) => {
      const tempTxs = [];
      querySnapshot.forEach((doc) => {
        tempTxs.push(doc.data());
      });
      setTxs(tempTxs);
    });


    //Stop realtime updates:
    //unsubscribe();
  }, []);

  return (
    <div className="App">
      <header className="App-header">
        <p>🔥 Firebase Moralis Streams Extension 🔥</p>
            <table className="table">
              <tr>
                <th>From</th>
                <th>To</th>
                <th>Amount</th>
              </tr>
              {txs?.map((e,i)=>{
                return(
                  <tr key={i}>
                    <td>{e.fromAddress}</td>
                    <td>{e.toAddress}</td>
                    <td>{e.value / 1E18} Matic</td>
                  </tr>
                )
              })}
            </table>
      </header>
    </div>
  );
}
export default App;
