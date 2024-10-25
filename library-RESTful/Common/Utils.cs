using library_RESTful.Data;
using library_RESTful.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace library_RESTful.Common
{

	public static class Utils
	{
		public static object? GetAnonymousProperty(this object obj, string propName)
		{
			var type = obj.GetType();
			var prop = type.GetProperty(propName);
			object? val = prop?.GetValue(obj);
			return val;
		}
	}
}
