Moralis.Cloud.define("getTransactions", function(request) {
  const {userAddress, pageSize, pageNum } = request.params;
  const offset = (pageNum - 1) * pageSize;
  
  const query = new Parse.Query("EthTransactions");
  query.equalTo("from_address", userAddress);
  query.descending("block_number");
  query.skip(offset)
  query.limit(pageSize);

  return query.find();
})

Moralis.Cloud.define("HelloWorld", function(request) {
  const name = request.params.name;
  const logger = Moralis.Cloud.getLogger();
  logger.info("Hello world " + name + "!");
})
