using System;
using System.Linq;

namespace GameJolt.Utils {
	public static class Extensions {
		public static T[] ArraySelect<T>(this JSONNode node, Func<JSONNode, T> selector) {
			return node.AsArray.Childs.Select(selector).ToArray();
		}

		#region Throw Utils
		[ExcludeFromCodeCoverage]
		public static T ThrowIfNull<T>(this T obj) where T : class {
			if(obj == null) throw new ArgumentNullException();
			return obj;
		}

		[ExcludeFromCodeCoverage]
		public static string ThrowIfNullOrEmpty(this string obj) {
			if(obj == null) throw new ArgumentNullException();
			if(obj == string.Empty) throw new ArgumentException("String must not be empty.");
			return obj;
		}

		[ExcludeFromCodeCoverage]
		public static T[] ThrowIfNullOrEmpty<T>(this T[] obj) {
			if(obj == null) throw new ArgumentNullException();
			if(obj.Length == 0) throw new ArgumentException("Array must not be empty.");
			return obj;
		}
		#endregion
	}
}
