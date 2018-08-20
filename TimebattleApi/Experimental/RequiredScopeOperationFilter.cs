using System;
using System.Collections.Generic;
using System.Linq;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TimebattleApi.Experimental
{
	public class RequiredScopeOperationFilter : IOperationFilter
	{
		public void Apply(Operation operation, OperationFilterContext context)
		{
			var authAttributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
				.Union(context.MethodInfo.GetCustomAttributes(true))
				.OfType<RequiredScopeAttribute>().ToList();
			if (!authAttributes.Any()) return;

			if (operation.Security == null) operation.Security = new List<IDictionary<String, IEnumerable<String>>>();
			operation.Security.Add(new Dictionary<String, IEnumerable<String>>
			{
				{"required-scope", authAttributes.SelectMany(_ => _.RequiredScopes)}
			});
		}
	}
}