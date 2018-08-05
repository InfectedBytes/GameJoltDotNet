using System;

namespace GameJolt.Utils {
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
	internal sealed class ExcludeFromCodeCoverageAttribute : Attribute { }
}
