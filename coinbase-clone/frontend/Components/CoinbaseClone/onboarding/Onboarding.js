import React, { useState, createRef, useRef, useEffect } from "react";
import {
  StyleSheet,
  Text,
  SafeAreaView,
  View,
  TouchableOpacity,
} from "react-native";

import Button from "./components/Button";

import {
  useMoralis,
} from "react-moralis";


const Onboarding = ({ navigation }) => {
  const {
    isAuthenticated
  } = useMoralis();

  useEffect(() => {
    isAuthenticated && navigation.replace("Dashboard")
  },[])

  return (
    <SafeAreaView style={styles.mainBody}>
      <View style={styles.logoContainer}>
        <Text style={styles.logoText}>coinbase</Text>
        <Text style={styles.logoSubtext}>Wallet</Text>
      </View>
      <View style={styles.subTextContainer}>
        <Text style={styles.subText}>Your ethereum wallet boilerplate</Text>
      </View>
      <View style={styles.buttonContainer}>
        <Button 
          navigation={navigation} 
          buttonName={'Create new wallet'} 
          action={() => navigation.push('CreateUsername')}
        />
        <TouchableOpacity 
          style={{marginTop: 30}} 
          onPress={() => navigation.push('SignIn')}>
            <Text style={styles.importWalletText}>Import existing wallet</Text>
        </TouchableOpacity>
      </View>
    </SafeAreaView>
  );
};
export default Onboarding;

const styles = StyleSheet.create({
  mainBody: {
    flex: 1,
    backgroundColor: '#fff'
  },
  logoContainer: {
    flexDirection: 'row', 
    alignSelf: 'center', 
    marginTop: '50%'
  },
  logoText: {
    color: '#0052FF', 
    fontSize: 40, 
    marginRight: 10,  
    fontWeight: 'bold'
  },
  logoSubtext: {
    color: '#000000', 
    fontSize: 35, 
    marginTop: 5, 
    fontWeight: 'bold'
  },
  subTextContainer: {
    width: '70%', 
    alignSelf: 'center', 
    marginTop: 17
  },
  subText: {
    fontSize: 18, 
    fontWeight: '400', 
    fontFamily: 'Helvetica Neue',
    color: 'rgba(0, 0, 0, 0.5)',
    textAlign: 'center'
  },
  buttonContainer: {
    position: 'absolute', 
    bottom: 100, 
    width: '100%'
  },
  importWalletText: {
    fontSize: 18, 
    fontFamily: 'Helvetica Neue', 
    textAlign: 'center', 
    color: '#1652F0'
  }
});
