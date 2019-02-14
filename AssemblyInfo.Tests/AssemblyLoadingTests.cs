using System;
using System.IO;
using AssemblyInfo.Common;
using NUnit.Framework;

namespace AssemblyInfo.Tests
{
	[TestFixture]
	public class AssemblyLoadingTests
	{
		[Test]
		public void X86AssemblyLoadTest()
		{
			var ass = AssemblyProber.Create(@"..\1\AssemblyInfo.Sample.v2.x86.dll");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.Success));
			Assert.That(ass.CLRVersion.StartsWith("v2."));
			Assert.That(ass.TargetFramework, Is.Null);
			Assert.That(ass.Architecture, Is.EqualTo("X86"));
			Assert.That(ass.GlobalAssemblyCache, Is.False);
			Assert.That(ass.Dependencies, Is.Not.Null);
			Assert.That(ass.AssemblyVersion, Is.EqualTo("1.0.0.0"));
			Assert.That(ass.FileVersion, Is.EqualTo("1.0.0.0"));
		}

		[Test]
		public void X86AssemblyLoadTest2()
		{
			var ass = AssemblyProber.Create(@"..\2\AssemblyInfo.Sample.v2.x86.dll");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.Success));
			Assert.That(ass.CLRVersion.StartsWith("v2."));
			Assert.That(ass.TargetFramework, Is.Null);
			Assert.That(ass.Architecture, Is.EqualTo("X86"));
			Assert.That(ass.GlobalAssemblyCache, Is.False);
			Assert.That(ass.Dependencies, Is.Not.Null);
			Assert.That(ass.AssemblyVersion, Is.EqualTo("1.0.0.0"));
			Assert.That(ass.FileVersion, Is.EqualTo("2.0.0.0"));
		}

		[Test]
		public void MultiAssemblyLoadTest()
		{
			var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
			var ass1 = AssemblyProber.Create(@"..\1\AssemblyInfo.Sample.v2.x86.dll");
			Assert.That(AppDomain.CurrentDomain.GetAssemblies(), Is.EqualTo(loadedAssemblies));
			Assert.That(ass1.ErrorLevel, Is.EqualTo(ErrorLevel.Success));
			Assert.That(ass1.AssemblyVersion, Is.EqualTo("1.0.0.0"));
			Assert.That(ass1.FileVersion, Is.EqualTo("1.0.0.0"));

			var ass2 = AssemblyProber.Create(@"..\2\AssemblyInfo.Sample.v2.x86.dll");
			Assert.That(AppDomain.CurrentDomain.GetAssemblies(), Is.EqualTo(loadedAssemblies));
			Assert.That(ass2.ErrorLevel, Is.EqualTo(ErrorLevel.Success));
			Assert.That(ass2.AssemblyVersion, Is.EqualTo("1.0.0.0"));
			Assert.That(ass2.FileVersion, Is.EqualTo("2.0.0.0"));
		}

		[Test]
		public void AnyCPUAssemblyLoadTest()
		{
			var ass = AssemblyProber.Create(@"AssemblyInfo.Sample.v4.MSIL.dll");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.Success));
			Assert.That(ass.CLRVersion.StartsWith("v4."));
			Assert.That(ass.TargetFramework, Is.EqualTo(".NET Framework 4.5"));
			Assert.That(ass.Architecture, Is.EqualTo("MSIL"));
			Assert.That(ass.GlobalAssemblyCache, Is.False);
			Assert.That(ass.Dependencies, Is.Not.Null);
		}

		[Test]
		public void X64AssemblyLoadTest()
		{
			var ass = AssemblyProber.Create(@"..\1\AssemblyInfo.Sample.v4.x64.dll");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.Success));
			Assert.That(ass.CLRVersion.StartsWith("v4."));
			Assert.That(ass.TargetFramework, Is.EqualTo(".NET Framework 4.5"));
			Assert.That(ass.Architecture, Is.EqualTo("Amd64"));
			Assert.That(ass.GlobalAssemblyCache, Is.False);
			Assert.That(ass.Dependencies, Is.Not.Null);
		}

		[Test]
		public void NullLoadTest()
		{
			var ass = AssemblyProber.Create(null);
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.ArgumentError));
			Assert.That(ass.Dependencies, Is.Not.Null);
			Assert.That(ass.Debug, Is.Null);
		}

		[Test]
		public void AssemblyNameLoadingTest()
		{
			var ass = AssemblyProber.Create("AssemblyInfo.Sample.v4.MSIL", true);
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.Success));
			Assert.That(ass.GlobalAssemblyCache, Is.False);
		}

		[Test, Explicit] // TODO: refactor without hardcoded path
		public void LoadByAssemblyNameFromDiffDirectory()
		{
			var curDir = Environment.CurrentDirectory;
			Environment.CurrentDirectory = @"c:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\";
			try
			{
				var ass = AssemblyProber.Create("Microsoft.Data.Entity.Design, Version=11.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", true);
				Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.Success));
				Assert.That(ass.Location, Is.EqualTo(Path.Combine(Environment.CurrentDirectory, "Microsoft.Data.Entity.Design.dll")));
				Assert.That(ass.GlobalAssemblyCache, Is.False);
			}
			finally
			{
				Environment.CurrentDirectory = curDir;
			}
		}

		[Test]
		public void GacLoadingTest()
		{
			var ass = AssemblyProber.Create("System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", true);
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.Success));
			Assert.That(ass.GlobalAssemblyCache, Is.True);
		}

		[Test]
		public void EmptyStringLoadTest()
		{
			var ass = AssemblyProber.Create(@"");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.ArgumentError));
			Assert.That(ass.Dependencies, Is.Not.Null);
			Assert.That(ass.Debug, Is.Null);
		}

		[Test]
		public void UnexistingFileLoadTest()
		{
			var ass = AssemblyProber.Create(@"UnexistingFile.dll");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.FileNotFound));
			Assert.That(ass.Dependencies, Is.Not.Null);
			Assert.That(ass.Debug, Is.Null);

			ass = AssemblyProber.Create(@"System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.FileNotFound));
			Assert.That(ass.Dependencies, Is.Not.Null);
			Assert.That(ass.Debug, Is.Null);
		}

		[Test]
		public void NonDllFileLoadTest()
		{
			var ass = AssemblyProber.Create(@"Samples/NonDllFile.txt");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.ReflectionError));
			Assert.That(ass.Dependencies, Is.Not.Null);
			Assert.That(ass.Debug, Is.Null);

			ass = AssemblyProber.Create(@"Samples/NonDllFile.dll");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.ReflectionError));
			Assert.That(ass.Dependencies, Is.Not.Null);
			Assert.That(ass.Debug, Is.Null);
		}

		[Test]
		public void NonAssemblyDllLoadTest()
		{
			var ass = AssemblyProber.Create(@"Samples/Native.dll");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.ReflectionError));
			Assert.That(ass.Dependencies, Is.Not.Null);
			Assert.That(ass.Debug, Is.Null);
		}

		[Test]
		public void DotNetStandardAssemblyLoadTest()
		{
			var ass = AssemblyProber.Create(@"..\1\netstandard2.0\AssemblyInfo.Sample.Standard.dll");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.Success));
			Assert.That(ass.CLRVersion.StartsWith("v4."));
			Assert.That(ass.TargetFramework, Is.Null);
			Assert.That(ass.Architecture, Is.EqualTo("MSIL"));
			Assert.That(ass.GlobalAssemblyCache, Is.False);
			Assert.That(ass.Dependencies, Is.Not.Null);
		}

		[Test]
		public void DotNetCoreAssemblyLoadTest()
		{
			var ass = AssemblyProber.Create(@"..\1\netcoreapp2.0\AssemblyInfo.Sample.Core.dll");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.Success));
			Assert.That(ass.CLRVersion.StartsWith("v4."));
			Assert.That(ass.TargetFramework, Is.Null);
			Assert.That(ass.Architecture, Is.EqualTo("MSIL"));
			Assert.That(ass.GlobalAssemblyCache, Is.False);
			Assert.That(ass.Dependencies, Is.Not.Null);
		}
	}
}
