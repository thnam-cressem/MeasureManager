using DiagramControl.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiagramControl
{
	/// <summary>
	/// Interaction logic for ModelMapControl.xaml
	/// </summary>
	public partial class ModelMapControl : UserControl
	{
		public static readonly DependencyProperty CursorPositionHeightProperty = DependencyProperty.Register("CursorPositionHeight", typeof(int), typeof(ModelMapControl), new FrameworkPropertyMetadata(25));

		//public event SelectedPackageEvent SelectedPackage = delegate { };
		//public event SelectedPackageEvent MovePosition = delegate { };

		private ModelViewerControl _modelMap = new ModelViewerControl();
		//private ModelInfo _modelInfo = new ModelInfo();
		//private Position _currentWindow;

		private System.Drawing.Point _lastMouseEventPosition;

		private List<Tuple<RectangleF, Brush>> _fillDiagrams = new List<Tuple<RectangleF, Brush>>();
		private List<Tuple<PointF, string>> _drawMessage = new List<Tuple<PointF, string>>();
		private List<Tuple<PointF, string>> _drawCustomMessage = new List<Tuple<PointF, string>>();

		private bool _isDrawModuleNickName = false;

		public ModelMapControl()
		{
			InitializeComponent();

			_modelMap.DiagramControlPaint += _modelMap_DiagramControlPaint;
			_modelMap.InvertYAxis = true;
			_modelMap.MouseMove += _modelMap_MouseMove;
			_modelMap.MouseClick += _modelMap_MouseEvent;
			windowsFormsHost.Child = _modelMap;

			CursorPositionHeight = 25;
			CustomMessageFontSize = 15.0f;
		}

		//public bool IsDrawModelImage
		//{
		//	set
		//	{
		//		_modelMap.IsDrawModelImage = value;
		//	}
		//}

		//public bool IsDrawModuleArea
		//{
		//	set
		//	{
		//		_modelMap.IsDrawModuleArea = value;
		//	}
		//}

		//public bool IsDrawPackageArea
		//{
		//	set
		//	{
		//		_modelMap.IsDrawPackageArea = value;
		//	}
		//}

		//public bool IsDrawWindowArea
		//{
		//	set
		//	{
		//		_modelMap.IsDrawWindowArea = value;
		//	}
		//}

		//public bool IsDrawModuleNickName
		//{
		//	get
		//	{
		//		return _isDrawModuleNickName;
		//	}
		//	set
		//	{
		//		_isDrawModuleNickName = value;
		//	}
		//}

		//public bool InvertYAxis
		//{
		//	set
		//	{
		//		_modelMap.InvertYAxis = value;
		//	}
		//}

		public float CustomMessageFontSize { get; set; }

		private void _modelMap_MouseEvent(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				ContextMenu cms = new ContextMenu();

				MenuItem item1 = new MenuItem();
				item1.Header = "Add Mark";
				item1.Click += AddMarkClickedClick;
				cms.Items.Add(item1);


				MenuItem item2 = new MenuItem();
				item2.Header = "Move Position";
				item2.Click += MovePositionClickedClick;
				cms.Items.Add(item2);

				cms.IsOpen = true;

				_lastMouseEventPosition.X = e.X;
				_lastMouseEventPosition.Y = e.Y;

			}
		}

		private void AddMarkClickedClick(object sender, RoutedEventArgs e)
		{
			PointF robot = _modelMap.PixelToRobot(_lastMouseEventPosition.X, _lastMouseEventPosition.Y);

			//foreach (var package in _modelInfo.Packages.Where(x => x.GetRobotRect().Contains(robot) == true))
			//{
			//	SelectedPackage(this, package);
			//}
		}

		private void MovePositionClickedClick(object sender, RoutedEventArgs e)
		{
			PointF robot = _modelMap.PixelToRobot(_lastMouseEventPosition.X, _lastMouseEventPosition.Y);

			//foreach (var package in _modelInfo.Packages.Where(x => x.GetRobotRect().Contains(robot) == true))
			//{
			//	MovePosition(this, package);
			//}
		}
		
		public int CursorPositionHeight
		{
			get
			{
				return (int)GetValue(CursorPositionHeightProperty);
			}
			set
			{
				SetValue(CursorPositionHeightProperty, value);
				CursorPositin.Height = new GridLength(value);
			}
		}

		private void _modelMap_MouseMove(object sender, MouseEventArgsEx e)
		{
			lblPosition.Content = String.Format("Pixel(X : {0}, Y : {1}), Robot(X : {2}, Y : {3})", e.X, e.Y, e.RobotPos.X, e.RobotPos.Y);
		}

		private void _modelMap_DiagramControlPaint(object sender, Graphics g)
		{
			//DrawCross(_currentWindow, g);
			DrawFillRects(g);
			DrawMessage(g);
		}

		private void DrawFillRects(Graphics g)
		{
			if (chkVisibleUserAnnotation.IsChecked == true)
			{
				foreach (Tuple<RectangleF, Brush> tup in _fillDiagrams)
				{
					g.FillRectangle(tup.Item2, tup.Item1.Left, tup.Item1.Bottom, tup.Item1.Width, -tup.Item1.Height);
				}
			}
				
		}

		//private void DrawCross(Position position, Graphics g)
		//{
		//	if (position == null)
		//		return;
		//	RectangleF rectF = RectangleF.FromLTRB(position.Left, position.Top, position.Right, position.Bottom);
			
		//	Pen pen = new Pen(Brushes.Blue);
		//	pen.Width = 2 / _modelMap.Zoom;

		//	g.DrawLine(pen, -1000000, rectF.Top + rectF.Height / 2, 1000000, rectF.Top + rectF.Height / 2);
		//	g.DrawLine(pen, rectF.Left + rectF.Width / 2, -1000000, rectF.Left + rectF.Width / 2, 1000000);
		//}

		//public void LoadModel(ModelInfo modelInfo, bool drawDetailObject = true)
		//{
		//	_modelInfo = modelInfo;

		//	ClearFillRect();
		//	ClearMessage();
		//	_modelMap.ClearDiagram();
		//	_modelMap.LoadModel(_modelInfo);

		//	if (_modelInfo != null && IsDrawModuleNickName == true)
		//	{
		//		foreach (ModuleInfo module in _modelInfo.Modules)
		//		{
		//			PointF pt = new PointF();
		//			pt.X = module.GetRobotRect().Left;
		//			pt.Y = module.GetRobotRect().Bottom;
		//			if (String.IsNullOrWhiteSpace(module.NickName) == false)
		//			{
		//				AddMessage(pt, module.NickName);
		//			}
		//			else
		//			{
		//				AddMessage(pt, module.ModuleId.ToString());
		//			}
		//		}
		//	}

		//}

		public void ClearFillRect()
		{
			_fillDiagrams.Clear();
		}

		public void AddFillRect(RectangleF rect, Brush brush)
		{
			//if (rect.IsEmpty == true )
			//	return;

			_fillDiagrams.Add(new Tuple<RectangleF, Brush>(rect, brush));
		}

		public void ClearMessage()
		{
			_drawMessage.Clear();
		}

		public void AddMessage(PointF rect, string message)
		{
			if (rect.IsEmpty == true || rect.X < 0 || rect.Y < 0)
				return;

			_drawMessage.Add(new Tuple<PointF, string>(rect, message));
		}

		public void ClearCustomMessage()
		{
			_drawCustomMessage.Clear();
		}

		public void AddCustomMessage(PointF rect, string message)
		{
			if (rect.IsEmpty == true || rect.X < 0 || rect.Y < 0)
				return;

			_drawCustomMessage.Add(new Tuple<PointF, string>(rect, message));
		}

		private void DrawMessage(Graphics g)
		{
			//if (_modelInfo != null)
			//{
			//	if (_modelInfo.PixelMicronHeight > 0.0f && _modelInfo.PixelMicronWidth > 0.0f)
			//	{
			//		// 이전에 모델을 그릴 때에는, Y축의 상하가 바뀌어져 그려져 있다.
			//		// 이 때문에 글씨를 쓸때에 글씨가 상하가 바뀌어서 써진다.
			//		// 이를 해결하기 위해 Y축의 상하를 바꾸고, 좌표의 Y값에 -1을 곱하여 글씨를 쓴다.
			//		g.ScaleTransform(1.0f, -1.0f);

			//		float moduleSize = 100.0f;
			//		if (_modelInfo.Modules != null && _modelInfo.Modules.Count() > 0)
			//			moduleSize = _modelInfo.Modules.FirstOrDefault().Width / (_modelInfo.Modules.FirstOrDefault().NickName.Length > 0 ? _modelInfo.Modules.FirstOrDefault().NickName.Length : 5);

			//		int emSize = (int)(moduleSize);
			//		// DtawString할 때에 emSize가 25보다작으면 Windows 7 에서"A generic error occurred in GDI+" 에러 발생
			//		// 원인은 아직 못밝힘. Graphics에서 Zoom과 관련이 있는 것으로 판단됨
			//		// 2018.11.06 bgyoon
			//		if (emSize <= 0)
			//			emSize = 50;
			//		Font drawFont = new Font("Arial", emSize, GraphicsUnit.Pixel);
			//		SolidBrush drawBrush = new SolidBrush(Color.WhiteSmoke);
			//		foreach (Tuple<PointF, string> tup in _drawMessage)
			//		{
			//			//RectangleF rect = new RectangleF(tup.Item1.Left, tup.Item1.Bottom, tup.Item1.Width, -tup.Item1.Height);
			//			PointF point = new PointF(tup.Item1.X, tup.Item1.Y * -1);

			//			g.DrawString(tup.Item2, drawFont, drawBrush, point);
			//		}

			//		if(chkVisibleUserAnnotation.IsChecked == true)
			//		{
			//			drawFont = new Font("Arial", CustomMessageFontSize, GraphicsUnit.Pixel);
			//			foreach (Tuple<PointF, string> tup in _drawCustomMessage)
			//			{
			//				PointF point = new PointF(tup.Item1.X, tup.Item1.Y * -1);

			//				g.DrawString(tup.Item2, drawFont, drawBrush, point);
			//			}
			//		}				

			//		g.ResetTransform();
			//	}

			//}

		}

		public void FitToRect(RectangleF rect)
		{
			_modelMap.FitToRect(rect);
		}

		public void FitToModelSize()
		{
			//RectangleF rect = RectangleF.FromLTRB(_modelInfo.Left, _modelInfo.Top, _modelInfo.Right, _modelInfo.Bottom);
			//FitToRect(rect);
		}

		public void SetCurrentPackage(int moduleId, string packageId, string windowId = null)
		{
			//if (_modelInfo == null)
			//	return;

			//var packageInfo = _modelInfo.Packages.Where(x => x.ModuleId == moduleId && x.PackageId == packageId).FirstOrDefault();
			//if (packageInfo == null)
			//{
			//	string msg = String.Format("No Package Info in this Model : [{0}]", moduleId);
			//	MessageBox.Show(msg);
			//	return;
			//}

			//if (String.IsNullOrWhiteSpace(windowId) == false)
			//	_currentWindow = packageInfo.Windows.Where(x => x.WindowId == windowId).FirstOrDefault();
			//else
			//	_currentWindow = packageInfo;

			_modelMap.Invalidate();
		}

		public void FillModule(string moduleId, Brush brush)
		{

		}

		public void FillPackage(int moduleId, long packageSeq)
		{

		}

		//public void AddCross(PointF center, float length)
		//{
		//	_modelMap.AddCross(center, length);
		//}

		public void Invalidate()
		{
			_modelMap.Invalidate();
		}

		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			_modelMap.Invalidate();
		}

		private void CaptureButton_Click(object sender, RoutedEventArgs e)
		{
			//var image = ScreenCaptureHelper.CaptureWindow(_modelMap.Handle);
			//SaveFileDialog dialog = new SaveFileDialog();
			//dialog.Filter = "jpg files (*.jpg)|*.jpg;*.jpg|All files (*.*)|*.*";
			//if(dialog.ShowDialog() == true)
			//{
			//	image.Save(dialog.FileName);
			//}
		}
	}
}
