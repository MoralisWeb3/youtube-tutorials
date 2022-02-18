import React, {useEffect, useState} from 'react';
import {useMoralis, useMoralisWeb3Api} from 'react-moralis';
import {View, Text, StyleSheet} from 'react-native';
import {useMoralisDapp} from '../../providers/MoralisDappProvider/MoralisDappProvider';
import {n4} from '../../utils/formatters';
import useNativeBalance from '../../hooks/useNativeBalance';

import {getNativeByChain} from '../../utils/getNativeByChain';

function NativeBalance(props) {
  const {account} = useMoralisWeb3Api();
  const {Moralis} = useMoralis();
  const {walletAddress, chainId: chain} = useMoralisDapp();
  const [nativeChainString, setNativeChainString] = useState();

  const {nativeBalance} = useNativeBalance(props?.chain || chainId);

  return (
    <View style={styles.itemView}>
      <Text style={styles.name}>💰 {nativeBalance} </Text>
    </View>
  );
}
const styles = StyleSheet.create({
  itemView: {
    backgroundColor: 'white',
    padding: 20,
    // marginVertical: 8,
    marginHorizontal: 2,
    marginVertical: 5,
    borderRadius: 10,
    flex: 1,
    flexDirection: 'row',
    justifyContent: 'center',
    alignItems: 'center',
  },
  name: {
    fontSize: 15,
    color: 'black',
    fontWeight: '500',
  },
});

export default NativeBalance;
