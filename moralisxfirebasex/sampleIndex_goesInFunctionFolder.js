///Import Required Dependencies

const admin = require("firebase-admin");
const functions = require("firebase-functions");
const fb = require("firebase-admin/app");
const Moralis = require("moralis").default;
const serviceAccountCert = require("./serviceAccountCert.json"); // ADD THIS FILE FROM FIREBASE ADMIN PANEL


///Add Admin SDK to server

const app = admin.initializeApp({
  ...functions.config().firebase,
  credential: fb.cert(serviceAccountCert),
});
const auth = admin.auth(app);


///Request a Moralis Signature Message

exports.requestMessage = functions.https.onCall(async (data) => {
  await Moralis.start({
    apiKey: "MORALIS API KEY",
  });

  const now = new Date();
  const expirationDays = 7;
  const expiration = new Date(now.getTime() + expirationDays * 86400000);

  const response = await Moralis.Auth.requestMessage({
    chain: data.chain,
    network: "evm",
    timeout: 15,
    domain: "mydomain.com",
    uri: "https://mydomain.com/my-uri",
    statement: "Please confirm this message",
    address: data.address,
    notBefore: now.toISOString(),
    expirationTime: expiration.toISOString(),
  });
  return response.raw;
});


///Verify a Moralis Signature and Issue Firebase Token

exports.issueToken = functions.https.onCall(async (data) => {
  await Moralis.start({
    apiKey: "MORALIS API KEY",
  });

  const response = await Moralis.Auth.verify({
    network: "evm",
    message: data.message,
    signature: data.signature,
  });
  const uid = response.result.profileId;

  try {
    const u = await auth.getUser(uid);
    console.log(u);
  } catch (e) {
    if (e.code === "auth/user-not-found") {
      await auth.createUser({
        uid,
        displayName: response.result.address.checksum,
      });
      console.log("user added");
    }
  }

  const token = await auth.createCustomToken(uid);
  return { token };
});


