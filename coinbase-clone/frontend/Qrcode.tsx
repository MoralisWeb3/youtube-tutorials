import React, { useState } from "react";
import { StyleSheet, View, Alert, Text, Pressable } from "react-native";
import QR from "react-native-qrcode-svg";
import { Modal, Portal, Button, Provider } from "react-native-paper";

export type QrcodeProps = {
  readonly uri?: string;
  readonly size?: number;
};

const padding = 15;

const styles = StyleSheet.create({
  center: {
    alignItems: "center",
    justifyContent: "center",
  },
  mt: {
    marginTop: 150,
  },
  qr: {
    padding,
    backgroundColor: "white",
    overflow: "hidden",
    borderRadius: padding,
  },
  centeredView: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
    marginTop: 22,
  },
  modalView: {
    margin: 20,
    backgroundColor: "white",
    borderRadius: 20,
    padding: 35,
    alignItems: "center",
    shadowColor: "#000",
    shadowOffset: {
      width: 0,
      height: 2,
    },
    shadowOpacity: 0.25,
    shadowRadius: 4,
    elevation: 5,
  },
  button: {
    borderRadius: 20,
    padding: 10,
    elevation: 2,
  },
  buttonOpen: {
    backgroundColor: "#F194FF",
  },
  buttonClose: {
    backgroundColor: "#2196F3",
  },
  textStyle: {
    color: "white",
    fontWeight: "bold",
    textAlign: "center",
  },
  modalText: {
    marginBottom: 15,
    textAlign: "center",
  },
});

export default function Qrcode({ size = 400, uri }: QrcodeProps): JSX.Element {
  const [modalVisible, setModalVisible] = useState(false);

  const [visible, setVisible] = React.useState(false);

  const showModal = () => setVisible(true);
  const hideModal = () => setVisible(false);
  const containerStyle = { backgroundColor: "white", padding: 20 };

  if (!uri) {
    return null;
  }
  return (
    <Provider>
      <Portal>
        <Modal
          visible={visible}
          onDismiss={hideModal}
          contentContainerStyle={containerStyle}>
          <View style={styles.modalView}>
            {/* <View
        style={[
          { width: size, height: size },
          styles.center,
          styles.qr,
          styles.mt,
        ]}> */}
            {typeof uri === "string" && !!uri.length && (
              // @ts-ignore
              <QR logoSize={size * 0.2} value={uri} size={size - padding * 2} />
            )}
            {/* </View> */}

            {/* <Text style={styles.modalText}>Hello World!</Text>
          <Pressable
            style={[styles.button, styles.buttonClose]}
            onPress={() => setModalVisible(!modalVisible)}>
            <Text style={styles.textStyle}>Hide Modal</Text>
          </Pressable> */}
            {/* </View> */}
          </View>
          <Text>Example Modal. Click outside this area to dismiss.</Text>
        </Modal>
      </Portal>
      <Button style={{ marginTop: 30 }} onPress={showModal}>
        Show
      </Button>
    </Provider>
  );
}
