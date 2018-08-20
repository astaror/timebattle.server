using System;

namespace TimebattleApi.Experimental
{
	[AttributeUsage(AttributeTargets.Method)]
	public class RequiredScopeAttribute : Attribute
	{
		public String[] RequiredScopes { get; private set; }

		public RequiredScopeAttribute(String[] requiredScopes)
		{
			RequiredScopes = requiredScopes;
		}
	}
}