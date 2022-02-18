import React, { useState, createRef, useRef, useEffect, useContext } from "react";
import {
  StyleSheet,
  View,
  Text,
  SafeAreaView,
  TouchableOpacity,
  FlatList,
  Image,
} from "react-native";

import MoralisMaharaja from "../../moralis-maharaja.png";
import MoralisMalevolent from "../../moralis-malevolent.png";
import MoralisMarauder from "../../moralis-marauder.png";
import MoralisMastermind from "../../moralis-mastermind.png";
import MoralisMechtician from "../../moralis-mechtician.png";
import MoralisMonastic from "../../moralis-monastic.png";
import MoralisMoralissimus from "../../moralis-moralissimus.png";
import MoralisMorpheus from "../../moralis-morpheus.png";

import {
  useMoralis,
} from "react-moralis";

import { UserContext } from "../../UserContext";


const Dashboard = ({ route }) => {
  const { Moralis } = useMoralis();
  const {value, setValue} = useContext(UserContext);
  const [uniswap, setUniswap] = useState();
  const [coinSelected, setCoinSelected] = useState(true);

  const tokenValue = (value, decimals) => (decimals ? value / Math.pow(10, decimals) : value);

  const user = Moralis.User.current()

  const NFTs = [
    {
      id: 1,
      nft: MoralisMaharaja,
      name: 'Moralis Maharaja'
    },
    {
      id: 2,
      nft: MoralisMalevolent,
      name: 'Moralis Malevolent'
    },
    {
      id: 3,
      nft: MoralisMarauder,
      name: 'Moralis Marauder'
    },
    {
      id: 4,
      nft: MoralisMastermind,
      name: 'Moralis Mastermind'
    },
    {
      id: 5,
      nft: MoralisMechtician,
      name: 'Moralis Mechtician'
    },
    {
      id: 6,
      nft: MoralisMonastic,
      name: 'Moralis Monastic'
    },
    {
      id: 7,
      nft: MoralisMoralissimus,
      name: 'Moralis Moralissimus'
    },
    {
      id: 8,
      nft: MoralisMorpheus,
      name: 'Moralis Morpheus'
    },
  ]

  const getTokenPrice = async () => {
    await Promise.all([
      setUniswap(await Moralis.Web3API.account.getTokenBalances({
        address: "0x1f9840a85d5aF5bf1D1762F925BDADdC4201F984",
      }))
    ])
  }

  useEffect(() => {
    getTokenPrice()
  },[])

  const renderItem = ({ item }) => (
    item.logo !== null &&
    <TouchableOpacity style={styles.coinsContainer} key={item.name}>
      <Image source={{uri: item.logo}} style={styles.coinsLogo}/>
      <View style={styles.coinsDetailContainer}>
        <Text style={styles.coinsNameText}>{item.name}</Text>
        <Text style={styles.coinsSymbol}>{item.symbol}</Text>
      </View>
    </TouchableOpacity>
  );

  const renderNFT = ({ item }) => (
    <TouchableOpacity key={item.id} style={styles.nftContainer}>
      <Image source={item.nft} style={styles.nftImage}/>
      <Text style={styles.nftName}>{item.name}</Text>
      <Text style={styles.nftCreator}>@BoredMoralisMages</Text>
    </TouchableOpacity>
  );

  return (
    <View style={styles.mainBody}>
      <SafeAreaView style={{marginTop: 20}}>
        <Text style={styles.userName}>@{user.attributes.username}</Text>
        <Text style={styles.usdBalance}>US$0.00</Text>

        <View style={styles.buttonContainer}>
          <TouchableOpacity style={styles.sendButton} >
            <Text style={styles.buttonText}>Send</Text>
          </TouchableOpacity>
          <TouchableOpacity style={styles.receiveButton}>
            <Text style={styles.buttonText}>Receive</Text>
          </TouchableOpacity>
        </View>
      </SafeAreaView>

      <View style={styles.portFolioContainer}>
        <View style={styles.portfolioButtons}>
          <TouchableOpacity 
            style={[
              styles.coinsButton, 
              {
                borderBottomColor: coinSelected ? '#1652F0': 'gray',
                borderBottomWidth: coinSelected ? 3: 0.2 
              }
            ]} 
            onPress={() => setCoinSelected(true)}>
            <Text style={styles.portfolioButtonsText}>Coins</Text>
          </TouchableOpacity>
          <TouchableOpacity 
            style={[
              styles.nftButton, 
              {
                borderBottomColor: !coinSelected ? '#1652F0': 'gray',
                borderBottomWidth: !coinSelected ? 3: 0.2 
              } 
            ]} 
            onPress={() => setCoinSelected(false)}>
            <Text style={styles.portfolioButtonsText}>NFTs</Text>
          </TouchableOpacity>
        </View>
        
        <View style={{padding: 10, margin: 10}}>
          {coinSelected ?
            <View>
              <Text style={styles.tokenLabelText}>Tokens</Text>
              <FlatList
                key={'coins'}
                data={uniswap}
                renderItem={renderItem}
                keyExtractor={item => item.id}
                style={{marginTop: 10}}
                contentContainerStyle={{ paddingBottom: 50}}
                showsVerticalScrollIndicator={false}
              />
            </View>
            :
            <View>
              <FlatList
                key={'tokens'}
                data={NFTs}
                renderItem={renderNFT}
                contentContainerStyle={{alignSelf: 'flex-start', paddingBottom: 50}}
                numColumns={Math.ceil(NFTs.length / 4)}
                keyExtractor={nft => nft.id}
                showsVerticalScrollIndicator={false}
                showsHorizontalScrollIndicator={false}
              />
            </View>
          }
        </View>
      </View>
    </View>
  );
};

export default Dashboard;

const styles = StyleSheet.create({
  mainBody: {
    flex: 1,
    height: '50%',
    backgroundColor: '#1652F0'
  },
  userName: {
    fontSize: 14, 
    color: '#B5CBFF', 
    textAlign: 'center'
  },
  usdBalance: {
    color: '#fff', 
    textAlign: 'center', 
    fontSize: 36, 
    fontWeight: '700', 
    fontFamily: 'Helvetica Neue', 
    marginTop: 10
  },
  buttonContainer: {
    backgroundColor: '#0A49EE', 
    width: '75%', 
    height: 47, 
    borderRadius: 15, 
    alignSelf: 'center', 
    marginTop: 20, 
    flexDirection: 'row'
  },
  sendButton: {
    width: '50%', 
    justifyContent: 'center', 
    borderRightColor: '#fff', 
    borderRightWidth: 1
  },
  receiveButton: {
    width: '50%', 
    justifyContent: 'center'
  },
  buttonText: {
    fontSize: 14, 
    fontWeight: '700', 
    fontFamily: 'Helvetica Neue', 
    color: '#fff', 
    textAlign: 'center'
  },
  tradeButton: {
    width: '33.3%', 
    justifyContent: 'center'
  },
  portFolioContainer: {
    backgroundColor: '#fff', 
    width: '95%', 
    flex: 1, 
    alignSelf: 'center', 
    marginTop: 40, 
    borderTopRightRadius: 20, 
    borderTopLeftRadius: 20
  },
  portfolioButtons: {
    flexDirection: 'row'
  },
  coinsButton: {
    width: '50%',
    justifyContent: 'center', 
    height: 51, 
    borderBottomWidth: 2
  },
  portfolioButtonsText: {
    fontSize: 14, 
    fontWeight: '700', 
    color: '#757575', 
    textAlign: 'center'
  },
  nftButton: {
    width: '50%',
    justifyContent: 'center', 
    height: 51, 
  },
  tokenLabelText: {
    color: '#AAAAAA',
    fontSize: 10, 
    fontWeight: '700', 
    fontFamily: 'Helvetica Neue'
  },
  coinsContainer: {
    flexDirection: 'row', 
    margin: 10
  },
  coinsLogo: {
    height: 50,
    width: 50
  },
  coinsDetailContainer: {
    justifyContent: 'center', 
    marginLeft: 13
  },
  coinsNameText: {
    fontFamily: 'Helvetica Neue', 
    fontWeight: '700'
  },
  coinsSymbol: {
    fontFamily: 'Helvetica Neue', 
    fontWeight: '700', 
    fontSize: 10, 
    color: '#AAAAAA'
  },
  nftContainer: {
    width: 150, 
    height: 156, 
    marginTop: 25,
    marginBottom: 25, 
    marginLeft: 10, 
    marginRight: 10
  },
  nftImage: {
    width: 150, 
    height: 156
  },
  nftName: { 
    fontWeight: '700', 
    fontSize: 12, 
    fontFamily: 'Helvetica Neue', 
    marginTop: 8
  },
  nftCreator: {
    color: '#AAAAAA',
    fontWeight: '400', 
    fontSize: 12, 
    fontFamily: 'Helvetica Neue', 
    marginTop: 3
  }
});
