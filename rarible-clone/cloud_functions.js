Moralis.Cloud.define("getUserItems", async (request) => {

    const query = new Moralis.Query("NFTTokenOwners");
    query.equalTo("contract_type", "ERC721");
    query.containedIn("owner_of", request.user.attributes.accounts);
    const queryResults = await query.find();
    const results = [];
    for (let i = 0; i < queryResults.length; ++i) {
      results.push({
        "tokenObjectId": queryResults[i].id,
        "tokenId": queryResults[i].attributes.token_id,
        "tokenAddress": queryResults[i].attributes.token_address,
        "symbol": queryResults[i].attributes.symbol,
        "tokenUri": queryResults[i].attributes.token_uri,
      });
    }
    return results;
  });
  
  Moralis.Cloud.beforeSave("ItemsForSale", async (request) => {
  
    const query = new Moralis.Query("NFTTokenOwners");
    query.equalTo("token_address", request.object.get('tokenAddress'));
    query.equalTo("token_id", request.object.get('tokenId'));
    const object = await query.first();
    if (object){
        const owner = object.attributes.owner_of;
      const userQuery = new Moralis.Query(Moralis.User);
        userQuery.equalTo("accounts", owner);
      const userObject = await userQuery.first({useMasterKey:true});
      if (userObject){
          request.object.set('user', userObject);
      }
      request.object.set('token', object);
    }
  });
  
  Moralis.Cloud.beforeSave("SoldItems", async (request) => {
  
    const query = new Moralis.Query("ItemsForSale");
    query.equalTo("uid", request.object.get('uid'));
    const item = await query.first();
    if (item){
      request.object.set('item', item);
      item.set('isSold', true);
      await item.save();
      
    
      const userQuery = new Moralis.Query(Moralis.User);
        userQuery.equalTo("accounts", request.object.get('buyer'));
      const userObject = await userQuery.first({useMasterKey:true});
      if (userObject){
          request.object.set('user', userObject);
      }
    }
  });    
  
  Moralis.Cloud.define("getItems", async (request) => {
      
    const query = new Moralis.Query("ItemsForSale");
    query.notEqualTo("isSold", true);
    query.select("uid","askingPrice","tokenAddress","tokenId", "token.token_uri", "token.symbol","token.owner_of","token.id", "user.avatar","user.username");
  
    const queryResults = await query.find({useMasterKey:true});
    const results = [];
  
    for (let i = 0; i < queryResults.length; ++i) {
  
      if (!queryResults[i].attributes.token || !queryResults[i].attributes.user) continue;
  
      results.push({
        "uid": queryResults[i].attributes.uid,
        "tokenId": queryResults[i].attributes.tokenId,
        "tokenAddress": queryResults[i].attributes.tokenAddress,
        "askingPrice": queryResults[i].attributes.askingPrice,
  
        "symbol": queryResults[i].attributes.token.attributes.symbol,
        "tokenUri": queryResults[i].attributes.token.attributes.token_uri,
        "ownerOf": queryResults[i].attributes.token.attributes.owner_of,
        "tokenObjectId": queryResults[i].attributes.token.id,
        
        "sellerUsername": queryResults[i].attributes.user.attributes.username,
        "sellerAvatar": queryResults[i].attributes.user.attributes.avatar,
      });
    }
  
    return results;
  });
  
  Moralis.Cloud.define("getItem", async (request) => {
      
    const query = new Moralis.Query("ItemsForSale");
    query.equalTo("uid", request.params.uid);
    query.select("uid","askingPrice","tokenAddress","tokenId", "token.token_uri", "token.symbol","token.owner_of","token.id","user.avatar","user.username");
  
    const queryResult = await query.first({useMasterKey:true});
    if (!queryResult) return;
    return {
        "uid": queryResult.attributes.uid,
        "tokenId": queryResult.attributes.tokenId,
        "tokenAddress": queryResult.attributes.tokenAddress,
        "askingPrice": queryResult.attributes.askingPrice,
  
        "symbol": queryResult.attributes.token.attributes.symbol,
        "tokenUri": queryResult.attributes.token.attributes.token_uri,
        "ownerOf": queryResult.attributes.token.attributes.owner_of,
        "tokenObjectId": queryResult.attributes.token.id,
  
        "sellerUsername": queryResult.attributes.user.attributes.username,
        "sellerAvatar": queryResult.attributes.user.attributes.avatar,
      };
  });