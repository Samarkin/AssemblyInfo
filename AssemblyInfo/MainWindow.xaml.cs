using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace AssemblyInfo
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly string _title;
		private AssemblyProber _prober;
		private string _fileName;

		public MainWindow()
		{
			InitializeComponent();
			_title = Title;

			Preload(Environment.GetCommandLineArgs().Skip(1).FirstOrDefault());
			DisplayPreloaded();
		}

		private void Preload(string fileName)
		{
			_prober = new AssemblyProber(fileName);
			_fileName = fileName;

			Background = GetBrushForError();
		}

		private void DisplayPreloaded()
		{
			DataContext = _prober;
			Background = Brushes.White;
			Title = !string.IsNullOrEmpty(_fileName)
				? string.Format("{0} - {1}", Path.GetFileName(_fileName), _title)
				: _title;
		}

		#region Event handlers

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.F3 || e.Key == Key.Escape)
				Close();
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
