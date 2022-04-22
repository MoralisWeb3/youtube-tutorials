
mergeInto(LibraryManager.library, {
  MoralisLiveQueries: function () {
  },
  
  OpenWebsocketJs: function (key, path) {
    window.moralisLiveQueries.openSocket(Pointer_stringify(key), Pointer_stringify(path));
  },
  
  OpenWebsocketResponse: function () {
    var bufferSize = lengthBytesUTF8(window.moralisLiveQueries.openSocketResponse) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(window.moralisLiveQueries.openSocketResponse, buffer, bufferSize);
    return buffer; 
  },
  
  CloseWebsocketJs: function (key) {
    console.log("mlqgl ... CloseWebsocketJs called.");
    window.moralisLiveQueries.closeSocket(Pointer_stringify(key));
  },
  
  CloseWebsocketResponse: function () {
    var bufferSize = lengthBytesUTF8(window.moralisLiveQueries.closeSocketResponse) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(window.moralisLiveQueries.closeSocketResponse, buffer, bufferSize);
    return buffer; 
  },
  
  SendMessageJs: function (key, message) {
    window.moralisLiveQueries.sendRequest(Pointer_stringify(key), Pointer_stringify(message));
  },

  GetErrorQueueJs: function (key) {
    console.log("mlqgl ... GetErrorQueueJs called.");
    var errors = window.moralisLiveQueries.getErrors(Pointer_stringify(key));
    var errorString = JSON.stringify(errors);
    var bufferSize = lengthBytesUTF8(errorString) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(errorString, buffer, bufferSize);
    return buffer; 
  },

  GetResponseQueueJs: function (key) {
    var msgs = window.moralisLiveQueries.getMessages(Pointer_stringify(key));
    var respString = JSON.stringify(msgs); 
    var bufferSize = lengthBytesUTF8(respString) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(respString, buffer, bufferSize);
    return buffer; 
  },

  GetSocketStateJs: function (key) {
    return window.moralisLiveQueries.getSocketState(Pointer_stringify(key));
  }
});
