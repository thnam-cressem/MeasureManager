using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;

namespace OpticaNX.Menu
{
	/// <summary>
	/// 
	/// </summary>
	public class MenuItem
	{
        #region Fields

        protected string _menuId;
        protected string _menuName;
		protected string _imagePath;
		protected MenuType _menuType;
		protected Brush _backColor;	// use only to create MetroMenu
		protected ICommand _command;
		protected string _path;

		#endregion

		#region Constructors

		public MenuItem()
		{
		}

		public MenuItem(string menuName, string imagePath, MenuType menuType, Brush brush)
		{
			_menuName = menuName;
			_imagePath = imagePath;
			_menuType = menuType;
			_backColor = brush;
		}

		public MenuItem(string menuName, string imagePath, ICommand command, MenuType menuType, Brush brush, string path)
			: this(menuName, imagePath, menuType, brush)
		{
			_command = command;
			_path = path;
        }

		#endregion

		#region Properties

		public MenuType MenuType
		{
			get
			{
				return _menuType;
			}
		}

        public string MenuId
        {
            get
            {
                return _path?.Replace("/", "").Replace(".", "");
            }
        }

        public string MenuName
		{
			get
			{
				return _menuName;
			}
		}

		public string ImagePath
		{
			get
			{
				return _imagePath;
			}
		}

		public Brush BackColor
		{
			get
			{
				return _backColor;
			}
		}

		public ICommand Commmand
		{
			get
			{
				return _command;
			}
		}

		public string Path
		{
			get
			{
				return _path;
			}
		}

		#endregion
	}
}
