import { createContext, useContext, useEffect, useState } from "react";
import { initializeFirebase } from "./Firebase";

const firebaseContext = createContext(null);

export function useFirebase() {
  const firebase = useContext(firebaseContext);
  if (!firebase) {
    throw new Error("Cannot find Firebase context");
  }
  return firebase;
}

export function FirebaseInitializer(props) {
  const [firebase, setFirebase] = useState(null);

  useEffect(() => {
    const handleSuccess = setFirebase;
    const handleError = alert;

    initializeFirebase().then(handleSuccess, handleError);
  }, []);

  return (
    <firebaseContext.Provider value={firebase}>
      {firebase ? props.initialized() : props.initializing()}
    </firebaseContext.Provider>
  );
}
