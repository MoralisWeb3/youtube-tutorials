using System;
using System.Collections.Generic;
using System.Linq;
using Moralis.WebGL.Platform.Abstractions;
using Newtonsoft.Json;

namespace Moralis.WebGL.Platform.Objects
{
    class MoralisAclJsonConvertor : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //MyCustomType myCustomType = new MyCustomType();//for null values        
            Dictionary<string, object> acl = new Dictionary<string, object>();
            string key = String.Empty;

            while (reader.Read())
            {
                var tokenType = reader.TokenType;
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    key = (reader.Value as string) ?? string.Empty;
                }
                else if (reader.TokenType == JsonToken.StartObject)
                {
                    Dictionary<string, object> cntlDict = new Dictionary<string, object>();
                    var cntlKey = string.Empty;

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.PropertyName)
                        {
                            cntlKey = (reader.Value as string) ?? string.Empty;
                        }
                        else if (reader.TokenType == JsonToken.Boolean)
                        {
                            bool? b = reader.Value as bool?;
                            cntlDict.Add(cntlKey, b.Value);
                        }
                        if (reader.TokenType == JsonToken.EndObject)
                        {
                            acl.Add(key, cntlDict);
                            break;
                        }
                    }
                }
                else if (reader.TokenType == JsonToken.EndObject)
                {
                    break;
                }
            }

            return new MoralisAcl(acl);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null || !(value is MoralisAcl))
            {
                serializer.Serialize(writer, null);
                return;
            }

            Dictionary<string, object> acl = ((MoralisAcl)value).ToParameterDictionary();

            serializer.Serialize(writer, acl);
        }
    }


    [JsonConverter(typeof(MoralisAclJsonConvertor))]
    public class MoralisAcl
    {
        private enum AccessKind
        {
            Read,
            Write
        }
        private const string publicName = "*";
        private readonly ICollection<string> readers = new HashSet<string>();
        private readonly ICollection<string> writers = new HashSet<string>();

        internal MoralisAcl(IDictionary<string, object> jsonObject)
        {
            readers = new HashSet<string>(from pair in jsonObject
                                          where ((IDictionary<string, object>)pair.Value).ContainsKey("read")
                                          select pair.Key);
            writers = new HashSet<string>(from pair in jsonObject
                                          where ((IDictionary<string, object>)pair.Value).ContainsKey("write")
                                          select pair.Key);
        }

        /// <summary>
        /// Creates an ACL with no permissions granted.
        /// </summary>
        public MoralisAcl()
        {
        }

        /// <summary>
        /// Creates an ACL where only the provided user has access.
        /// </summary>
        /// <param name="owner">The only user that can read or write objects governed by this ACL.</param>
        public MoralisAcl(MoralisUser owner)
        {
            SetReadAccess(owner, true);
            SetWriteAccess(owner, true);
        }

        public Dictionary<string, object> ToParameterDictionary()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (string user in readers.Union(writers))
            {
                Dictionary<string, object> userPermissions = new Dictionary<string, object>();
                if (readers.Contains(user))
                {
                    userPermissions["read"] = true;
                }
                if (writers.Contains(user))
                {
                    userPermissions["write"] = true;
                }
                result[user] = userPermissions;
            }
            return result;
        }

        private void SetAccess(AccessKind kind, string userId, bool allowed)
        {
            if (userId == null)
            {
                throw new ArgumentException("Cannot set access for an unsaved user or role.");
            }
            ICollection<string> target = null;
            switch (kind)
            {
                case AccessKind.Read:
                    target = readers;
                    break;
                case AccessKind.Write:
                    target = writers;
                    break;
                default:
                    throw new NotImplementedException("Unknown AccessKind");
            }
            if (allowed)
            {
                target.Add(userId);
            }
            else
            {
                target.Remove(userId);
            }
        }

        private bool GetAccess(AccessKind kind, string userId)
        {
            if (userId == null)
            {
                throw new ArgumentException("Cannot get access for an unsaved user or role.");
            }
            switch (kind)
            {
                case AccessKind.Read:
                    return readers.Contains(userId);
                case AccessKind.Write:
                    return writers.Contains(userId);
                default:
                    throw new NotImplementedException("Unknown AccessKind");
            }
        }

        /// <summary>
        /// Gets or sets whether the public is allowed to read this object.
        /// </summary>
        public bool PublicReadAccess
        {
            get => GetAccess(AccessKind.Read, publicName);
            set => SetAccess(AccessKind.Read, publicName, value);
        }

        /// <summary>
        /// Gets or sets whether the public is allowed to write this object.
        /// </summary>
        public bool PublicWriteAccess
        {
            get => GetAccess(AccessKind.Write, publicName);
            set => SetAccess(AccessKind.Write, publicName, value);
        }

        /// <summary>
        /// Sets whether the given user id is allowed to read this object.
        /// </summary>
        /// <param name="userId">The objectId of the user.</param>
        /// <param name="allowed">Whether the user has permission.</param>
        public void SetReadAccess(string userId, bool allowed) => SetAccess(AccessKind.Read, userId, allowed);

        /// <summary>
        /// Sets whether the given user is allowed to read this object.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="allowed">Whether the user has permission.</param>
        public void SetReadAccess(MoralisUser user, bool allowed) => SetReadAccess(user.objectId, allowed);

        /// <summary>
        /// Sets whether the given user id is allowed to write this object.
        /// </summary>
        /// <param name="userId">The objectId of the user.</param>
        /// <param name="allowed">Whether the user has permission.</param>
        public void SetWriteAccess(string userId, bool allowed) => SetAccess(AccessKind.Write, userId, allowed);

        /// <summary>
        /// Sets whether the given user is allowed to write this object.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="allowed">Whether the user has permission.</param>
        public void SetWriteAccess(MoralisUser user, bool allowed) => SetWriteAccess(user.objectId, allowed);

        /// <summary>
        /// Gets whether the given user id is *explicitly* allowed to read this object.
        /// Even if this returns false, the user may still be able to read it if
        /// PublicReadAccess is true or a role that the user belongs to has read access.
        /// </summary>
        /// <param name="userId">The user objectId to check.</param>
        /// <returns>Whether the user has access.</returns>
        public bool GetReadAccess(string userId) => GetAccess(AccessKind.Read, userId);

        /// <summary>
        /// Gets whether the given user is *explicitly* allowed to read this object.
        /// Even if this returns false, the user may still be able to read it if
        /// PublicReadAccess is true or a role that the user belongs to has read access.
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <returns>Whether the user has access.</returns>
        public bool GetReadAccess(MoralisUser user) => GetReadAccess(user.objectId);

        /// <summary>
        /// Gets whether the given user id is *explicitly* allowed to write this object.
        /// Even if this returns false, the user may still be able to write it if
        /// PublicReadAccess is true or a role that the user belongs to has write access.
        /// </summary>
        /// <param name="userId">The user objectId to check.</param>
        /// <returns>Whether the user has access.</returns>
        public bool GetWriteAccess(string userId) => GetAccess(AccessKind.Write, userId);

        /// <summary>
        /// Gets whether the given user is *explicitly* allowed to write this object.
        /// Even if this returns false, the user may still be able to write it if
        /// PublicReadAccess is true or a role that the user belongs to has write access.
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <returns>Whether the user has access.</returns>
        public bool GetWriteAccess(MoralisUser user) => GetWriteAccess(user.objectId);

        /// <summary>
        /// Sets whether users belonging to the role with the given <paramref name="roleName"/>
        /// are allowed to read this object.
        /// </summary>
        /// <param name="roleName">The name of the role.</param>
        /// <param name="allowed">Whether the role has access.</param>
        public void SetRoleReadAccess(string roleName, bool allowed) => SetAccess(AccessKind.Read, "role:" + roleName, allowed);

        /// <summary>
        /// Sets whether users belonging to the given role are allowed to read this object.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="allowed">Whether the role has access.</param>
        public void SetRoleReadAccess(MoralisRole role, bool allowed) => SetRoleReadAccess(role.name, allowed);

        /// <summary>
        /// Gets whether users belonging to the role with the given <paramref name="roleName"/>
        /// are allowed to read this object. Even if this returns false, the role may still be
        /// able to read it if a parent role has read access.
        /// </summary>
        /// <param name="roleName">The name of the role.</param>
        /// <returns>Whether the role has access.</returns>
        public bool GetRoleReadAccess(string roleName) => GetAccess(AccessKind.Read, "role:" + roleName);

        /// <summary>
        /// Gets whether users belonging to the role are allowed to read this object.
        /// Even if this returns false, the role may still be able to read it if a
        /// parent role has read access.
        /// </summary>
        /// <param name="role">The name of the role.</param>
        /// <returns>Whether the role has access.</returns>
        public bool GetRoleReadAccess(MoralisRole role) => GetRoleReadAccess(role.name);

        /// <summary>
        /// Sets whether users belonging to the role with the given <paramref name="roleName"/>
        /// are allowed to write this object.
        /// </summary>
        /// <param name="roleName">The name of the role.</param>
        /// <param name="allowed">Whether the role has access.</param>
        public void SetRoleWriteAccess(string roleName, bool allowed) => SetAccess(AccessKind.Write, "role:" + roleName, allowed);

        /// <summary>
        /// Sets whether users belonging to the given role are allowed to write this object.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="allowed">Whether the role has access.</param>
        public void SetRoleWriteAccess(MoralisRole role, bool allowed) => SetRoleWriteAccess(role.name, allowed);

        /// <summary>
        /// Gets whether users belonging to the role with the given <paramref name="roleName"/>
        /// are allowed to write this object. Even if this returns false, the role may still be
        /// able to write it if a parent role has write access.
        /// </summary>
        /// <param name="roleName">The name of the role.</param>
        /// <returns>Whether the role has access.</returns>
        public bool GetRoleWriteAccess(string roleName) => GetAccess(AccessKind.Write, "role:" + roleName);

        /// <summary>
        /// Gets whether users belonging to the role are allowed to write this object.
        /// Even if this returns false, the role may still be able to write it if a
        /// parent role has write access.
        /// </summary>
        /// <param name="role">The name of the role.</param>
        /// <returns>Whether the role has access.</returns>
        public bool GetRoleWriteAccess(MoralisRole role) => GetRoleWriteAccess(role.name);
    }
}
