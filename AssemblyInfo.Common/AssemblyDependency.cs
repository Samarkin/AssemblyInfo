using System;

namespace AssemblyInfo.Common
{
	[Serializable]
	public class AssemblyDependency
	{
		private readonly string _displayName;
		private readonly bool _satisfied;
		private readonly string _resolvedName;
		private readonly bool _redirected;

		public AssemblyDependency(string displayName, string resolvedName)
		{
			if (displayName == null)
			{
				throw new ArgumentNullException("displayName");
			}

			_displayName = displayName;
			_satisfied = resolvedName != null;
			_resolvedName = resolvedName;
			_redirected = _satisfied && displayName != resolvedName;
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
	}
}