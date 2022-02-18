import React, { useState } from "react";
import {
  StyleSheet,
  View,
  Text,
  SafeAreaView,
  TouchableOpacity,
} from "react-native";

import Button from "./components/Button";
import Clipboard from '@react-native-community/clipboard';

import {
  useMoralis,
} from "react-moralis";


const RecoveryPhrase = ({ route, navigation }) => {
  const {
    Moralis,
  } = useMoralis();
  const { recoveryPhrase } = route.params;

  const copyToClipboard = () => {
    Clipboard.setString(recoveryPhrase);
  };

  const renderRecoveryPhrase = () => {
    const split = recoveryPhrase.split(" ");
    return(
      <View style={styles.recoveryPhraseTextContainer}>
        {split.map((text) => {
          return(
            <TouchableOpacity style={{margin: 12}} key={text}>
              <Text style={styles.recoveryPhraseText}>{text}</Text>
            </TouchableOpacity>
          )
        })}
      </View>
    )
  }

  return (
    <SafeAreaView style={styles.mainBody}>
      <Text style={styles.recoveryPhraseHeaderText}>Write Down Recovery Phrase</Text>
      <View style={styles.card}>
       {renderRecoveryPhrase()}
       <TouchableOpacity style={styles.copyToClipboardButton} onPress={() => copyToClipboard()}>
        <Text style={styles.copyToClipboardText}>Copy to clipboard</Text>
       </TouchableOpacity>
      </View>
      <View style={styles.subtextContainer}>
        <Text>These 12 words are the keys to your wallet. Back it up on the cloud or back it up manually. Do not share this with anyone.</Text>
      </View>
      <View style={styles.buttonContainer}>
        <Button buttonName={'Next'} action={() => navigation.replace("Dashboard")}/>
      </View>
    </SafeAreaView>
  );
};
export default RecoveryPhrase;

const styles = StyleSheet.create({
  mainBody: {
    flex: 1,
    backgroundColor: '#fff'
  },
  card: {
    marginTop: 22, 
    width: '90%',
    height: 161,
    borderColor: '#c2c2c2', 
    borderWidth: 1,
    borderRadius: 5, 
    alignSelf: 'center'
  },
  recoveryPhraseHeaderText: {
    textAlign: 'center', 
    fontSize: 12, 
    color: '#C2C2C2'
  },
  copyToClipboardButton: {
    position: 'absolute',
    bottom: 10, 
    alignSelf: 'center'
  },
  copyToClipboardText: {
    color: '#1652F0', 
    textAlign: 'center', 
    fontSize: 12
  },
  subtextContainer: {
    width: '90%', 
    alignSelf: 'center', 
    marginTop: 20
  },
  buttonContainer: {
    position: 'absolute', 
    bottom: 45, 
    width: '100%'
  },
  recoveryPhraseTextContainer: {
    flexDirection: 'row', 
    flexWrap: 'wrap', 
    alignSelf: 'center', 
    alignSelf: 'center'
  },
  recoveryPhraseText: {
    textAlign: 'center', 
    fontSize: 15, 
    fontWeight: 'bold'
  }
});
