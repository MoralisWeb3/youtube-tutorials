Moralis.Cloud.afterSave("_User", function(request, response) {
  
  if (!request.original){  
    
      var user = request.object;
    
    
      var acl = new Moralis.ACL();
      acl.setPublicReadAccess(true);
      acl.setWriteAccess(user.id, true);

    
      var playerData = new Moralis.Object("PlayerData");
      playerData.set("user",user);
      playerData.set("skin",Math.floor(Math.random() * (6 - 1) + 1));
      playerData.setACL(acl);
	  playerData.save()   

  } 
});

