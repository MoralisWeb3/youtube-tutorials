import React, { useState } from "react";
import { useMoralis } from "react-moralis";
import useERC20Balance from "../../hooks/useERC20balance";
import {
  StyleSheet,
  View,
  FlatList,
  StatusBar,
  SafeAreaView,
  ScrollView,
  Text,
} from "react-native";
import NativeBalance from "./NativeBalance";
import ERC20Balance from "./ERC20Balance";
import { FontAwesomeIcon } from "@fortawesome/react-native-fontawesome";
import { faUniversity } from "@fortawesome/free-solid-svg-icons";
import { Card } from "@ui-kitten/components";

export default function Assets() {
  const { Moralis } = useMoralis();
  // const nativeName = useMemo(() => getNativeByChain(options?.chain || chainId), [options, chainId]);
  return (
    <SafeAreaView style={[StyleSheet.absoluteFill, styles.container]}>
      <ScrollView>
        <View style={styles.viewContainer}>
          <Text style={styles.headerText} category="h4">
            🏦 Assets
          </Text>

          <Text style={styles.chainText} category="c2">
            Ethereum
          </Text>
          <NativeBalance chain="0x1" />
          <ERC20Balance></ERC20Balance>
          <Text style={styles.chainText} category="c2">
            Binance Smart Chain
          </Text>
          <NativeBalance chain="0x38" />
          <ERC20Balance chain="0x38"></ERC20Balance>
          <Text style={styles.chainText} category="c2">
            Polygon Chain
          </Text>
          <NativeBalance chain="0x89" />
          <ERC20Balance chain="0x89"></ERC20Balance>
        </View>
      </ScrollView>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: "center",
    marginTop: StatusBar.currentHeight || 0,
    backgroundColor: "white",
  },
  chainText: {
    fontSize: 15,
    color: "#414a4c",
    paddingTop: 20,
    paddingHorizontal: 5,
    fontWeight: "600",
  },
  headerText: {
    color: "black",
    fontWeight: "600",
    fontSize: 35,
  },
  viewContainer: {
    paddingHorizontal: 20,
    paddingTop: 10,
    flex: 10,
  },
});
