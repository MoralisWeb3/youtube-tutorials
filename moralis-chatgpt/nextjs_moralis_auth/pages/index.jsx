import { useEvmNativeBalance } from '@moralisweb3/next';

function HomePage() {
    const address = '0xd8da6bf26964af9d7eed9e03e53415d37aa96045';
    const { data: nativeBalance } = useEvmNativeBalance({ address });
    console.log(nativeBalance);
    return (
        <div>
            <h3>Wallet: {address}</h3>
            <h3>Native Balance: {nativeBalance?.balance.ether} ETH</h3>
        </div>
    );
}

export default HomePage;
