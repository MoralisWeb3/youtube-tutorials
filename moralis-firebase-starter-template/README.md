# Moralis + Firebase + Next.js Demo

This project contains a simple Next.js app integrated with Moralis and Firebase.

Required Google Cloud services:

- [Firebase Hosting](https://firebase.google.com/docs/hosting)
- [Firebase Authentication](https://firebase.google.com/docs/auth)
- [Secret Manager](https://cloud.google.com/secret-manager/) (check the [pricing](https://cloud.google.com/secret-manager/pricing))

## 🚀 How to Start

1. Create a Firebase project.
2. Upgrade to Blaze plan.
3. Activate the `Authentication` feature in the Firebase Console. Go to the Firebase Console > Your Project > Build > Authentication and click the Get Started button.
4. Create a Webb App and Copy Config for step 15.
5. Generate a certificate for the [Service Account](https://firebase.google.com/support/guides/service-accounts). You will need it in the next step.
6. Convert the certificate to extension variables by [this online converter](https://moralisweb3.github.io/firebase-extensions/service-account-converter/). You will use these variables in step 13.
7. Clone this repo.
8. [Install Firebase CLI](https://firebase.google.com/docs/cli) globally: `npm install -g firebase-tools`
9. Login to your account: `firebase login`
10. Get list of your projects: `firebase projects:list`. If this list is empty you should add a new project. You can do it by the Firebase Console.
11. Set your project ID: `firebase use <PROJECT_ID>`
12. Enable the [webframeworks feature](https://firebase.google.com/docs/hosting/frameworks-overview): `firebase experiments:enable webframeworks`
13. Install the Authenticate with Moralis Web3 extension: `firebase ext:install moralis/moralis-auth`.
14. Deploy extensions `firebase deploy --only extensions`
15. Copy `hosting/.env.example` to `hosting/.env` and set all variables.

### 🔌 Run Locally

1. Run emulators: `firebase emulators:start`
2. Open `http://localhost:5555/` in your browser.
