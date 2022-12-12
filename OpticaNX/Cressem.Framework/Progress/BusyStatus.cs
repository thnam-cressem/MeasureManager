using Cressem.Framework.InfraStructure;

namespace Cressem.Framework.Progress
{
	/// <summary>
	/// 데이터 처리 상태를 대표하는 모델 클래스.
	/// </summary>
	public class BusyStatus : ObservableObject
	{
		#region Fields

		private bool _isBusy;					// 데이터 처리중 여부
		private bool _isIndeterminateBusy;  // 종료시점을 가늠할 수 없는 데이터 처리중 여부
		private int _progressValue;			// 데이터 처리 진행값
		private int _minimum;					// 최소값
		private int _maximum;					// 최대값
		private string _busyContent;			// 처리중 문자
		private bool _supportCancellation;	// 작업 취소 지원 여부

		#endregion

		#region Constructors

		public BusyStatus()
		{
			_isBusy = false;
			_minimum = 0;
			_maximum = 100;
			_busyContent = "Progressing...";
			_supportCancellation = false;
            _progressValue = 0;

        }

		#endregion

		#region Properties

		/// <summary>
		/// 데이터 처리중 여부 (종료시점을 알수 있을때 사용, 진행율 정의해야 함)
		/// </summary>
		public bool IsBusy
		{
			get { return _isBusy; }
			set
			{
				if (value == _isBusy)
					return;

				_isBusy = value;
				OnPropertyChanged(this, "IsBusy");

				_isIndeterminateBusy = false;
				OnPropertyChanged(this, "IsIndeterminateBusy");
			}
		}

		/// <summary>
		/// 데이터 처리중 여부 (종료시점을 알수 없을때 사용)
		/// </summary>
		public bool IsIndeterminateBusy
		{
			get { return _isIndeterminateBusy; }
			set
			{
				if (value == _isIndeterminateBusy)
					return;

				_isIndeterminateBusy = value;
				OnPropertyChanged(this, "IsIndeterminateBusy");
				// 값 동기화
				_isBusy = value;
				OnPropertyChanged(this, "IsBusy");
			}
		}

		/// <summary>
		/// 데이터 처리 진행값
		/// </summary>
		public int ProgressValue
		{
			get { return _progressValue; }
			set
			{
				if (value == _progressValue)
					return;

				_progressValue = value;
				OnPropertyChanged(this, "ProgressValue");
			}
		}

		/// <summary>
		/// 최소값
		/// </summary>
		public int Minimum
		{
			get { return _minimum; }
			set
			{
				if (value == _minimum)
					return;

				_minimum = value;
				OnPropertyChanged(this, "Minimum");
			}
		}

		/// <summary>
		/// 최대값
		/// </summary>
		public int Maximum
		{
			get { return _maximum; }
			set
			{
				if (value == _maximum)
					return;

				_maximum = value;
				OnPropertyChanged(this, "Maximum");
			}
		}

		/// <summary>
		/// 처리중 문자
		/// </summary>
		public string BusyContent
		{
			get { return _busyContent; }
			set
			{
				if (value == _busyContent)
					return;

				_busyContent = value;
				OnPropertyChanged(this, "BusyContent");
			}
		}

		/// <summary>
		/// 작업취소 지원 여부
		/// </summary>
		public bool SupportCancellation
		{
			get { return _supportCancellation; }
			set
			{
				if (value == _supportCancellation)
					return;

				_supportCancellation = value;
				OnPropertyChanged(this, "SupportCancellation");
			}
		}

		#endregion
	}
}
