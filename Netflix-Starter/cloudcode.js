Moralis.Cloud.define("updateMyList", async (request) => {
  const addrs = request.params.addrs;
  const newFav = request.params.newFav;

  const user = Moralis.Object.extend("_User");
  const query = new Moralis.Query(user);
  query.equalTo("ethAddress", addrs);

  const data = await query.first({ useMasterKey: true });

  if (data.attributes.myList) {
    const { myList } = data.attributes;
    myList.push(newFav);
    data.set("myList", myList);
  } else {
    data.set("myList", [newFav]);
  }

  await data.save(null, { useMasterKey: true });
});

Moralis.Cloud.define("getMyList", async (request) => {
  const addrs = request.params.addrs;
  const user = Moralis.Object.extend("_User");
  const query = new Moralis.Query(user);
  query.equalTo("ethAddress", addrs);

  const data = await query.first({ useMasterKey: true });

  if (data.attributes.myList) {
    return data.attributes.myList;
  } else {
    return [];
  }
});
