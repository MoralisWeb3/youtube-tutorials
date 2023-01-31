import { initializeApp } from "firebase/app";
import { browserSessionPersistence, getAuth } from "firebase/auth";
import { connectFunctionsEmulator, getFunctions } from "firebase/functions";
import { getMoralisAuth } from "@moralisweb3/client-firebase-auth-utils";

export async function initializeFirebase() {
  const app = initializeApp({
    apiKey: process.env.NEXT_PUBLIC_FIREBASE_API_KEY,
    authDomain: process.env.NEXT_PUBLIC_FIREBASE_AUTH_DOMAIN,
    projectId: process.env.NEXT_PUBLIC_FIREBASE_PROJECT_ID,
    storageBucket: process.env.NEXT_PUBLIC_FIREBASE_STORAGE_BUCKET,
    messagingSenderId: process.env.NEXT_PUBLIC_FIREBASE_MESSAGING_SENDER_ID,
    appId: process.env.NEXT_PUBLIC_FIREBASE_APP_ID,
  });

  const auth = getAuth(app);
  await auth.setPersistence(browserSessionPersistence);

  const functions = getFunctions(app);
  if (window.location.hostname === "localhost") {
    connectFunctionsEmulator(functions, "localhost", 5001);
  }

  const moralisAuth = getMoralisAuth(app, {
    auth,
    functions,
  });

  return { app, auth, functions, moralisAuth };
}
