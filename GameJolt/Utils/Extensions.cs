using System;
using System.Linq;

namespace GameJolt.Utils {
	public static class Extensions {
		public static T[] ArraySelect<T>(this JSONNode node, Func<JSONNode, T> selector) {
			return node.AsArray.Childs.Select(selector).ToArray();
		}

		public static T ThrowIfNull<T>(this T obj) where T : class {
			if(obj == null) throw new ArgumentNullException();
			return obj;
		}

		public static string ThrowIfNullOrEmpty(this string obj) {
			if(obj == null) throw new ArgumentNullException();
			if(obj == string.Empty) throw new ArgumentException("String must not be empty.");
			return obj;
		}

		public static T[] ThrowIfNullOrEmpty<T>(this T[] obj) {
			if(obj == null) throw new ArgumentNullException();
			if(obj.Length == 0) throw new ArgumentException("Array must not be empty.");
			return obj;
		}
	}
}
