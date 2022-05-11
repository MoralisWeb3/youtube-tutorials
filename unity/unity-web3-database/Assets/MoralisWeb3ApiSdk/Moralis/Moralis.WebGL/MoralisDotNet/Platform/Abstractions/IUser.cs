using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Moralis.WebGL.Platform.Abstractions
{
    public interface IUser
    {
        /// <summary>
        /// Get or set the user name
        /// </summary>
        //[JsonProperty("username")]
        string username { get; set; }

        /// <summary>
        /// Get or set the user password
        /// </summary>
        //[JsonProperty("password")]
        string password { get; set; }

        /// <summary>
        /// Get or set user email address.
        /// </summary>
        //[JsonProperty("email")]
        string email { get; set; }

        //[JsonProperty("sessionToken")]
        string sessionToken { get;}

        /// <summary>
        /// Gets the authData for this user.
        /// </summary>
        IDictionary<string, IDictionary<string, object>> AuthData { get; internal set; }


        /// <summary>
        /// Signs up a new user. This will create a new ParseUser on the server and will also persist the
        /// session on disk so that you can access the user using <see cref="CurrentUser"/>. A username and
        /// password must be set before calling SignUpAsync.
        /// </summary>
        UniTask SignUpAsync();

        /// <summary>
        /// Signs up a new user. This will create a new ParseUser on the server and will also persist the
        /// session on disk so that you can access the user using <see cref="CurrentUser"/>. A username and
        /// password must be set before calling SignUpAsync.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        UniTask SignUpAsync(CancellationToken cancellationToken);

    }
}
