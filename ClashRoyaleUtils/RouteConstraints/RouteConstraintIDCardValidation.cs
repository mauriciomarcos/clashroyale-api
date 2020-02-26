using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Linq;

namespace ClashRoyaleUtils.RouteConstraints
{
    public class RouteConstraintIDCardValidation : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var paramValueId = values.FirstOrDefault(v => v.Key == routeKey).Value.ToString();

            if (paramValueId.Length < 8) return false;

            if (!paramValueId.Substring(0, 1).Equals(2.ToString())) return false;

            return true;
        }
    }
}
