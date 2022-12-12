using System;
using System.ComponentModel;

namespace Cressem.Util.Helpers
{
	/// <summary>
	/// 
	/// </summary>
	public static class EnumHelper
	{
		/// <summary>
		/// Converts a <see cref="System.String"/> to an <see cref="System.Enum"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="ignoreCase"></param>
		/// <returns></returns>
		/// <example>
		/// <![CDATA[
		/// MachineTypeEnum machineType = EnumHelper.ParseEnum<MachineTypeEnum>("AOI");
		/// ]]>
		/// </example>
		public static T ParseEnum<T>(string value, bool ignoreCase = true)
		{
			return (T)Enum.Parse(typeof(T), value, ignoreCase);
		}

		/// <summary>
		/// Gets description attribute of an enumerator item if description is defined.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="optionValue"></param>
		/// <returns></returns>
		public static string GetDescription<T>(T optionValue)
		{
			var optionDescription = optionValue.ToString();
			var optionInfo = typeof(T).GetField(optionDescription);

			if (Attribute.IsDefined(optionInfo, typeof(DescriptionAttribute)))
			{
				var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(optionInfo, typeof(DescriptionAttribute));
				return attribute.Description;
			}

			return optionDescription;
		}
	}
}
