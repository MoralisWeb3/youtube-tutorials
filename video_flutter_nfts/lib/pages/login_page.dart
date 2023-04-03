import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:url_launcher/url_launcher_string.dart';
import 'package:walletconnect_dart/walletconnect_dart.dart';
import 'package:video_flutter_nfts/components/list_nfts.dart';

import 'package:http/http.dart' as http;

class LoginPage extends StatefulWidget {
  const LoginPage({Key? key}) : super(key: key);

  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  var connector = WalletConnect(
      bridge: 'https://bridge.walletconnect.org',
      clientMeta: const PeerMeta(
          name: 'Moralis NFTs',
          description: 'Get NFTs',
          url: 'https://moralis.io',
          icons: [
            'https://files.gitbook.com/v0/b/gitbook-legacy-files/o/spaces%2F-LJJeCjcLrr53DcT1Ml7%2Favatar.png?alt=media'
          ]));

  var _session, _uri;

  loginUsingMetamask(BuildContext context) async {
    if (!connector.connected) {
      try {
        var session = await connector.createSession(onDisplayUri: (uri) async {
          _uri = uri;
          await launchUrlString(uri, mode: LaunchMode.externalApplication);
        });
        print(session.accounts[0]);
        print(session.chainId);
        setState(() {
          _session = session;
        });
      } catch (exp) {
        print(exp);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Moralis NFTs'),
      ),
      body: SingleChildScrollView(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Container(
              width: double.infinity,
              alignment: Alignment.center,
              child: SizedBox(
                width: 150,
                height: 200,
                child: Image.asset(
                  'assets/images/main_page_image.png',
                  fit: BoxFit.contain,
                ),
              ),
            ),
            (_session != null)
                ? Container(
                    padding: const EdgeInsets.fromLTRB(20, 0, 20, 0),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          'Account',
                          style: GoogleFonts.robotoMono(
                              fontWeight: FontWeight.bold, fontSize: 16),
                        ),
                        Text(
                          '${_session.accounts[0]}',
                          style: GoogleFonts.robotoMono(fontSize: 16),
                        ),
                        Container(
                          margin: const EdgeInsets.only(top: 30),
                          child: Center(
                            child: ElevatedButton(
                              onPressed: () async {
                                Navigator.push(
                                    context,
                                    MaterialPageRoute(
                                      builder: (context) => NFTListPage(
                                          address: _session.accounts[0],
                                          chain: '0x' +
                                              _session.chainId.toString()),
                                    ));
                              },
                              child: const Text('Get NFTs'),
                            ),
                          ),
                        ),
                      ],
                    ))
                : ElevatedButton(
                    onPressed: () => loginUsingMetamask(context),
                    child: const Text(
                      "Connect with Metamask",
                    ))
          ],
        ),
      ),
    );
  }
}

// http://192.168.100.47:5002