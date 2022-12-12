using System.Collections.ObjectModel;

namespace OpticaNX.Menu
{
	/// <summary>
	/// 메뉴 그룹을 대표하는 모델 클래스.
	/// </summary>
	public class ItemGroup
	{
		/// <summary>
		/// 메뉴그룹명
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
		/// 메뉴그룹에 속한 메뉴항목
		/// </summary>
		public ObservableCollection<Item> Items
		{
			get;
			set;
		}

		/// <summary>
		/// 메뉴타입 목록
		/// </summary>
		public MenuType[] MenuTypes
		{
			get;
			set;
		}
	}
}
