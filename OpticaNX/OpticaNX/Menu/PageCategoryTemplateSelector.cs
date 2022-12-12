using System.Windows;
using System.Windows.Controls;

namespace OpticaNX.Menu
{
	/// <summary>
	/// 페이지 카테고리 템플릿 선택자
	/// </summary>
	public class PageCategoryTemplateSelector : DataTemplateSelector
	{
		public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
		{
			Page page = item as Page;

			if (page == null)
				return base.SelectTemplate(item, container);
			
			//if (page.ParentModel is CustomerViewModel)
			//	return App.MainWindowView.FindResource(new DataTemplateKey(typeof(CustomerViewModel))) as DataTemplate;

			//if (page.ParentModel is MapViewModel)
			//	return App.MainWindowView.FindResource(new DataTemplateKey(typeof(MapViewModel))) as DataTemplate;

			if (page.ParentModel is MenuViewModel)
				return App.MainWindowView.FindResource(new DataTemplateKey(typeof(MenuViewModel))) as DataTemplate;
			
			return base.SelectTemplate(item, container);
		}
	}
}
