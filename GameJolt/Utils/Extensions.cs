using System;
using System.Linq;

namespace GameJolt.Utils {
	/// <summary>
	/// Utility extension methods used to simplify often used functionality.
	/// </summary>
	internal static class Extensions {
		/// <summary>
		/// Shorthand function used to select all childnodes of an json array.
		/// </summary>
		/// <typeparam name="T">The type of the objects to select.</typeparam>
		/// <param name="node">The json node which must be an array.</param>
		/// <param name="selector">The selector function.</param>
		/// <returns></returns>
		public static T[] ArraySelect<T>(this JSONNode node, Func<JSONNode, T> selector) {
			return node.AsArray.Childs.Select(selector).ToArray();
		}

		#region Throw Utils
		/// <summary>
		/// Throws an <see cref="ArgumentNullException"/> when the provided argument is null.
		/// </summary>
		/// <typeparam name="T">Type of the object to test.</typeparam>
		/// <param name="obj">The object which should not be null.</param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public static T ThrowIfNull<T>(this T obj) where T : class {
			if(obj == null) throw new ArgumentNullException();
			return obj;
		}

		/// <summary>
		/// Throws an <see cref="ArgumentNullException"/> when the provided string is null.
		/// Throws an <see cref="ArgumentException"/> when the provided string is empty.
		/// </summary>
		/// <param name="obj">The string which should neither be null or empty.</param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public static string ThrowIfNullOrEmpty(this string obj) {
			if(obj == null) throw new ArgumentNullException();
			if(obj == string.Empty) throw new ArgumentException("String must not be empty.");
			return obj;
		}

		/// <summary>
		/// Throws an <see cref="ArgumentNullException"/> when the provided array is null.
		/// Throws an <see cref="ArgumentException"/> when the provided array is empty.
		/// </summary>
		/// <typeparam name="T">The type of the array.</typeparam>
		/// <param name="obj">The array which should neither be null or empty.</param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public static T[] ThrowIfNullOrEmpty<T>(this T[] obj) {
			if(obj == null) throw new ArgumentNullException();
			if(obj.Length == 0) throw new ArgumentException("Array must not be empty.");
			return obj;
		}
		#endregion
	}
}
