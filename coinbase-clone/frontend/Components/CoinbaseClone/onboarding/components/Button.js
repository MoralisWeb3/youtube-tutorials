import React  from "react";
import {
  StyleSheet,
  Text,
  TouchableOpacity
} from "react-native";

const Button = ({ buttonName, action }) => {
  return (
    <TouchableOpacity style={styles.button} onPress={() => action()}> 
      <Text style={styles.buttonText}>{buttonName}</Text>
    </TouchableOpacity>
  );
};
export default Button;

const styles = StyleSheet.create({
  button: {
    alignSelf: 'center',
    backgroundColor: '#1652F0', 
    width: '90%', 
    height: 58, 
    borderRadius: 15, 
    justifyContent: 'center'
  },
  buttonText: {
    color: '#fff', 
    fontSize: 18, 
    textAlign: 'center'
  },
});
