using System;
using System.Text.RegularExpressions;

namespace Moralis.WebGL.Platform.Objects
{
    public class MoralisRole : MoralisObject
    {
        private static readonly Regex namePattern = new Regex("^[0-9a-zA-Z_\\- ]+$");
        private string roleName;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MoralisRole() : base("_Role") { }
        /// <summary>
        /// Constructs a new MoralisRole with the given name.
        /// </summary>
        /// <param name="name">The name of the role to create.</param>
        /// <param name="acl">The ACL for this role. Roles must have an ACL.</param>
        public MoralisRole(string rolName, MoralisAcl acl) : this()
        {
            name = roleName;
            ACL = acl;
        }

        public string name
        {
            get { return roleName; }

            set
            {
                if (objectId != null)
                {
                    throw new InvalidOperationException(
                        "A role's name can only be set before it has been saved.");
                }
                if (!namePattern.IsMatch((string)value))
                {
                    throw new ArgumentException("A role's name can only contain alphanumeric characters, _, -, and spaces.", nameof(value));
                }

                roleName = value;
            }
        }



        /// <summary>
        /// Gets the <see cref="Moralis{ParseUser}"/> for the <see cref="ParseUser"/>s that are
        /// direct children of this role. These users are granted any privileges that
        /// this role has been granted (e.g. read or write access through ACLs). You can
        /// add or remove child users from the role through this relation.
        /// </summary>
        //[JsonProperty("users")]
        //public Moralis<MoralisUser> Users => GetRelationProperty<MoralisUser>("Users");

        /// <summary>
        /// Gets the <see cref="Moralis{ParseRole}"/> for the <see cref="ParseRole"/>s that are
        /// direct children of this role. These roles' users are granted any privileges that
        /// this role has been granted (e.g. read or write access through ACLs). You can
        /// add or remove child roles from the role through this relation.
        /// </summary>
        //[JsonProperty("roles")]
        //public Moralis<MoralisRole> Roles => GetRelationProperty<MoralisRole>("Roles");
    }
}
