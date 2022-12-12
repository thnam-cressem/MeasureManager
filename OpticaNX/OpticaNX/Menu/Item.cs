using System.Windows.Input;
using System.Windows.Media;

namespace OpticaNX.Menu
{
	/// <summary>
	/// 메뉴 항목을 대표하는 모델 클래스.
	/// </summary>
	public class Item
	{
		/// <summary>
		/// 메뉴명
		/// </summary>
		public string MenuName
		{
			get;
			set;
		}

		/// <summary>
		/// 메뉴 내용
		/// </summary>
		public object Content
		{
			get;
			set;
		}
		
		/// <summary>
		/// 커멘드
		/// </summary>
		public ICommand Command
		{
			get;
			set;
		}

		/// <summary>
		/// 작은 아이콘 이미지
		/// </summary>
		public ImageSource Glyph
		{
			get;
			set;
		}

		/// <summary>
		/// 큰 아이콘 이미지
		/// </summary>
		public ImageSource LargeGlyph
		{
			get;
			set;
		}

		/// <summary>
		/// 메뉴 타입
		/// </summary>
		public MenuType MenuType
		{
			get;
			set;
		}
	}
}
