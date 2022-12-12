using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Cressem.Util.Converter
{
	/// <summary>
	/// This class is to converter SideId short value to string value
	/// </summary>
	[ValueConversion(typeof(Int16), typeof(String))]
	public class SideIdConverter : IValueConverter
	{
		#region IValueConverter Members

		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			short sideId = Convert.ToInt16(value);

			if (sideId == 0)
				return "Top";
			else
				return "Bot";
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
