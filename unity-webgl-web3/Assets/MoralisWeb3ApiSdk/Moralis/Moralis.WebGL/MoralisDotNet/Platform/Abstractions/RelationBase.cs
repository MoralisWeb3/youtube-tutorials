//using Moralis.Platform.Objects;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Moralis.Platform.Abstractions
//{
//    [EditorBrowsable(EditorBrowsableState.Never)]
//    public abstract class RelationBase : IJsonConvertible
//    {
//        MoralisObject Parent { get; set; }

//        string Key { get; set; }

//        internal ParseRelationBase(MoralisObject parent, string key) => EnsureParentAndKey(parent, key);

//        internal ParseRelationBase(MoralisObject parent, string key, string targetClassName) : this(parent, key) => TargetClassName = targetClassName;

//        internal void EnsureParentAndKey(MoralisObject parent, string key)
//        {
//            Parent ??= parent;
//            Key ??= key;

//            Debug.Assert(Parent == parent, "Relation retrieved from two different objects");
//            Debug.Assert(Key == key, "Relation retrieved from two different keys");
//        }

//        internal void Add(MoralisObject entity)
//        {
//            ParseRelationOperation change = new ParseRelationOperation(Parent.Services.ClassController, new[] { entity }, default);

//            Parent.PerformOperation(Key, change);
//            TargetClassName = change.TargetClassName;
//        }

//        internal void Remove(ParseObject entity)
//        {
//            ParseRelationOperation change = new ParseRelationOperation(Parent.Services.ClassController, default, new[] { entity });

//            Parent.PerformOperation(Key, change);
//            TargetClassName = change.TargetClassName;
//        }

//        IDictionary<string, object> IJsonConvertible.ConvertToJSON() => new Dictionary<string, object>
//        {
//            ["__type"] = "Relation",
//            ["className"] = TargetClassName
//        };

//        internal MoralisQuery<T> GetQuery<T>() where T : MoralisQuery => TargetClassName is { } ? new MoralisQuery<T>(Parent.Services, TargetClassName).WhereRelatedTo(Parent, Key) : new ParseQuery<T>(Parent.Services, Parent.ClassName).RedirectClassName(Key).WhereRelatedTo(Parent, Key);

//        internal string TargetClassName { get; set; }
//    }
//}
