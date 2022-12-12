using System.Collections.ObjectModel;

namespace OpticaNX.Menu
{
	/// <summary>
	/// 메뉴 뷰모델의 베이스 뷰모델
	/// </summary>
	public abstract class MenuViewModelBase
	{
		/// <summary>
		/// 메뉴명
		/// </summary>
		public string Caption 
		{ 
			get;
			set;
		}

		/// <summary>
		/// 페이지 목록
		/// </summary>
		public ObservableCollection<Page> ContextualPages
		{
			get;
			set;
		}
	}
}
