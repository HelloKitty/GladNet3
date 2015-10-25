using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common.Tests
{
	public class EnumRangeAttribute : ValuesAttribute
	{
		public EnumRangeAttribute(Type enumType, params object[] excludes)
		{
			if (!enumType.IsEnum)
				throw new ArgumentException("Type must be Enum.", "enumType");

			if (excludes != null)
			{
				//Check Type of excludes
				foreach (var e in excludes)
					if (!Enum.IsDefined(enumType, e))
						throw new ArgumentException("All excludes must be valid Enum values.", "excludes");

				//Now we remove the values.
				data = Enum.GetValues(enumType).Cast<object>().Where(x => !excludes.Contains(x)).Cast<object>().ToArray();
			}
			else
				data = Enum.GetValues(enumType).Cast<object>().ToArray();
		}
	}
}
