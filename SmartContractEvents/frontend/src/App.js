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
  get: "FROM FIREBASE ADMIN DASHBOARD"
};

const app = initializeApp(firebaseConfig);
const db = getFirestore(app);

function App() {
  const [txs, setTxs] = useState(null);

  useEffect(() => {
    console.log("start");
    const q = query(collection(db, "moralis/events/Cryptopunktransferred"));
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
        <p>🔥 Streaming CryptoPunk Transfers 🔥</p>
            <table className="table">
              <tr>
                <th>From</th>
                <th>To</th>
                <th>Image</th>
                <th>Id</th>
              </tr>
              {txs?.map((e,i)=>{
                return(
                  <tr key={i}>
                    <td>{e.from}</td>
                    <td>{e.to}</td>
                    <td><img src={e.image} alt={`${i}punk`} width={50}/></td>
                    <td>{e.punkIndex}</td>
                  </tr>
                )
              })}
            </table>
      </header>
    </div>
  );
}
export default App;
