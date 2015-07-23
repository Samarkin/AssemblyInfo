using System;
using System.Linq;

namespace AssemblyInfo.Common
{
	[Serializable]
	public class AssemblyDependency
	{
		private readonly string _displayName;
		private readonly bool _satisfied;
		private readonly string _resolvedName;
		private readonly bool _redirected;
		private readonly string _resolvedDifference;
		private AssemblyProber _assemblyProber;

		public AssemblyDependency(string displayName, bool satisfied, string resolvedName)
		{
			if (displayName == null)
			{
				throw new ArgumentNullException("displayName");
			}

			_displayName = displayName;
			_satisfied = satisfied;
			_resolvedName = resolvedName;
			_redirected = resolvedName != null && displayName != resolvedName;

			if (resolvedName != null)
			{
				var displayNameParts = displayName.Split(new [] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();
				var resolvedNameParts = resolvedName.Split(',');
				_resolvedDifference = string.Join(",", resolvedNameParts.Where(p => !displayNameParts.Contains(p.Trim())));
			}
			else
			{
				_resolvedDifference = null;
			}
		}

		public string DisplayName
		{
			get { return _displayName; }
		}

		public bool Satisfied
		{
			get { return _satisfied; }
		}

		public bool Redirected
		{
			get { return _redirected; }
		}

		public string ResolvedName
		{
			get { return _resolvedName; }
		}

		public string ResolvedDifference
		{
			get { return _resolvedDifference; }
		}

		public AssemblyProber Assembly
		{
			get { return _assemblyProber ?? (_assemblyProber = AssemblyProber.Create(_displayName, true)); }
		}
	}
}