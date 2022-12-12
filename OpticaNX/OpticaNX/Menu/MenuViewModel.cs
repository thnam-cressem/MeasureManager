using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace OpticaNX.Menu
{
	/// <summary>
	/// SPC 상단 리본 메뉴에 대한 뷰모델
	/// </summary>
	public class MenuViewModel : MenuViewModelBase
	{
		#region Fields

		private MenuGenerator _menuGenerator;

		#endregion

		#region Constructors

		public MenuViewModel(ICommand cmd)
		{
			_menuGenerator = new MenuGenerator(cmd);

			this.Caption = null;
			BuildContextualPages();
		}

		#endregion

		public MenuItem GetMenuItem(string name)
		{
			return _menuGenerator.MenuItems.Where(x => x.MenuName == name).FirstOrDefault();
		}

		#region Private Methods

		private void BuildContextualPages()
		{
			this.ContextualPages = new ObservableCollection<Page>();

			// Setting
			this.ContextualPages.Add(
				new Page()
				{
					Caption = "Setting",
					ParentModel = this,
					ItemGroups = GetItemGroups(new Tuple<string, MenuType[]>("Setting", new MenuType[] { MenuType.Setting }))
				});

			// View
			this.ContextualPages.Add(
				new Page()
				{
					Caption = "Production",
					ParentModel = this,
					ItemGroups = GetItemGroups(new Tuple<string, MenuType[]>("Production", new MenuType[] { MenuType.Production }))
				});

			
			//// MeasureAnalysis
			//this.ContextualPages.Add(
			//	new Page()
			//	{
			//		Caption = "Measure Analysis",
			//		ParentModel = this,
			//		ItemGroups = GetItemGroups(new Tuple<string, MenuType[]>("MeasureAnalysis", new MenuType[] { MenuType.MeasureAnalysis }))
			//	});

			//// Defect Analysis
			//this.ContextualPages.Add(
			//	new Page()
			//	{
			//		Caption = "Defect Analysis",
			//		ParentModel = this,
			//		ItemGroups = GetItemGroups(new Tuple<string, MenuType[]>("DefectAnalysis", new MenuType[] { MenuType.DefectAnalysis }))
			//	});

			//// Export
			//this.ContextualPages.Add(
			//	new Page()
			//	{
			//		Caption = "Export",
			//		ParentModel = this,
			//		ItemGroups = GetItemGroups(new Tuple<string, MenuType[]>("Export", new MenuType[] { MenuType.Export }))
			//	});

			//// Home
			//page = new Page()
			//{
			//	Caption = "IDC_HOME",
			//	ParentModel = this,
			//};
			//itemGroups = new List<Tuple<string, MenuType[]>>();
			//itemGroups.Add(new Tuple<string, MenuType[]>(String.Empty, new MenuType[] { MenuType.Welcome }));
			////if (App.SpcEdition != SpcEditionType.BuiltInSPC) {
			////	itemGroups.Add(new Tuple<string, MenuType[]>(String.Empty, new MenuType[] { MenuType.Connections }));
			////}
			//itemGroups.Add(new Tuple<string, MenuType[]>(String.Empty, new MenuType[] { MenuType.Setting }));
			//itemGroups.Add(new Tuple<string, MenuType[]>(String.Empty, new MenuType[] { MenuType.Exit }));
			//page.ItemGroups = GetItemGroups(itemGroups.ToArray());
			//this.ContextualPages.Add(page);

			//// View
			//this.ContextualPages.Add(
			//	new Page()
			//	{
			//		Caption = "IDC_VIEW",
			//		ParentModel = this,
			//		ItemGroups = GetItemGroups(new Tuple<string, MenuType[]>("PCB", new MenuType[] { MenuType.View1 }),
			//											new Tuple<string, MenuType[]>(String.Empty, new MenuType[] { MenuType.View2 }))
			//	});

			//// Analysis
			//this.ContextualPages.Add(
			//	new Page()
			//	{
			//		Caption = "IDC_ANALYSIS",
			//		ParentModel = this,
			//		ItemGroups = GetItemGroups(new Tuple<string, MenuType[]>(String.Empty, new MenuType[] { MenuType.Analysis }))
			//	});

			////// Multi-SPC
			////if (App.SpcEdition != SpcEditionType.BuiltInSPC)
			////{
			////	this.ContextualPages.Add(
			////		new Page()
			////		{
			////			Caption = LocalizingEngine.GetUIString("IDC_MULTI_SPC"),
			////			ParentModel = this,
			////			ItemGroups = GetItemGroups(new Tuple<string, MenuType[]>(String.Empty, new MenuType[] { MenuType.MultiSPC }))
			////		});
			////}

			//// Report
			//this.ContextualPages.Add(
			//	new Page()
			//	{
			//		Caption = "IDC_REPORT",
			//		ParentModel = this,
			//		ItemGroups = GetItemGroups(new Tuple<string, MenuType[]>("General", new MenuType[] { MenuType.GeneralReport }),
			//											new Tuple<string, MenuType[]>("Export", new MenuType[] { MenuType.ExportReport }),
			//											new Tuple<string, MenuType[]>("Customer", new MenuType[] { MenuType.CustomerReport }))
			//	});

			//// Tool
			//this.ContextualPages.Add(
			//	new Page()
			//	{
			//		Caption = "IDC_TOOL",
			//		ParentModel = this,
			//		//ItemGroups = GetItemGroups(new Tuple<string, MenuType[]>("Search Condition", new MenuType[] { MenuType.SearchCondition }),
			//		//									new Tuple<string, MenuType[]>("Layout", new MenuType[] { MenuType.Layout }))
			//		ItemGroups = GetItemGroups(new Tuple<string, MenuType[]>("Search Condition", new MenuType[] { MenuType.SearchCondition }))
			//	});

			//// Help
			//this.ContextualPages.Add(
			//	new Page()
			//	{
			//		Caption = ("IDC_HELP"),
			//		ParentModel = this,
			//		ItemGroups = GetItemGroups(new Tuple<string, MenuType[]>(String.Empty, new MenuType[] { MenuType.Help }))
			//	});
		}

		private ObservableCollection<ItemGroup> GetItemGroups(params Tuple<string, MenuType[]>[] tuples)
		{
			var groups = new ObservableCollection<ItemGroup>();

			ItemGroup group;
			foreach (var tuple in tuples)
			{
				group = new ItemGroup()
				{
					Caption = tuple.Item1,
					Items = GetItems(tuple.Item2),
					MenuTypes = tuple.Item2
				};
				groups.Add(group);
			}

			return groups;
		}

		private ObservableCollection<Item> GetItems(params MenuType[] menuTypes)
		{
			var items = new ObservableCollection<Item>();

			Item item;
			foreach (var menuType in menuTypes)
			{
				foreach (var menuItem in _menuGenerator.GetItems(menuType))
				{
					item = new Item
					{
						MenuName = menuItem.MenuName,
						Content = menuItem.MenuName,
						Command = menuItem.Commmand,						
						Glyph = new BitmapImage(new Uri(menuItem.ImagePath)),
						LargeGlyph = new BitmapImage(new Uri(menuItem.ImagePath)),
						MenuType = menuItem.MenuType,
					};
					items.Add(item);
				}
			}

			return items;
		}

		#endregion

		public void AddManagerMenu()
		{
			if (ContextualPages.Where(x => x.Caption == "Manager").Count() == 1)
				return;
			this.ContextualPages.Add(
				new Page()
				{
					Caption = "Manager",
					ParentModel = this,
					ItemGroups = GetItemGroups(new Tuple<string, MenuType[]>("Manager", new MenuType[] { MenuType.Manager }))
				});

		}
	}
}
