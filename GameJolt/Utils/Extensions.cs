using System;
using System.Linq;

namespace GameJolt.Utils {
	public static class Extensions {
		public static T[] ArraySelect<T>(this JSONNode node, Func<JSONNode, T> selector) {
			return node.AsArray.Childs.Select(selector).ToArray();
		}
	}
}
