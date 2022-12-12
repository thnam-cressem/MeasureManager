using Cressem.Framework.InfraStructure;
using Cressem.Util.Command;
using OpticaNX.Menu;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OpticaNX.ViewModel
{
	public class MainViewModel : ObservableObject
	{
		private string splashCaption = "Loading...";

		private MenuViewModel _selectedWorkspace; // for Custom Menu

		private string _title;
		private bool _showLayoutPanelCaptions;
		public MainViewModel()
		{
			Workspaces = new List<MenuViewModel>();
			Workspaces.Add(new MenuViewModel(OpenMenuItemCommand));
		}

        private void InitCommands()
        {
            PreviewKeyDownCommand = new RelayCommand<KeyEventArgs>((x) => OnPreviewKeyDown(x));
            PreviewKeyUpCommand = new RelayCommand<KeyEventArgs>((x) => OnPreviewKeyDown(x));
            LoadedCommand = new RelayCommand(() => OnLoaded());
            OpenFileCommand = new RelayCommand(() => OnOpenFile());
        }

		public  ICommand LoadedCommand { get; set; }
		public ICommand OpenMenuItemCommand { get; set; }
        public ICommand OpenFileCommand { get; set; }
		public ICommand PreviewKeyDownCommand { get; private set; }
		public ICommand PreviewKeyUpCommand { get; private set; }

		public string SplashCaption
		{
			get { return splashCaption; }
			set
			{
				splashCaption = value;
				OnPropertyChanged(this, "SplashCaption");
			}
		}

		public MenuViewModel SelectedWorkspace
		{
			get
			{
				return _selectedWorkspace;
			}
			set
			{
				if (value == _selectedWorkspace)
					return;

				_selectedWorkspace = value;
				OnPropertyChanged(this, "SelectedWorkspace");
			}
		}

		public bool ShowLayoutPanelCaptions
		{
			get
			{
				return _showLayoutPanelCaptions;
			}
			set
			{
				_showLayoutPanelCaptions = value;
				OnPropertyChanged();
			}
		}


		public IList<MenuViewModel> Workspaces
		{
			get;
			set;
		}

		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				_title = value;
				OnPropertyChanged(this, "Title");
			}
		}

		private void OnLoaded()
		{
			Title = String.Format($"OpticaNX - {GetAssemblyFileVersion()}");
		}

        private void OnOpenFile()
        {

        }
		public static string GetAssemblyFileVersion()
		{
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
			FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);
			return fileVersion.FileVersion;
		}

		private void OnPreviewKeyDown(KeyEventArgs args)
		{
			if (args.Key == Key.LeftShift && args.KeyStates == KeyStates.Down)
			{
				ShowLayoutPanelCaptions = true;
			}
			else
			{
				ShowLayoutPanelCaptions = false;
			}

			Debug.WriteLine($"Key : {args.Key}, KeyStatus : {args.KeyStates}");
		}
	}
}
