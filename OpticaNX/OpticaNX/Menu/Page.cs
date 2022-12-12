using System.Collections.ObjectModel;

namespace OpticaNX.Menu
{
	/// <summary>
	/// 메뉴 페이지를 대표하는 모델 클래스.
	/// </summary>
	public class Page
	{
		/// <summary>
		/// 페이지명
		/// </summary>
		public string Caption
		{
			get;
			set;
		}
		
		/// <summary>
		/// 부모 객체
		/// </summary>
		public object ParentModel;

		/// <summary>
		/// 항목 그룹 목록
		/// </summary>
		public ObservableCollection<ItemGroup> ItemGroups
		{
			get;
			set;
		}
	}
}
