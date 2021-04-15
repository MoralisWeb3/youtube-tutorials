import Moralis from "moralis";

Moralis.Cloud.define("searchEthAddress", async function(request) {
  const { address } = request.params
  if (!address) {
    return null
  }

  // find out if address is already watched
  const query = new Moralis.Query("WatchedEthAddress");
  query.equalTo("address", address)
  const watchCount = await query.count();
  
  if (watchCount > 0) {
    // already watched don't sync again
    return null;
  }
  
  return Moralis.Cloud.run("watchEthAddress", {address});
});

Moralis.Cloud.define("getTransactions", function(request) {
  const {userAddress, pageSize, pageNum } = request.params;
  const offset = (pageNum - 1) * pageSize;
  
  const query = new Moralis.Query("EthTransactions");
  query.equalTo("from_address", userAddress);
  query.descending("block_number");
  query.withCount();
  query.skip(offset)
  query.limit(pageSize);

  return query.find();
});

Moralis.Cloud.define("getTokenTranfers", async (request) => {
  const {userAddress, pageSize, pageNum } = request.params;
  const offset = (pageNum - 1) * pageSize;
  const output = {
    results: [],
    count: 0,
  };

  // count results
  const matchPipeline = {
    match: {
      $expr: {
        $or: [
          { $eq: ["$from_address", userAddress] },
          { $eq: ["$to_address", userAddress] },
        ],
      },
    },
    sort: { block_number: -1 },
    count: "count",
  };
  const query = new Moralis.Query("EthTokenTransfers");
  const countResult = await query.aggregate(matchPipeline);
  output.count = countResult[0].count;

  // get page results
  const lookupPipeline = {
    ...matchPipeline,
    skip: offset,
    limit: pageSize,
    lookup: {
      from: "EthTokenBalance",
      let: { tokenAddress: "$token_address", userAddress },
      pipeline: [
        {
          $match: {
            $expr: {
              $and: [
                { $eq: ["$token_address", "$$tokenAddress"] },
                { $eq: ["$address", "$$userAddress"] },
              ],
            },
          },
        },
      ],
      as: "EthTokenBalance",
    },
    unwind: "$EthTokenBalance",
  };
  delete lookupPipeline.count;

  output.results = await query.aggregate(lookupPipeline);
  return output;
});
