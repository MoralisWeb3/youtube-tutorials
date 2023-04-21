import React, { useState, useEffect } from 'react';
import type { PropsWithChildren } from 'react';
import {
  SafeAreaView,
  ScrollView,
  StatusBar,
  StyleSheet,
  Text,
  TextInput,
  TouchableOpacity,
  useColorScheme,
  View,
  Image,
} from 'react-native';
import { Colors } from 'react-native/Libraries/NewAppScreen';
import axios from 'axios';

type SectionProps = PropsWithChildren<{
  title: string;
}>;

function App(): JSX.Element {
  const isDarkMode = useColorScheme() === 'dark';
  const [walletAddress, setWalletAddress] = useState('');
  const [nfts, setNFTs] = useState<any>();

  interface NFT {
    owner_of: string;
    normalized_metadata: {
      image: string;
      name: string;
      description: string;
      attributes: { value: string }[];
    };
  }

  const backgroundStyle = {
    backgroundColor: isDarkMode ? Colors.darker : Colors.lighter,
  };


  const getNFTs = async () => {
    const response = await axios.get(
      `http://192.168.100.47:5002/get_user_nfts?address=${walletAddress}`
    ).then((res) => {
      setNFTs(res.data.result)
    }).catch((err) => console.log(err))
  }

  const NFTCard = ({ nft }: { nft: NFT }) => {
    return (
      <View style={styles.card}>
        <Image source={{ uri: nft.normalized_metadata.image }} style={styles.image} />
        <View style={styles.info}>
          <Text style={styles.title}>{nft.normalized_metadata.name}</Text>
          <Text style={styles.description}>{nft.normalized_metadata.description}</Text>
          {nft.normalized_metadata.attributes.map((attr, index) => (
            <Text style={styles.attribute} key={index}>
              {attr.value}
            </Text>
          ))}
        </View>
      </View>
    )
  };

  const renderedNFts = nfts && nfts.map((nft: NFT, index: number) => <NFTCard key={index} nft={nft} />)

  return (
    <SafeAreaView style={[backgroundStyle, { flex: 1 }]}>
      <StatusBar
        barStyle={isDarkMode ? 'light-content' : 'dark-content'}
        backgroundColor={backgroundStyle.backgroundColor}
      />
      <ScrollView
        contentInsetAdjustmentBehavior="automatic"
        style={[backgroundStyle, { flex: 1 }]}>
        <View style={styles.container}>
          <View style={styles.logoContainer}>
            <Image
              source={require('./assets/logo.png')}
              style={styles.logo}
              resizeMode="contain"
            />
          </View>
          <Text style={styles.appTitle}>Moralis NFTs</Text>
          <TextInput
            placeholder="Enter your wallet address"
            style={styles.input}
            onChangeText={text => setWalletAddress(text)}
            value={walletAddress}
          />
          <TouchableOpacity style={styles.button} onPress={getNFTs}>
            <Text style={styles.buttonText}>GET NFTs</Text>
          </TouchableOpacity>
          {renderedNFts}
        </View>
      </ScrollView>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  sectionContainer: {
    marginTop: 32,
    paddingHorizontal: 24,
  },
  sectionTitle: {
    fontSize: 24,
    fontWeight: '600',
  },
  sectionDescription: {
    marginTop: 8,
    fontSize: 18,
    fontWeight: '400',
  },
  highlight: {
    fontWeight: '700',
  },
  appTitle: {
    fontSize: 28,
    fontWeight: 'bold',
    marginTop: 20,
    marginBottom: 10,
  },
  input: {
    borderWidth: 1,
    borderColor: 'gray',
    borderRadius: 8,
    paddingHorizontal: 10,
    paddingVertical: 8,
    marginBottom: 10,
    width: '100%',
  },
  button: {
    backgroundColor: '#0f8cff',
    borderRadius: 8,
    paddingHorizontal: 20,
    paddingVertical: 10,
    marginBottom: 30,
  },
  buttonText: {
    color: 'white',
    fontSize: 18,
    fontWeight: 'bold',
    textAlign: 'center',
  },
  container: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    padding: 20,
  },
  logoContainer: {
    height: 200,
    alignSelf: 'stretch',
    justifyContent: 'center',
    alignItems: 'center',
    marginVertical: 20,
  },
  logo: {
    flex: 1,
    alignSelf: 'stretch',
    width: undefined,
    height: undefined,
  },
  containerCard: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#F5FCFF',
  },
  card: {
    flexDirection: 'row',
    alignItems: 'center',
    marginVertical: 10,
    marginHorizontal: 20,
    backgroundColor: 'white',
    borderRadius: 10,
    shadowColor: '#000',
    shadowOffset: {
      width: 0,
      height: 2,
    },
    shadowOpacity: 0.25,
    shadowRadius: 3.84,

    elevation: 5,
  },
  image: {
    width: 100,
    height: 100,
    borderRadius: 10,
    margin: 10,
  },
  info: {
    flex: 1,
    margin: 10,
  },
  title: {
    fontSize: 18,
    fontWeight: 'bold',
    marginBottom: 5,
  },
  description: {
    marginBottom: 5,
  },
  attribute: {
    color: '#777',
    fontSize: 12,
  },
});

export default App;

