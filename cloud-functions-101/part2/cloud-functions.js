//put this code into the Moralis Cloud Functions section on the server
Moralis.Cloud.define("getTransactions", function(request) {
  const userAddress = request.params.userAddress;
  const query = new Parse.Query("EthTransactions");
  query.equalTo("from_address", userAddress);
  query.descending("block_number");
  query.limit(10);

  return query.find();
})

Moralis.Cloud.define("HelloWorld", function(request) {
  const name = request.params.name;
  const logger = Moralis.Cloud.getLogger();
  logger.info("Hello world " + name + "!");
})
