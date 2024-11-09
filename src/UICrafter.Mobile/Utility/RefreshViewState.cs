namespace UICrafter.Mobile.Utility;
using System.ComponentModel;

public partial class RefreshViewState : INotifyPropertyChanged
{
	private bool _isRefreshing;

	public bool IsRefreshing
	{
		get => _isRefreshing;
		private set
		{
			if (_isRefreshing != value)
			{
				_isRefreshing = value;
				OnPropertyChanged(nameof(IsRefreshing));
			}
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	public void SetIsRefreshing(bool isRefreshing)
	{
		IsRefreshing = isRefreshing;
	}

	protected virtual void OnPropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
