﻿using System;
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

		private string _displayName = Unknown;
		private string _clrVersion = Unknown;
		private string _architecture = Unknown;
		private string _culture = Unknown;
		private string _assemblyVersion = Unknown;
		private string _fileVersion = Unknown;
		private string _productVersion = Unknown;
		private string _fileName;
		private string _location;

		private string[] _dependencies = new string[0];

		private bool _gac;

		private readonly ErrorLevel _errorLevel;

		public AssemblyProber(string name, bool isAssemblyName = false)
		{
			try
			{
				if (isAssemblyName)
				{
					Assembly assembly;
					try
					{
						assembly = Assembly.ReflectionOnlyLoad(name);
					}
					catch (FileNotFoundException)
					{
						int commaPos = name.IndexOf(',');
						if (commaPos < 0)
						{
							throw;
						}
						var dllPath = Path.Combine(Directory.GetCurrentDirectory(), name.Substring(0, commaPos) + ".dll");
						assembly = Assembly.ReflectionOnlyLoadFrom(dllPath);
					}
					LoadAssemblyProperties(assembly);
					LoadFileProperties(assembly.Location);
				}
				else
				{
					LoadFileProperties(name);
					var assembly = Assembly.ReflectionOnlyLoadFrom(name);
					LoadAssemblyProperties(assembly);
				}

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

		private void LoadAssemblyProperties(Assembly assembly)
		{
			var assemblyName = assembly.GetName();

			_displayName = assemblyName.FullName;
			_architecture = assemblyName.ProcessorArchitecture.ToString();
			_culture = assemblyName.CultureName;
			_assemblyVersion = assemblyName.Version.ToString();

			_clrVersion = assembly.ImageRuntimeVersion;
			_gac = assembly.GlobalAssemblyCache;

			_dependencies = assembly.GetReferencedAssemblies().Select(an => an.FullName).OrderBy(a => a).ToArray();
		}

		private void LoadFileProperties(string fileName)
		{
			_location = fileName;
			_fileName = Path.GetFileName(fileName);

			var fileVersionInfo = FileVersionInfo.GetVersionInfo(fileName);
			_fileVersion = fileVersionInfo.FileVersion;
			_productVersion = fileVersionInfo.ProductVersion;
		}

		public ErrorLevel ErrorLevel
		{
			get { return _errorLevel; }
		}

		public string FileName
		{
			get { return _fileName; }
		}

		public string Location
		{
			get { return _location; }
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
