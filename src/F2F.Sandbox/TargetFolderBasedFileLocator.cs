using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace F2F.Sandbox
{
	/// <summary>
	/// File locator that locates files from the target output directory (debug/rel/...).
	/// The folder is
	/// </summary>
	public class TargetFolderBasedFileLocator : FolderBasedFileLocator
	{
		/// <summary>
		/// Creates a new instance looking up the target folder by resolving the given type's assembly location.
		/// </summary>
		/// <param name="typeInRootNamespace"></param>
		public TargetFolderBasedFileLocator(Type typeInRootNamespace)
			: base(Path.GetDirectoryName(typeInRootNamespace.Assembly.Location))
		{
		}
	}
}