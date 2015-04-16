using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace F2F.Sandbox
{
	/// <summary>Empty file locator. </summary>
	public class EmptyFileLocator : IFileLocator
	{
		/// <summary>
		/// Default ctor.
		/// </summary>
		public EmptyFileLocator()
		{
		}

		/// <summary>
		/// See <see cref="F2F.Sandbox.IFileLocator.Exists(string)"/>
		/// </summary>
		public bool Exists(string fileName)
		{
			return false;
		}

		/// <summary>
		/// See <see cref="F2F.Sandbox.IFileLocator.EnumeratePath(string)"/>
		/// </summary>
		public IEnumerable<string> EnumeratePath(string path)
		{
			return new string[0];
		}

		/// <summary>
		/// See
		/// <see cref="F2F.Sandbox.IFileLocator.CopyTo(string, string)"/>
		/// </summary>
		public void CopyTo(string fileName, string destinationPath)
		{
		}
	}
}