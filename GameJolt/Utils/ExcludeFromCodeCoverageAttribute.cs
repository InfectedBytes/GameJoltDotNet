using System;

namespace GameJolt.Utils {
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
	public sealed class ExcludeFromCodeCoverageAttribute : Attribute { }
}
