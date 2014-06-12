using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AssemblyInfo
{
	public enum ErrorLevel
	{
		Success = 0,
		ReflectionError = 1,
		FileNotFound = 2,
		ArgumentError = 3
	}

	public class AssemblyProber
	{
		private const string Unknown = "<Unknown>";

		private readonly string _displayName = Unknown;
		private readonly string _clrVersion = Unknown;
		private readonly string _architecture = Unknown;
		private readonly string _culture = Unknown;
		private readonly string _assemblyVersion = Unknown;
		private readonly string _fileVersion = Unknown;
		private readonly string _productVersion = Unknown;
		private readonly string _fileName = Unknown;

		private readonly string[] _dependencies = new string[0];

		private readonly bool _gac;

		private readonly ErrorLevel _errorLevel;

		public AssemblyProber(string fileName)
		{
			try
			{
				_fileName = Path.GetFileName(fileName);

				var fileVersionInfo = FileVersionInfo.GetVersionInfo(fileName);
				_fileVersion = fileVersionInfo.FileVersion;
				_productVersion = fileVersionInfo.ProductVersion;

				var assembly = Assembly.ReflectionOnlyLoadFrom(fileName);
				var assemblyName = assembly.GetName();

				_displayName = assemblyName.FullName;
				_architecture = assemblyName.ProcessorArchitecture.ToString();
				_culture = assemblyName.CultureName;
				_assemblyVersion = assemblyName.Version.ToString();

				_clrVersion = assembly.ImageRuntimeVersion;

				_dependencies = assembly.GetReferencedAssemblies().Select(an => an.FullName).OrderBy(a => a).ToArray();

				_errorLevel = ErrorLevel.Success;
			}
			catch (ArgumentException)
			{
				_errorLevel = ErrorLevel.ArgumentError;
			}
			catch (FileNotFoundException)
			{
				_errorLevel = ErrorLevel.FileNotFound;
			}
			catch (PathTooLongException)
			{
				_errorLevel = ErrorLevel.FileNotFound;
			}
			catch (FileLoadException)
			{
				_errorLevel = ErrorLevel.ReflectionError;
			}
			catch (BadImageFormatException)
			{
				_errorLevel = ErrorLevel.ReflectionError;
			}
		}

		public ErrorLevel ErrorLevel
		{
			get { return _errorLevel; }
		}

		public string FileName
		{
			get { return _fileName; }
		}

		public string CLRVersion
		{
			get { return _clrVersion; }
		}

		public string Architecture
		{
			get { return _architecture; }
		}

		public string Culture
		{
			get { return _culture; }
		}

		public string AssemblyVersion
		{
			get { return _assemblyVersion; }
		}

		public string FileVersion
		{
			get { return _fileVersion; }
		}

		public string ProductVersion
		{
			get { return _productVersion; }
		}

		public string DisplayName
		{
			get { return _displayName; }
		}

		public bool GlobalAssemblyCache
		{
			get { return _gac; }
		}

		public IEnumerable<string> Dependencies
		{
			get { return _dependencies; }
		}
	}
}
