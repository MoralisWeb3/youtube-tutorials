using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Moralis.WebGL.Platform.Abstractions;
using Moralis.WebGL.Platform.Objects;

namespace Moralis.WebGL.Platform.Queries
{
    public static class QueryServiceExtensions
    {
        public static MoralisQuery<T> GetQuery<T, TUser>(this IServiceHub<TUser> serviceHub) where T : MoralisObject where TUser : MoralisUser => new MoralisQuery<T>(serviceHub.QueryService, serviceHub.InstallationService, serviceHub.ServerConnectionData, serviceHub.JsonSerializer, serviceHub.CurrentUserService.CurrentUser.sessionToken);

        /// <summary>
        /// Constructs a query that is the and of the given queries.
        /// </summary>
        /// <typeparam name="T">The type of MoralisObject being queried.</typeparam>
        /// <param name="serviceHub"></param>
        /// <param name="source">An initial query to 'and' with additional queries.</param>
        /// <param name="queries">The list of MoralisQueries to 'and' together.</param>
        /// <returns>A query that is the and of the given queries.</returns>
        public static MoralisQuery<T> ConstructAndQuery<T, TUser>(this IServiceHub<TUser> serviceHub, MoralisQuery<T> source, params MoralisQuery<T>[] queries) where T : MoralisObject where TUser : MoralisUser => serviceHub.ConstructAndQuery(queries.Concat(new[] { source }));

        // ALTERNATE NAME: BuildOrQuery

        /// <summary>
        /// Constructs a query that is the or of the given queries.
        /// </summary>
        /// <typeparam name="T">The type of MoralisObject being queried.</typeparam>
        /// <param name="source">An initial query to 'or' with additional queries.</param>
        /// <param name="queries">The list of MoralisQueries to 'or' together.</param>
        /// <returns>A query that is the or of the given queries.</returns>
        public static MoralisQuery<T> ConstructOrQuery<T, TUser>(this IServiceHub<TUser> serviceHub, MoralisQuery<T> source, params MoralisQuery<T>[] queries) where T : MoralisObject where TUser : MoralisUser => serviceHub.ConstructOrQuery(queries.Concat(new[] { source }));
        public static MoralisQuery<T> ConstructNorQuery<T, TUser>(this IServiceHub<TUser> serviceHub, MoralisQuery<T> source, params MoralisQuery<T>[] queries) where T : MoralisObject where TUser : MoralisUser => serviceHub.ConstructNorQuery(queries.Concat(new[] { source }));

        /// <summary>
        /// Construct a query that is the and of two or more queries.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceHub"></param>
        /// <param name="queries">The list of MoralisQueries to 'and' together.</param>
        /// <returns>A MoralisQquery that is the 'and' of the passed in queries.</returns>
        public static MoralisQuery<T> ConstructAndQuery<T, TUser>(this IServiceHub<TUser> serviceHub, IEnumerable<MoralisQuery<T>> queries) where T : MoralisObject where TUser : MoralisUser
        {
            string className = default;
            List<IDictionary<string, object>> andValue = new List<IDictionary<string, object>> { };

            // We need to cast it to non-generic IEnumerable because of AOT-limitation

            IEnumerable nonGenericQueries = queries;
            foreach (object obj in nonGenericQueries)
            {
                MoralisQuery<T> query = obj as MoralisQuery<T>;

                if (className is { } && query.ClassName != className)
                {
                    throw new ArgumentException("All of the queries in an and query must be on the same class.");
                }

                className = query.ClassName;
                IDictionary<string, object> parameters = query.BuildParameters();

                if (parameters.Count == 0)
                {
                    continue;
                }

                if (!parameters.TryGetValue("where", out object where) || parameters.Count > 1)
                {
                    throw new ArgumentException("None of the queries in an and query can have non-filtering clauses");
                }

                //orValue.Add(where as IDictionary<string, object>);

                andValue.Add(serviceHub.JsonSerializer.Deserialize<IDictionary<string, object>>(where.ToString()));
                //andValue.Add(JsonConvert.DeserializeObject<IDictionary<string, object>>(where.ToString()));
            }

            return new MoralisQuery<T>(new MoralisQuery<T>(serviceHub.QueryService, serviceHub.InstallationService, serviceHub.ServerConnectionData, serviceHub.JsonSerializer, serviceHub.CurrentUserService.CurrentUser?.sessionToken, className), where: new Dictionary<string, object> { ["$and"] = andValue });
        }

        /// <summary>
        /// Constructs a query that is the or of the given queries.
        /// </summary>
        /// <param name="queries">The list of MoralisQueries to 'or' together.</param>
        /// <returns>A MoralisQquery that is the 'or' of the passed in queries.</returns>
        public static MoralisQuery<T> ConstructOrQuery<T, TUser>(this IServiceHub<TUser> serviceHub, IEnumerable<MoralisQuery<T>> queries) where T : MoralisObject where TUser : MoralisUser
        {
            string className = default;
            List<IDictionary<string, object>> orValue = new List<IDictionary<string, object>> { };

            // We need to cast it to non-generic IEnumerable because of AOT-limitation

            IEnumerable nonGenericQueries = queries;
            foreach (object obj in nonGenericQueries)
            {
                MoralisQuery<T> query = obj as MoralisQuery<T>;

                if (className is { } && query.ClassName != className)
                {
                    throw new ArgumentException("All of the queries in an or query must be on the same class.");
                }

                className = query.ClassName;
                IDictionary<string, object> parameters = query.BuildParameters();

                if (parameters.Count == 0)
                {
                    continue;
                }

                if (!parameters.TryGetValue("where", out object where) || parameters.Count > 1)
                {
                    throw new ArgumentException("None of the queries in an or query can have non-filtering clauses");
                }

                //orValue.Add(where as IDictionary<string, object>);
                orValue.Add(serviceHub.JsonSerializer.Deserialize<IDictionary<string, object>>(where.ToString()));
                //orValue.Add(JsonConvert.DeserializeObject<IDictionary<string, object>>(where.ToString()));
            }

            return new MoralisQuery<T>(new MoralisQuery<T>(serviceHub.QueryService, serviceHub.InstallationService, serviceHub.ServerConnectionData, serviceHub.JsonSerializer, serviceHub.CurrentUserService.CurrentUser.sessionToken, className), where: new Dictionary<string, object> { ["$or"] = orValue });
        }

        /// <summary>
        /// Construct a query that is the nor of two queries.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceHub"></param>
        /// <param name="queries">The list of MoralisQueries to 'nor' together.</param>
        /// <returns>A MoralisQquery that is the 'nor' of the passed in queries.</returns>
        public static MoralisQuery<T> ConstructNorQuery<T, TUser>(this IServiceHub<TUser> serviceHub, IEnumerable<MoralisQuery<T>> queries) where T : MoralisObject where TUser : MoralisUser
        {
            string className = default;
            List<IDictionary<string, object>> norValue = new List<IDictionary<string, object>> { };

            // We need to cast it to non-generic IEnumerable because of AOT-limitation

            IEnumerable nonGenericQueries = queries;
            foreach (object obj in nonGenericQueries)
            {
                MoralisQuery<T> query = obj as MoralisQuery<T>;

                if (className is { } && query.ClassName != className)
                {
                    throw new ArgumentException("All of the queries in an nor query must be on the same class.");
                }

                className = query.ClassName;
                IDictionary<string, object> parameters = query.BuildParameters();

                if (parameters.Count == 0)
                {
                    continue;
                }

                if (!parameters.TryGetValue("where", out object where) || parameters.Count > 1)
                {
                    throw new ArgumentException("None of the queries in an or query can have non-filtering clauses");
                }

                norValue.Add(serviceHub.JsonSerializer.Deserialize<IDictionary<string, object>>(where.ToString()));
            }

            return new MoralisQuery<T>(new MoralisQuery<T>(serviceHub.QueryService, serviceHub.InstallationService, serviceHub.ServerConnectionData, serviceHub.JsonSerializer, serviceHub.CurrentUserService.CurrentUser.sessionToken, className), where: new Dictionary<string, object> { ["$nor"] = norValue });
        }
    }

}
