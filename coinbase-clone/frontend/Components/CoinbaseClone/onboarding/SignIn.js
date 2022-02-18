import React, { useState, createRef, useRef, useEffect, useContext } from "react";
import {
  StyleSheet,
  View,
  Text,
  SafeAreaView,
  TouchableOpacity,
  TextInput
} from "react-native";

import Button from "./components/Button";
import { UserContext } from "../../../UserContext";

import {
  useMoralis,
} from "react-moralis";

import Web3 from "web3";


const SignIn = ({ navigation }) => {
  const {
    Moralis,
  } = useMoralis();

  const web3Js = new Web3();

  const {setValue} = useContext(UserContext);

  const [recoveryPhrase, setRecoveryPhrase] = useState();


  const authenticate = async () => {
    const params = { recoveryPhrase: recoveryPhrase };
    const user = await Moralis.Cloud.run("getPassword", params);
    let keyStore = []
    let userDecrpytData = []
    let username = "";

    if(user.length > 0) {
      keyStore = user[0].attributes.keyStore;
      userDecrpytData = web3Js.eth.accounts.decrypt(keyStore, recoveryPhrase);
      username = user[0].attributes.username;

      await Moralis.User.logIn(username, recoveryPhrase)

      setValue(userDecrpytData);
      navigation.replace("Dashboard");
    }

  }

  return (
    <SafeAreaView style={styles.mainBody}>
      <Text style={styles.siginInRecoveryText}>Sign in with Recovery Phrase</Text>
      <View style={styles.subTextContainer}>
        <Text style={{color: '#4F5C6C'}}>This is a 12 word phrase you were given when you created your previous wallet.</Text>
      </View>
      <TextInput 
        style={styles.card}
        multiline={true}
        selectTextOnFocus={true}
        placeholder="Recovery phrase..."
        onChangeText={phrase => setRecoveryPhrase(phrase)}
        numberOfLines={4}/>
      <View style={{position: 'absolute', width: '100%', bottom: 45}}>
        <Button buttonName={'Sign in'} action={() => authenticate()}/>
      </View>
    </SafeAreaView>
  );
};
export default SignIn;

const styles = StyleSheet.create({
  mainBody: {
    flex: 1,
    backgroundColor: '#fff'
  },
  card: {
    backgroundColor: '#E9E9E9',
    marginTop: 22, 
    width: '90%',
    height: 93,
    borderColor: '#c2c2c2', 
    borderWidth: 1,
    borderRadius: 5, 
    alignSelf: 'center',
    padding: 20,
    paddingTop: 20,
    fontSize: 15, 
    fontWeight: 'bold'
  },
  siginInRecoveryText: {
    textAlign: 'center', 
    fontSize: 24, color: '#1F2533',
    fontWeight: '700', 
    fontFamily: 'Helvetica Neue', 
    marginTop: 35
  },
  subTextContainer: {
    width: '90%', 
    alignSelf: 'center', 
    marginTop: 10
  }
});
