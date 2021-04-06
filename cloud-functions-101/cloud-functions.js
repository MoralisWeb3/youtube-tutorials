//put this code into the Moralis Cloud Functions section on the server
Moralis.Cloud.define("HelloWorld", function(request) {
  const name = request.params.name;
  const logger = Moralis.Cloud.getLogger();
  logger.info("Hello world " + name + "!");
})
