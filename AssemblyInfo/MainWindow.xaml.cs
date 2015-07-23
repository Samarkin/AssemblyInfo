using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AssemblyInfo.Common;

namespace AssemblyInfo
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private const string AssemblyKeyword = "/assembly";

		private readonly string _title;
		private AssemblyProber _prober;
		private AssemblyProber _loadedProber;
		private readonly string _exePath;

		public MainWindow()
		{
			InitializeComponent();
			_title = Title;

			string name = null;
			bool assembly = false;
			foreach (var arg in Environment.GetCommandLineArgs().Skip(1))
			{
				if (string.Equals(arg, AssemblyKeyword, StringComparison.OrdinalIgnoreCase))
				{
					assembly = true;
				}
				else
				{
					name = name ?? arg;
				}
			}
			Preload(name, assembly);
			_exePath = Environment.GetCommandLineArgs().FirstOrDefault();
			DisplayPreloaded();
		}

		private void Preload(string name, bool isAssemblyName = false)
		{
			_prober = AssemblyProber.Create(name, isAssemblyName);

			Background = GetBrushForError();
		}

		private void DisplayPreloaded()
		{
			DataContext = _prober;
			Background = Brushes.White;
			Title = (_prober != null && !string.IsNullOrEmpty(_prober.FileName))
				? string.Format("{0} - {1}", _prober.FileName, _title)
				: _title;
			_loadedProber = _prober;
		}

		private void DiscardPreloaded()
		{
			_prober = null;
		}

		#region Event handlers

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.F3 || e.Key == Key.Escape)
				Close();
		}

		private void OnDependencySelected(object sender, MouseButtonEventArgs e)
		{
			AssemblyDependency dependency = ((ListViewItem)sender).Content as AssemblyDependency;
			if (dependency == null) return;
			try
			{
				var locDir = Path.GetDirectoryName(_prober.Location);
				Process.Start(new ProcessStartInfo
					{
						FileName = _exePath,
						Arguments = string.Format("{0} \"{1}\"", AssemblyKeyword, dependency.DisplayName),
						WorkingDirectory = string.IsNullOrWhiteSpace(locDir) ? Environment.CurrentDirectory : locDir
					});
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void OnLocateClick(object sender, RoutedEventArgs e)
		{
			try
			{
				Process.Start("explorer.exe", string.Format("/select,{0}", _prober.Location));
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void OnDisplayNameClick(object sender, MouseButtonEventArgs e)
		{
			if (_loadedProber.DisplayName == null) return;
			try
			{
				Clipboard.SetText(_loadedProber.DisplayName);
				MessageBox.Show("Assembly name copied to clipboard", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("Unable to copy to clipboard: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		#endregion

		#region Drag-n-drop

		private void OnDragEnter(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent("FileDrop"))
			{
				e.Effects = DragDropEffects.None;
				return;
			}
			
			var files = e.Data.GetData("FileDrop") as string[];
			if (files == null || files.Length == 0) return;

			Preload(files[0]);
		}

		private void OnDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("FileDrop"))
			{
				DisplayPreloaded();
			}
		}

		private void OnDragLeave(object sender, DragEventArgs e)
		{
			Background = Brushes.White;
			DiscardPreloaded();
		}

		#endregion

		private Brush GetBrushForError()
		{
			switch (_prober.ErrorLevel)
			{
				case ErrorLevel.Success:
					return Brushes.PaleGreen;
				case ErrorLevel.ReflectionError:
					return Brushes.Beige;
				case ErrorLevel.FileNotFound:
					return Brushes.PaleVioletRed;
				default:
					return Brushes.Black;
			}
		}
	}
}
