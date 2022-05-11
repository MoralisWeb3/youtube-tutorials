using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Moralis.Platform.Abstractions;
using Moralis.Platform.Exceptions;
using Moralis.Platform.Objects;
using Moralis.Platform.Utilities;

namespace Moralis.Platform.Queries
{
    public class MoralisQuery<T> where T : MoralisObject
    {
        /// <summary>
        /// Serialized <see langword="where"/> clauses.
        /// </summary>
        Dictionary<string, object> Filters { get; }

        /// <summary>
        /// Serialized <see langword="orderby"/> clauses.
        /// </summary>
        ReadOnlyCollection<string> Orderings { get; }

        /// <summary>
        /// Serialized related data query merging request (data inclusion) clauses.
        /// </summary>
        ReadOnlyCollection<string> Includes { get; }

        /// <summary>
        /// Serialized key selections.
        /// </summary>
        ReadOnlyCollection<string> KeySelections { get; }

        string RedirectClassNameForKey { get; }

        int? SkipAmount { get; }

        int? LimitAmount { get; }

        internal string ClassName { get; }

        internal string SessionToken { get; }

        internal IQueryService QueryService { get; }

        internal IInstallationService InstallationService { get; }

        internal IServerConnectionData ServerConnectionData { get; }

        internal IJsonSerializer JsonSerializer { get; }

        /// <summary>
        /// Private constructor for composition of queries. A source query is required,
        /// but the remaining values can be null if they won't be changed in this
        /// composition.
        /// </summary>
        internal MoralisQuery(MoralisQuery<T> source, IDictionary<string, object> where = null, IEnumerable<string> replacementOrderBy = null, IEnumerable<string> thenBy = null, int? skip = null, int? limit = null, IEnumerable<string> includes = null, IEnumerable<string> selectedKeys = null, string redirectClassNameForKey = null)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            QueryService = source.QueryService;
            SessionToken = source.SessionToken;
            ClassName = source.ClassName;
            Filters = source.Filters;
            Orderings = replacementOrderBy is null ? source.Orderings : new ReadOnlyCollection<string>(replacementOrderBy.ToList());
            InstallationService = source.InstallationService;
            ServerConnectionData = source.ServerConnectionData;
            JsonSerializer = source.JsonSerializer;

            SkipAmount = skip is null ? source.SkipAmount : (source.SkipAmount ?? 0) + skip;
            LimitAmount = limit ?? source.LimitAmount;
            Includes = source.Includes;
            KeySelections = source.KeySelections;
            RedirectClassNameForKey = redirectClassNameForKey ?? source.RedirectClassNameForKey;

            if (thenBy is { })
            {
                List<string> newOrderBy = new List<string>(Orderings ?? throw new ArgumentException("You must call OrderBy before calling ThenBy."));

                newOrderBy.AddRange(thenBy);
                Orderings = new ReadOnlyCollection<string>(newOrderBy);
            }

            // Remove duplicates.

            if (Orderings is { })
            {
                Orderings = new ReadOnlyCollection<string>(new HashSet<string>(Orderings).ToList());
            }

            if (where is { })
            {
                Filters = new Dictionary<string, object>(MergeWhereClauses(where));
            }
            else
            {
                Filters = new Dictionary<string, object>();
            }

            if (includes is { })
            {
                Includes = new ReadOnlyCollection<string>(MergeIncludes(includes).ToList());
            }

            if (selectedKeys is { })
            {
                KeySelections = new ReadOnlyCollection<string>(MergeSelectedKeys(selectedKeys).ToList());
            }
        }

        HashSet<string> MergeIncludes(IEnumerable<string> includes)
        {
            if (Includes is null)
            {
                return new HashSet<string>(includes);
            }

            HashSet<string> newIncludes = new HashSet<string>(Includes);

            foreach (string item in includes)
            {
                newIncludes.Add(item);
            }

            return newIncludes;
        }

        HashSet<string> MergeSelectedKeys(IEnumerable<string> selectedKeys) => new HashSet<string>((KeySelections ?? Enumerable.Empty<string>()).Concat(selectedKeys));

        IDictionary<string, object> MergeWhereClauses(IDictionary<string, object> where)
        {
            if (Filters is null)
            {
                return where;
            }

            Dictionary<string, object> newWhere = new Dictionary<string, object>(Filters);
            foreach (KeyValuePair<string, object> pair in where)
            {
                if (newWhere.ContainsKey(pair.Key))
                {
                    if (!(newWhere[pair.Key] is IDictionary<string, object> oldCondition) || !(pair.Value is IDictionary<string, object> condition))
                    {
                        throw new ArgumentException("More than one where clause for the given key provided.");
                    }

                    Dictionary<string, object> newCondition = new Dictionary<string, object>(oldCondition);
                    foreach (KeyValuePair<string, object> conditionPair in condition)
                    {
                        if (newCondition.ContainsKey(conditionPair.Key))
                        {
                            throw new ArgumentException("More than one condition for the given key provided.");
                        }

                        newCondition[conditionPair.Key] = conditionPair.Value;
                    }

                    newWhere[pair.Key] = newCondition;
                }
                else
                {
                    newWhere[pair.Key] = pair.Value;
                }
            }
            return newWhere;
        }

        /// <summary>
        /// Constructs a query based upon the MoralisObject subclass used as the generic parameter for the MoralisQuery.
        /// </summary>
        public MoralisQuery(IQueryService queryService, IInstallationService installationService, IServerConnectionData connectionData, IJsonSerializer jsonSerializer, string sessionToken) : this(queryService, installationService, connectionData, jsonSerializer, sessionToken, typeof(T).Name) { }

        /// <summary>
        /// Constructs a query. A default query with no further parameters will retrieve
        /// all <see cref="MoralisObject"/>s of the provided class.
        /// </summary>
        /// <param name="className">The name of the class to retrieve MoralisObjects for.</param>
        public MoralisQuery(IQueryService queryService, IInstallationService installationService, IServerConnectionData connectionData, IJsonSerializer jsonSerializer, string sessionToken, string className) => (ClassName, QueryService, InstallationService, ServerConnectionData, SessionToken, JsonSerializer) = (className ?? throw new ArgumentNullException(nameof(className), "Must specify a MoralisObject class name when creating a MoralisQuery."), queryService, installationService, connectionData, sessionToken, jsonSerializer);

        #region Order By

        /// <summary>
        /// Sorts the results in ascending order by the given key.
        /// This will override any existing ordering for the query.
        /// </summary>
        /// <param name="key">The key to order by.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> OrderBy(string key) => new MoralisQuery<T>(this, replacementOrderBy: new List<string> { key });

        /// <summary>
        /// Sorts the results in descending order by the given key.
        /// This will override any existing ordering for the query.
        /// </summary>
        /// <param name="key">The key to order by.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> OrderByDescending(string key) => new MoralisQuery<T>(this, replacementOrderBy: new List<string> { "-" + key });

        /// <summary>
        /// Sorts the results in ascending order by the given key, after previous
        /// ordering has been applied.
        ///
        /// This method can only be called if there is already an <see cref="OrderBy"/>
        /// or <see cref="OrderByDescending"/>
        /// on this query.
        /// </summary>
        /// <param name="key">The key to order by.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> ThenBy(string key) => new MoralisQuery<T>(this, thenBy: new List<string> { key });

        /// <summary>
        /// Sorts the results in descending order by the given key, after previous
        /// ordering has been applied.
        ///
        /// This method can only be called if there is already an <see cref="OrderBy"/>
        /// or <see cref="OrderByDescending"/> on this query.
        /// </summary>
        /// <param name="key">The key to order by.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> ThenByDescending(string key) => new MoralisQuery<T>(this, thenBy: new List<string> { "-" + key });

        #endregion

        /// <summary>
        /// Include nested MoralisObjects for the provided key. You can use dot notation
        /// to specify which fields in the included objects should also be fetched.
        /// </summary>
        /// <param name="key">The key that should be included.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> Include(string key) => new MoralisQuery<T>(this, includes: new List<string> { key });

        /// <summary>
        /// Restrict the fields of returned MoralisObjects to only include the provided key.
        /// If this is called multiple times, then all of the keys specified in each of
        /// the calls will be included.
        /// </summary>
        /// <param name="key">The key that should be included.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> Select(string key) => new MoralisQuery<T>(this, selectedKeys: new List<string> { key });

        /// <summary>
        /// Skips a number of results before returning. This is useful for pagination
        /// of large queries. Chaining multiple skips together will cause more results
        /// to be skipped.
        /// </summary>
        /// <param name="count">The number of results to skip.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> Skip(int count) => new MoralisQuery<T>(this, skip: count);

        /// <summary>
        /// Controls the maximum number of results that are returned. Setting a negative
        /// limit denotes retrieval without a limit. Chaining multiple limits
        /// results in the last limit specified being used. The default limit is
        /// 100, with a maximum of 1000 results being returned at a time.
        /// </summary>
        /// <param name="count">The maximum number of results to return.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> Limit(int count) => new MoralisQuery<T>(this, limit: count);

        internal MoralisQuery<T> RedirectClassName(string key) => new MoralisQuery<T>(this, redirectClassNameForKey: key);

        #region Where

        /// <summary>
        /// Adds a constraint to the query that requires a particular key's value to be
        /// contained in the provided list of values.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <param name="values">The values that will match.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> WhereContainedIn<TIn>(string key, IEnumerable<TIn> values) => new MoralisQuery<T>(this, where: new Dictionary<string, object> { { key, new Dictionary<string, object> { { "$in", values.ToList() } } } });

        /// <summary>
        /// Add a constraint to the querey that requires a particular key's value to be
        /// a list containing all of the elements in the provided list of values.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <param name="values">The values that will match.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> WhereContainsAll<TIn>(string key, IEnumerable<TIn> values) => new MoralisQuery<T>(this, where: new Dictionary<string, object> { { key, new Dictionary<string, object> { { "$all", values.ToList() } } } });

        /// <summary>
        /// Adds a constraint for finding string values that contain a provided string.
        /// This will be slow for large data sets.
        /// </summary>
        /// <param name="key">The key that the string to match is stored in.</param>
        /// <param name="substring">The substring that the value must contain.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> WhereContains(string key, string substring) => new MoralisQuery<T>(this, where: new Dictionary<string, object> { { key, new Dictionary<string, object> { { "$regex", RegexQuote(substring) } } } });

        /// <summary>
        /// Adds a constraint for finding objects that do not contain a given key.
        /// </summary>
        /// <param name="key">The key that should not exist.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> WhereDoesNotExist(string key) => new MoralisQuery<T>(this, where: new Dictionary<string, object> { { key, new Dictionary<string, object> { { "$exists", false } } } });

        /// <summary>
        /// Adds a constraint to the query that requires that a particular key's value
        /// does not match another MoralisQuery. This only works on keys whose values are
        /// MoralisObjects or lists of MoralisObjects.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <param name="query">The query that the value should not match.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> WhereDoesNotMatchQuery<TOther>(string key, MoralisQuery<TOther> query) where TOther : MoralisObject => new MoralisQuery<T>(this, where: new Dictionary<string, object> { { key, new Dictionary<string, object> { { "$notInQuery", query.BuildParameters(true) } } } });

        /// <summary>
        /// Adds a constraint for finding string values that end with a provided string.
        /// This will be slow for large data sets.
        /// </summary>
        /// <param name="key">The key that the string to match is stored in.</param>
        /// <param name="suffix">The substring that the value must end with.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> WhereEndsWith(string key, string suffix) => new MoralisQuery<T>(this, where: new Dictionary<string, object> { { key, new Dictionary<string, object> { { "$regex", RegexQuote(suffix) + "$" } } } });

        /// <summary>
        /// Adds a constraint to the query that requires a particular key's value to be
        /// equal to the provided value.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <param name="value">The value that the MoralisObject must contain.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> WhereEqualTo(string key, object value) => new MoralisQuery<T>(this, where: new Dictionary<string, object> { { key, value } });

        /// <summary>
        /// Adds a constraint for finding objects that contain a given key.
        /// </summary>
        /// <param name="key">The key that should exist.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> WhereExists(string key) => new MoralisQuery<T>(this, where: new Dictionary<string, object> { { key, new Dictionary<string, object> { { "$exists", true } } } });

        /// <summary>
        /// Adds a constraint to the query that requires a particular key's value to be
        /// greater than the provided value.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <param name="value">The value that provides a lower bound.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> WhereGreaterThan(string key, object value) => new MoralisQuery<T>(this, where: new Dictionary<string, object> { { key, new Dictionary<string, object> { { "$gt", value } } } });

        /// <summary>
        /// Adds a constraint to the query that requires a particular key's value to be
        /// greater or equal to than the provided value.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <param name="value">The value that provides a lower bound.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> WhereGreaterThanOrEqualTo(string key, object value) => new MoralisQuery<T>(this, where: new Dictionary<string, object> { { key, new Dictionary<string, object> { { "$gte", value } } } });

        /// <summary>
        /// Adds a constraint to the query that requires a particular key's value to be
        /// less than the provided value.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <param name="value">The value that provides an upper bound.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> WhereLessThan(string key, object value) => new MoralisQuery<T>(this, where: new Dictionary<string, object> { { key, new Dictionary<string, object> { { "$lt", value } } } });

        /// <summary>
        /// Adds a constraint to the query that requires a particular key's value to be
        /// less than or equal to the provided value.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <param name="value">The value that provides a lower bound.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> WhereLessThanOrEqualTo(string key, object value) => new MoralisQuery<T>(this, where: new Dictionary<string, object> { { key, new Dictionary<string, object> { { "$lte", value } } } });

        /// <summary>
        /// Adds a regular expression constraint for finding string values that match the provided
        /// regular expression. This may be slow for large data sets.
        /// </summary>
        /// <param name="key">The key that the string to match is stored in.</param>
        /// <param name="regex">The regular expression pattern to match. The Regex must
        /// have the <see cref="RegexOptions.ECMAScript"/> options flag set.</param>
        /// <param name="modifiers">Any of the following supported PCRE modifiers:
        /// <code>i</code> - Case insensitive search
        /// <code>m</code> Search across multiple lines of input</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> WhereMatches(string key, Regex regex, string modifiers) => !regex.Options.HasFlag(RegexOptions.ECMAScript) ? throw new ArgumentException("Only ECMAScript-compatible regexes are supported. Please use the ECMAScript RegexOptions flag when creating your regex.") : new MoralisQuery<T>(this, where: new Dictionary<string, object> { { key, EncodeRegex(regex, modifiers) } });

        /// <summary>
        /// Adds a regular expression constraint for finding string values that match the provided
        /// regular expression. This may be slow for large data sets.
        /// </summary>
        /// <param name="key">The key that the string to match is stored in.</param>
        /// <param name="regex">The regular expression pattern to match. The Regex must
        /// have the <see cref="RegexOptions.ECMAScript"/> options flag set.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> WhereMatches(string key, Regex regex) => WhereMatches(key, regex, null);

        /// <summary>
        /// Adds a regular expression constraint for finding string values that match the provided
        /// regular expression. This may be slow for large data sets.
        /// </summary>
        /// <param name="key">The key that the string to match is stored in.</param>
        /// <param name="pattern">The PCRE regular expression pattern to match.</param>
        /// <param name="modifiers">Any of the following supported PCRE modifiers:
        /// <code>i</code> - Case insensitive search
        /// <code>m</code> Search across multiple lines of input</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> WhereMatches(string key, string pattern, string modifiers = null) => WhereMatches(key, new Regex(pattern, RegexOptions.ECMAScript), modifiers);

        /// <summary>
        /// Adds a regular expression constraint for finding string values that match the provided
        /// regular expression. This may be slow for large data sets.
        /// </summary>
        /// <param name="key">The key that the string to match is stored in.</param>
        /// <param name="pattern">The PCRE regular expression pattern to match.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> WhereMatches(string key, string pattern) => WhereMatches(key, pattern, null);

        /// <summary>
        /// Adds a constraint to the query that requires a particular key's value
        /// to match a value for a key in the results of another MoralisQuery.
        /// </summary>
        /// <param name="key">The key whose value is being checked.</param>
        /// <param name="keyInQuery">The key in the objects from the subquery to look in.</param>
        /// <param name="query">The subquery to run</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> WhereMatchesKeyInQuery<TOther>(string key, string keyInQuery, MoralisQuery<TOther> query) where TOther : MoralisObject => new MoralisQuery<T>(this, where: new Dictionary<string, object>
        {
            [key] = new Dictionary<string, object>
            {
                ["$select"] = new Dictionary<string, object>
                {
                    [nameof(query)] = query.BuildParameters(true),
                    [nameof(key)] = keyInQuery
                }
            }
        });

        /// <summary>
        /// Adds a constraint to the query that requires a particular key's value
        /// does not match any value for a key in the results of another MoralisQuery.
        /// </summary>
        /// <param name="key">The key whose value is being checked.</param>
        /// <param name="keyInQuery">The key in the objects from the subquery to look in.</param>
        /// <param name="query">The subquery to run</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> WhereDoesNotMatchesKeyInQuery<TOther>(string key, string keyInQuery, MoralisQuery<TOther> query) where TOther : MoralisObject => new MoralisQuery<T>(this, where: new Dictionary<string, object>
        {
            [key] = new Dictionary<string, object>
            {
                ["$dontSelect"] = new Dictionary<string, object>
                {
                    [nameof(query)] = query.BuildParameters(true),
                    [nameof(key)] = keyInQuery
                }
            }
        });

        /// <summary>
        /// Adds a constraint to the query that requires that a particular key's value
        /// matches another MoralisQuery. This only works on keys whose values are
        /// MoralisObjects or lists of MoralisObjects.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <param name="query">The query that the value should match.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> WhereMatchesQuery<TOther>(string key, MoralisQuery<TOther> query) where TOther : MoralisObject => new MoralisQuery<T>(this, where: new Dictionary<string, object>
        {
            [key] = new Dictionary<string, object>
            {
                ["$inQuery"] = query.BuildParameters(true)
            }
        });

        /// <summary>
        /// Adds a proximity-based constraint for finding objects with keys whose GeoPoint
        /// values are near the given point.
        /// </summary>
        /// <param name="key">The key that the MoralisGeoPoint is stored in.</param>
        /// <param name="point">The reference MoralisGeoPoint.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> WhereNear(string key, MoralisGeoPoint point) => new MoralisQuery<T>(this, where: new Dictionary<string, object>
        {
            [key] = new Dictionary<string, object>
            {
                ["$nearSphere"] = point
            }
        });

        /// <summary>
        /// Adds a constraint to the query that requires a particular key's value to be
        /// contained in the provided list of values.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <param name="values">The values that will match.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> WhereNotContainedIn<TIn>(string key, IEnumerable<TIn> values) => new MoralisQuery<T>(this, where: new Dictionary<string, object>
        {
            [key] = new Dictionary<string, object>
            {
                ["$nin"] = values.ToList()
            }
        });

        /// <summary>
        /// Adds a constraint to the query that requires a particular key's value not
        /// to be equal to the provided value.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <param name="value">The value that that must not be equalled.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> WhereNotEqualTo(string key, object value) => new MoralisQuery<T>(this, where: new Dictionary<string, object>
        {
            [key] = new Dictionary<string, object>
            {
                ["$ne"] = value
            }
        });

        /// <summary>
        /// Adds a constraint for finding string values that start with the provided string.
        /// This query will use the backend index, so it will be fast even with large data sets.
        /// </summary>
        /// <param name="key">The key that the string to match is stored in.</param>
        /// <param name="suffix">The substring that the value must start with.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> WhereStartsWith(string key, string suffix) => new MoralisQuery<T>(this, where: new Dictionary<string, object>
        {
            [key] = new Dictionary<string, object>
            {
                ["$regex"] = $"^{RegexQuote(suffix)}"
            }
        });

        /// <summary>
        /// Add a constraint to the query that requires a particular key's coordinates to be
        /// contained within a given rectangular geographic bounding box.
        /// </summary>
        /// <param name="key">The key to be constrained.</param>
        /// <param name="southwest">The lower-left inclusive corner of the box.</param>
        /// <param name="northeast">The upper-right inclusive corner of the box.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> WhereWithinGeoBox(string key, MoralisGeoPoint southwest, MoralisGeoPoint northeast) => new MoralisQuery<T>(this, where: new Dictionary<string, object>
        {
            [key] = new Dictionary<string, object>
            {
                ["$within"] = new Dictionary<string, object>
                {
                    ["$box"] = new[]
                    {
                        southwest,
                        northeast
                    }
                }
            }
        });

        /// <summary>
        /// Adds a proximity-based constraint for finding objects with keys whose GeoPoint
        /// values are near the given point and within the maximum distance given.
        /// </summary>
        /// <param name="key">The key that the MoralisGeoPoint is stored in.</param>
        /// <param name="point">The reference MoralisGeoPoint.</param>
        /// <param name="maxDistance">The maximum distance (in radians) of results to return.</param>
        /// <returns>A new query with the additional constraint.</returns>
        public MoralisQuery<T> WhereWithinDistance(string key, MoralisGeoPoint point, MoralisGeoDistance maxDistance) => new MoralisQuery<T>(WhereNear(key, point), where: new Dictionary<string, object>
        {
            [key] = new Dictionary<string, object>
            {
                ["$maxDistance"] = maxDistance.Radians
            }
        });

        internal MoralisQuery<T> WhereRelatedTo(MoralisObject parent, string key) => new MoralisQuery<T>(this, where: new Dictionary<string, object>
        {
            ["$relatedTo"] = new Dictionary<string, object>
            {
                ["object"] = parent,
                [nameof(key)] = key
            }
        });

        #endregion

        /// <summary>
        /// Executes an aggregate query and returns aggregate results
        /// </summary>
        /// <returns>The list of MoralisObjects that match this query.</returns>
        public Task<IEnumerable<T>> AggregateAsync() => AggregateAsync(CancellationToken.None);

        /// <summary>
        /// Executes an aggregate query and returns aggregate results
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The list of MoralisObjects that match this query.</returns>
        public Task<IEnumerable<T>> AggregateAsync(CancellationToken cancellationToken)
        {
            EnsureNotInstallationQuery();
            //return QueryService.AggregateAsync<T>(this, SessionToken, cancellationToken).OnSuccess(task => task.Result);
            return QueryService.AggregateAsync<T>(this, SessionToken, cancellationToken).OnSuccess(task => {
                IEnumerable<T> items = task.Result;

                foreach (T i in items)
                {
                    i.ObjectService = this.QueryService.ObjectService;
                    i.sessionToken = this.SessionToken;
                }

                return items;
            });
        }

        /// <summary>
        /// Retrieves a list of MoralisObjects that satisfy this query from Moralis.
        /// </summary>
        /// <returns>The list of MoralisObjects that match this query.</returns>
        public Task<IEnumerable<T>> FindAsync() => FindAsync(CancellationToken.None);

        /// <summary>
        /// Retrieves a list of MoralisObjects that satisfy this query from Moralis.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The list of MoralisObjects that match this query.</returns>
        public Task<IEnumerable<T>> FindAsync(CancellationToken cancellationToken)
        {
            EnsureNotInstallationQuery();
            return QueryService.FindAsync(this, SessionToken, cancellationToken).OnSuccess(task =>
            {
                IEnumerable<T> items = task.Result;

                foreach (T i in items)
                {
                    i.ObjectService = this.QueryService.ObjectService;
                    i.sessionToken = this.SessionToken;
                }

                return items;
            }); //task.Result);
        }

        /// <summary>
        /// Retrieves a list of distinct MoralisObjects that satisfy this query from Moralis.
        /// </summary>
        /// <returns>The list of MoralisObjects that match this query.</returns>
        public Task<IEnumerable<T>> DistinctAsync() => DistinctAsync(CancellationToken.None);

        /// <summary>
        /// Retrieves a list of distinct MoralisObjects that satisfy this query from Moralis.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The list of MoralisObjects that match this query.</returns>
        public Task<IEnumerable<T>> DistinctAsync(CancellationToken cancellationToken)
        {
            EnsureNotInstallationQuery();
            return QueryService.DistinctAsync(this, SessionToken, cancellationToken).OnSuccess(task => {
                IEnumerable<T> items = task.Result;

                foreach (T i in items)
                {
                    i.ObjectService = this.QueryService.ObjectService;
                    i.sessionToken = this.SessionToken;
                }

                return items;
            }); // task.Result);
        }

        /// <summary>
        /// Retrieves at most one MoralisObject that satisfies this query.
        /// </summary>
        /// <returns>A single MoralisObject that satisfies this query, or else null.</returns>
        public Task<T> FirstOrDefaultAsync() => FirstOrDefaultAsync(CancellationToken.None);

        /// <summary>
        /// Retrieves at most one MoralisObject that satisfies this query.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A single MoralisObject that satisfies this query, or else null.</returns>
        public Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken)
        {
            EnsureNotInstallationQuery();
            return QueryService.FirstAsync<T>(this, SessionToken, cancellationToken).OnSuccess(task => {
                T i = task.Result;

                i.ObjectService = this.QueryService.ObjectService;
                i.sessionToken = this.SessionToken;

                return i;
            }); // task.Result);
        }

        /// <summary>
        /// Retrieves at most one MoralisObject that satisfies this query.
        /// </summary>
        /// <returns>A single MoralisObject that satisfies this query.</returns>
        /// <exception cref="MoralisFailureException">If no results match the query.</exception>
        public Task<T> FirstAsync() => FirstAsync(CancellationToken.None);

        /// <summary>
        /// Retrieves at most one MoralisObject that satisfies this query.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A single MoralisObject that satisfies this query.</returns>
        /// <exception cref="MoralisFailureException">If no results match the query.</exception>
        public Task<T> FirstAsync(CancellationToken cancellationToken) => FirstOrDefaultAsync(cancellationToken).OnSuccess(task => task.Result);

        /// <summary>
        /// Counts the number of objects that match this query.
        /// </summary>
        /// <returns>The number of objects that match this query.</returns>
        public Task<int> CountAsync() => CountAsync(CancellationToken.None);

        /// <summary>
        /// Counts the number of objects that match this query.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of objects that match this query.</returns>
        public Task<int> CountAsync(CancellationToken cancellationToken)
        {
            EnsureNotInstallationQuery();
            return QueryService.CountAsync(this, SessionToken, cancellationToken);
        }

        /// <summary>
        /// Constructs a MoralisObject whose id is already known by fetching data
        /// from the server.
        /// </summary>
        /// <param name="objectId">ObjectId of the MoralisObject to fetch.</param>
        /// <returns>The MoralisObject for the given objectId.</returns>
        public Task<T> GetAsync(string objectId) => GetAsync(objectId, CancellationToken.None);

        /// <summary>
        /// Constructs a MoralisObject whose id is already known by fetching data
        /// from the server.
        /// </summary>
        /// <param name="objectId">ObjectId of the MoralisObject to fetch.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The MoralisObject for the given objectId.</returns>
        public Task<T> GetAsync(string objectId, CancellationToken cancellationToken)
        {
            MoralisQuery<T> singleItemQuery = new MoralisQuery<T>(QueryService, InstallationService, ServerConnectionData, JsonSerializer, SessionToken, ClassName).WhereEqualTo(nameof(objectId), objectId);
            singleItemQuery = new MoralisQuery<T>(singleItemQuery, includes: Includes, selectedKeys: KeySelections, limit: 1);
            return singleItemQuery.FindAsync(cancellationToken).OnSuccess(t => t.Result.FirstOrDefault() ?? throw new MoralisFailureException(MoralisFailureException.ErrorCode.ObjectNotFound, "Object with the given objectId not found."));
        }

        internal object GetConstraint(string key) => Filters?.GetOrDefault(key, null);

        internal IDictionary<string, object> BuildParameters(bool includeClassName = false)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            if (Filters != null)
                result["where"] = JsonSerializer.Serialize(Filters);
            else
                result["where"] = new Dictionary<string, object>();

            if (Orderings != null)
                result["order"] = String.Join(",", Orderings.ToArray());
            if (SkipAmount != null)
                result["skip"] = SkipAmount.Value;
            if (LimitAmount != null)
                result["limit"] = LimitAmount.Value;
            if (Includes != null)
                result["include"] = String.Join(",", Includes.ToArray());
            if (KeySelections != null)
                result["keys"] = String.Join(",", KeySelections.ToArray());
            if (includeClassName)
                result["className"] = ClassName;
            if (RedirectClassNameForKey != null)
                result["redirectClassNameForKey"] = RedirectClassNameForKey;
            return result;
        }

        string RegexQuote(string input) => "\\Q" + input.Replace("\\E", "\\E\\\\E\\Q") + "\\E";

        string GetRegexOptions(Regex regex, string modifiers)
        {
            string result = modifiers ?? "";
            if (regex.Options.HasFlag(RegexOptions.IgnoreCase) && !modifiers.Contains("i"))
                result += "i";
            if (regex.Options.HasFlag(RegexOptions.Multiline) && !modifiers.Contains("m"))
                result += "m";
            return result;
        }

        IDictionary<string, object> EncodeRegex(Regex regex, string modifiers)
        {
            string options = GetRegexOptions(regex, modifiers);
            Dictionary<string, object> dict = new Dictionary<string, object> { ["$regex"] = regex.ToString() };

            if (!String.IsNullOrEmpty(options))
            {
                dict["$options"] = options;
            }

            return dict;
        }

        void EnsureNotInstallationQuery()
        {
            // The MoralisInstallation class is not accessible from this project; using string literal.

            if (ClassName.Equals("_Installation"))
            {
                throw new InvalidOperationException("Cannot directly query the Installation class.");
            }
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c></returns>
        public override bool Equals(object obj) => obj == null || !(obj is MoralisQuery<T> other) ? false : Equals(ClassName, other.ClassName) && Filters.CollectionsEqual(other.Filters) && Orderings.CollectionsEqual(other.Orderings) && Includes.CollectionsEqual(other.Includes) && KeySelections.CollectionsEqual(other.KeySelections) && Equals(SkipAmount, other.SkipAmount) && Equals(LimitAmount, other.LimitAmount);

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() =>
            // TODO (richardross): Implement this.
            0;
    }
}
