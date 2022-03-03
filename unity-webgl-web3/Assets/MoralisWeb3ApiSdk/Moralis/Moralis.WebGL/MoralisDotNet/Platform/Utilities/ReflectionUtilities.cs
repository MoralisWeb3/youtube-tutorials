
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Moralis.WebGL.Platform.Utilities
{
    public static class ReflectionUtilities
    {
        /// <summary>
        /// Gets all of the defined constructors that aren't static on a given <see cref="Type"/> instance.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<ConstructorInfo> GetInstanceConstructors(this Type type) => type.GetTypeInfo().DeclaredConstructors.Where(constructor => (constructor.Attributes & MethodAttributes.Static) == 0);

        /// <summary>
        /// This method helps simplify the process of getting a constructor for a type.
        /// A method like this exists in .NET but is not allowed in a Portable Class Library,
        /// so we've built our own.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        public static ConstructorInfo FindConstructor(this Type self, params Type[] parameterTypes) => self.GetConstructors().Where(constructor => constructor.GetParameters().Select(parameter => parameter.ParameterType).SequenceEqual(parameterTypes)).SingleOrDefault();

        /// <summary>
        /// Checks if a <see cref="Type"/> instance is another <see cref="Type"/> instance wrapped with <see cref="Nullable{T}"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CheckWrappedWithNullable(this Type type) => type.IsConstructedGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));

        /// <summary>
        /// Gets the value of <see cref="ParseClassNameAttribute.ClassName"/> if the type has a custom attribute of type <see cref="ParseClassNameAttribute"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        //public static string GetParseClassName(this Type type) => type.GetCustomAttribute<ParseClassNameAttribute>()?.ClassName;
    }
}
