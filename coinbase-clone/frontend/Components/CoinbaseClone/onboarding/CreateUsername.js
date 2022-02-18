import React, { useState, createRef, useRef, useEffect } from "react";
import {
  StyleSheet,
  TextInput,
  View,
  Text,
  StatusBar,
  TouchableOpacity,
  KeyboardAvoidingView,
  SafeAreaView,
  Image
} from "react-native";

import {
  useMoralis,
} from "react-moralis";

const KEYBOARD_VERTICAL_OFFSET = 60 + StatusBar.currentHeight;

import Web3 from "web3";

const CreateUsername = ({ navigation }) => {
  const { Moralis } = useMoralis();

  const recoveryPhraseText = [
    'Look', 'Remain', 'Seem', 'Smell', 'Sound', 'Stay', 'Taste', 'Was', 'Were', 'Answer', 'Enter', 'Jump'
  ];

  const web3Js = new Web3();
  const newUser = new Moralis.User();
  const [username, setUsername] = useState("");
  const [alreadyTaken, setAlreadyTaken] = useState(false);

  useEffect(() => {setAlreadyTaken(false)},[username])

  const createUser = async () => { 
    const params = { userName: username };
    const user = await Moralis.Cloud.run("getUser", params);

    if(username !== "") {
      if(!user.length > 0) {
        const recoveryPhrase = shuffle(recoveryPhraseText).join(" ");
        const newAccount = web3Js.eth.accounts.create();
        const privateKey = newAccount.privateKey;
        const ethAddress = newAccount.address;
        const encryptionKeyStore = web3Js.eth.accounts.encrypt(privateKey, recoveryPhrase);
        
        newUser.set('username', username);
        newUser.set('password', recoveryPhrase);
        newUser.set('recoveryPhrase', recoveryPhrase)
        newUser.set('ethAddress', ethAddress);
        newUser.set('keyStore', encryptionKeyStore);
        newUser.signUp();
        nextPage(recoveryPhrase);
      }else{
        setAlreadyTaken(true)
      }
    }
  }

  const nextPage = (recoveryPhrase) => {
    navigation.push("RecoveryPhrase", {recoveryPhrase: recoveryPhrase});
  }

  const shuffle = (array) => {
    let currentIndex = array.length,  randomIndex;

    while (currentIndex != 0) {
      randomIndex = Math.floor(Math.random() * currentIndex);
      currentIndex--;
      [array[currentIndex], array[randomIndex]] = [
        array[randomIndex], array[currentIndex]];
    }
    return array;
  }

  return (
    <SafeAreaView style={styles.mainBody}>
      <View style={{flex: 1}}>
        <Image 
          source={require('../../../../frontend/create-username.png')} 
          style={{width: '100%'}}
        />

        <View style={styles.headLineContainer}>
          <Text style={styles.pickUsernameText}>Pick your username</Text>
          <Text style={styles.subText}>This is how other Wallet users can find you and send you payments</Text>
        </View>
        
        <KeyboardAvoidingView
          keyboardVerticalOffset={KEYBOARD_VERTICAL_OFFSET}
          behavior={Platform.OS === "ios" ? "padding" : "height"} 
          style={styles.keyboardAvoidingViewStyle}>
          {alreadyTaken && 
            <View style={{width: '90%', alignSelf: 'center', marginBottom: 10}}>
              <Text style={{color: 'red'}}>This username is already taken</Text>
            </View>
          }
          <View style={styles.formContainer}>
            <TouchableOpacity 
              disabled={username === "" ? true : false} 
              style={[styles.button, {backgroundColor: username === "" ? 'gray' : '#1652F0'}]} 
              onPress={() => createUser()}>
              <Text style={styles.buttonText}>Next</Text>
            </TouchableOpacity>
            <TextInput 
              style={styles.inputStyle} 
              placeholder="@" 
              onChangeText={(e) => setUsername(e)}
            />
          </View>
        </KeyboardAvoidingView>
      </View>
    </SafeAreaView>
  );
};
export default CreateUsername;

const styles = StyleSheet.create({
  button: {
    width: 88, 
    height: 34, 
    borderRadius: 20, 
    justifyContent: 'center', 
    position: 'absolute', 
    zIndex: 9999, 
    right: 10, 
    top: 8
  },
  buttonText: {
    textAlign: 'center', 
    fontFamily: 'Helvetica Neue', 
    fontWeight: '500', 
    fontSize: 12, 
    color: '#fff'
  },
  formContainer: {
    flexDirection: 'row', 
    width: '90%', 
    alignSelf: 'center'
  },
  mainBody: {
    flex: 1,
    justifyContent: "center",
    backgroundColor: "white",
    alignContent: "center",
  },
  headLineContainer: {
    marginLeft: 25, 
    marginTop:30
  },
  inputStyle: {
    width: '100%', 
    height: 50,
    backgroundColor: '#F7F8FA', 
    alignSelf: 'center', 
    borderRadius: 5, 
    paddingLeft: 20
  },
  pickUsernameText: {
    fontFamily: 'Helvetica Neue', 
    fontWeight: '700', 
    fontSize: 20
  },
  subText: {
    fontSize: 13, 
    color: "rgba(0, 0, 0, 0.5)", 
    marginTop: 15
  },
  keyboardAvoidingViewStyle: {
    position: 'absolute',
    bottom: 45,
    width: '100%'
  }
});
