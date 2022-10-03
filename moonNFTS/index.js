const firebaseAdmin = require("firebase-admin");
const { v4: uuidv4 } = require("uuid");
const serviceAccount = require("./serviceAccount.json");


const admin = firebaseAdmin.initializeApp({
    credential: firebaseAdmin.credential.cert(serviceAccount),
  });

const storageRef = admin.storage().bucket("gs://moonnfts-d3cbe.appspot.com");

async function uploadFile(path, filename) {
  
    const storage = storageRef.upload(path, {
        public: true,
        destination: `metadata/${filename}`,
        metadata: {
          metadata: {
            firebaseStorageDownloadTokens: uuidv4(),
          },
        },
    });

    return storage

}


(async () => {
    for (let i = 1; i < 11; i++) {
    await uploadFile(`./metadata/${i}.json`, `${i}.json`);
    console.log("Uploaded meta number " + i);
    }
})();
