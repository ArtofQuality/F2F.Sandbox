using System;
using System.Collections.Generic;
using System.Text;

namespace F2F.Sandbox
{
	/// <summary>Interface for a file locator. Required for resolving files in file sandbox.</summary>
	public interface IFileLocator
	{
		/// <summary>Query if 'fileName' exists.</summary>
		/// <param name="fileName">The file to locate.</param>
		/// <returns>true if file exists, false if it fails.</returns>
		bool Exists(string fileName);

		/// <summary>Query all known files.</summary>
		/// <param name="fileName">The file to locate.</param>
		/// <returns>The known files.</returns>
		IEnumerable<string> EnumeratePath(string fileName);

		/// <summary>Deploy 'fileName' to 'destinationPath'.</summary>
		/// <param name="fileName">The file to locate.</param>
		/// <param name="destinationPath">The target file path.</param>
		void CopyTo(string fileName, string destinationPath);
	}
}