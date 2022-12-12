
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace OpticaNX.Menu
{
	/// <summary>
	/// SPC의 상단 메뉴 생성 클래스.
	/// </summary>
	public class MenuGenerator
	{
		#region Fields

		private IList<MenuItem> _menuItems;    // 메뉴 목록
															//private readonly List<SpcEditionType> ALL_EDITIONS = new List<SpcEditionType> { SpcEditionType.BuiltInSPC, SpcEditionType.RemoteSPC, SpcEditionType.SpcClient };
															//private readonly List<SpcEditionType> REMOTE_SPC_SPCCLIENT_EDITIONS = new List<SpcEditionType> { SpcEditionType.RemoteSPC, SpcEditionType.SpcClient };
															//private readonly List<SpcEditionType> BUILTIN_EDITION = new List<SpcEditionType> { SpcEditionType.BuiltInSPC };

		#endregion

		#region Constructors

		/// <summary>
		/// 기본 생성자
		/// </summary>
		/// <param name="cmd">메뉴를 실행할 커멘드</param>
		public MenuGenerator(ICommand cmd)
		{
			_menuItems = new List<MenuItem>();
			// 메뉴항목 빌드
			BuildMenuItems(cmd);
		}

		#endregion

		public IEnumerable<MenuItem> MenuItems
		{
			get { return _menuItems; }
		}

		#region Public Methods

		/// <summary>
		/// 주어진 메뉴종류에 따른 해당 메뉴항목들을 반환한다.
		/// </summary>
		/// <param name="menuType">반환하고자 하는 메뉴종류</param>
		/// <returns></returns>
		public IEnumerable<MenuItem> GetItems(MenuType menuType)
		{
			return _menuItems.Where(x => x.MenuType == menuType);
		}

		/// <summary>
		/// Customer Report를 제외한 모든 메뉴항목들을 반환한다.
		/// </summary>
		/// <returns>메뉴항목 목록</returns>
		public IEnumerable<MenuItem> GetAllItems()
		{
			return _menuItems.Where(x => x.MenuType != MenuType.CustomerReport);
		}

		#endregion

		#region Private Methods

		// 메뉴항목 빌드
		private void BuildMenuItems(ICommand cmd)
		{
			// Home
			var brush = new SolidColorBrush(Color.FromRgb(0x6c, 0xbd, 0x45));
			//_menuItems.Add(new MenuItem("IDC_WELCOME", "pack://application:,,,/Image/Copy-24x24.png", cmd, MenuType.Welcome, brush, @"View/Form/YieldView.xaml"));

			// Setting
			brush = new SolidColorBrush(Colors.DarkGray);
			//_menuItems.Add(new MenuItem("SPC Setting", "pack://application:,,,/Image/setting.png", cmd, MenuType.Setting, brush, @"View/Form/SettingView.xaml"));
			//_menuItems.Add(new MenuItem("Defect Type", "pack://application:,,,/Image/defect.png", cmd, MenuType.Setting, brush, @"View/Form/DefectTypeView.xaml"));
			//_menuItems.Add(new MenuItem("Manager Login", "pack://application:,,,/Image/ManagerLogin.png", cmd, MenuType.Setting, brush, @"View/Form/DefectTypeView.xaml"));

			// View
			brush = new SolidColorBrush(Color.FromRgb(0xd4, 0xaf, 0x00));
			////_menuItems.Add(new MenuItem("Yield_Old", "pack://application:,,,/Image/Yield.png", cmd, MenuType.Production, brush, @"View/Form/YieldView.xaml"));
			//_menuItems.Add(new MenuItem("Layer Merge", "pack://application:,,,/Image/GlobalColorScheme_32x32.png", cmd, MenuType.Production, brush, @"View/Form/ProductionSideCombineView.xaml"));
			//_menuItems.Add(new MenuItem("Yield", "pack://application:,,,/Image/ChangeChartSeriesType_32x32.png", cmd, MenuType.Production, brush, @"View/Form/ProductionYieldView.xaml"));
			////_menuItems.Add(new MenuItem("Yield Report", "pack://application:,,,/Image/Yield-R.png", cmd, MenuType.Production, brush, @"View/Form/YieldReportView.xaml"));
			//_menuItems.Add(new MenuItem("Defect", "pack://application:,,,/Image/LegendNone_32x32.png", cmd, MenuType.Production, brush, @"View/Form/ProductionFailView.xaml"));
			////_menuItems.Add(new MenuItem("Defect All", "pack://application:,,,/Image/Exponential_32x32.png", cmd, MenuType.Production, brush, @"View/Form/ProductionFailTypeView.xaml"));
			//_menuItems.Add(new MenuItem("Defect TOP10", "pack://application:,,,/Image/Exponential_32x32.png", cmd, MenuType.Production, brush, @"View/Form/ProductionFailTop10View.xaml"));
			////_menuItems.Add(new MenuItem("Defect22", "pack://application:,,,/Image/LegendNone_32x32.png", cmd, MenuType.Production, brush, @"View/Control/SearchControlView_V2.xaml"));
			//_menuItems.Add(new MenuItem("Defect Map", "pack://application:,,,/Image/LegendNone_32x32.png", cmd, MenuType.Production, brush, @"View/Form/ProductionFailMapView.xaml"));
			//_menuItems.Add(new MenuItem("Panel Summary", "pack://application:,,,/Image/BOReport_32x32.png", cmd, MenuType.Production, brush, @"View/Form/ProductionPanelView.xaml"));
			//_menuItems.Add(new MenuItem("검사성적서 수율", "pack://application:,,,/Image/GlobalColorScheme_32x32.png", cmd, MenuType.Production, brush, @"View/Form/ProductionSideCombineCustomView.xaml"));

			//_menuItems.Add(new MenuItem("Detail AOI 수율", "pack://application:,,,/Image/GlobalColorScheme_32x32.png", cmd, MenuType.Manager, brush, @"View/Form/DetailAoiYieldView.xaml"));


			#region Report

			#endregion

		}

		#endregion
	}
}
