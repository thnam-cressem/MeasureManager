using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OpticaNX;

namespace OpticaNX.Menu
{
	/// <summary>
	/// 메뉴그룹 템플릿 선택자
	/// </summary>
	public class ItemGroupTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			ItemGroup itemGroup = item as ItemGroup;

			if (itemGroup != null)
			{
				DataTemplate template = null;

				if (itemGroup.MenuTypes.Contains(MenuType.CustomerReport))
					template = App.MainWindowView.FindResource("galleryItemGroupTemplate") as DataTemplate;
				else
					template = App.MainWindowView.FindResource("itemGroupTemplate") as DataTemplate;

				return template;
			}

			return base.SelectTemplate(item, container);
		}
	}
}
