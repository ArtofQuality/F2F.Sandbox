using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace F2F.Sandbox
{
	/// <summary>Resource file locator. </summary>
	public class ResourceFileLocator : IFileLocator
	{
		private readonly string _namespace;

		private readonly Assembly _assembly;

		/// <summary>Initializes the file locator with the given type.</summary>
		/// <param name="typeInRootNamespace">The type in root namespace.</param>
		public ResourceFileLocator(Type typeInRootNamespace)
		{
			_namespace = typeInRootNamespace.Namespace;
			_assembly = typeInRootNamespace.Assembly;
		}

		/// <summary>
		/// See <see cref="F2F.Sandbox.IFileLocator.Exists(string)"/>
		/// </summary>
		public bool Exists(string fileName)
		{
			string resourceName = GetFullResourceName(fileName);

			return _assembly.GetManifestResourceStream(resourceName) != null;
		}

		/// <summary>
		/// See <see cref="F2F.Sandbox.IFileLocator.EnumeratePath(string)"/>
		/// </summary>
		public IEnumerable<string> EnumeratePath(string path)
		{
			if (!String.IsNullOrEmpty(path) && path[0].Equals('.')) path = path.TrimStart('.');
			string resourceName = GetFullResourceName(path);

			// TODO Replace this with LINQ when > .NET 2.0
			List<string> result = new List<string>();

			foreach (string name in _assembly.GetManifestResourceNames())
			{
				if (name.StartsWith(resourceName))
				{
					var resourceFile = GetPathFromResource(name);
					var normalizedDirectoryPath = NormalizeDirectoryPath(path);
					if (!String.IsNullOrEmpty(resourceFile) && resourceFile.StartsWith(normalizedDirectoryPath))
					{
						result.Add(resourceFile);
					}
				}
			}

			return result;
		}

		/// <summary>
		/// See <see cref="F2F.Sandbox.IFileLocator.CopyTo(string, string)"/>
		/// </summary>
		public void CopyTo(string fileName, string destinationPath)
		{
			string resourceName = GetFullResourceName(fileName);

			using (Stream sr = _assembly.GetManifestResourceStream(resourceName))
			using (Stream sw = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
			{
				CopyTo(sr, sw);
			}
		}

		private string GetFullResourceName(string fileName)
		{
			string resName = fileName.Replace('/', '.').Replace('\\', '.');
			string fullResourceName = String.Format("{0}.{1}", _namespace, resName);
			return fullResourceName;
		}

		private string GetPathFromResource(string resourceName)
		{
			string path = String.Empty;

			resourceName = resourceName.Replace(_namespace + ".", "");

			if (!resourceName.Equals("Properties.Resources.resources"))
			{
				path = ReplaceLast(resourceName, ".", "^");
				path = path.Replace('.', '\\');
				path = path.Replace('^', '.');
			}

			return path;
		}

		private string NormalizeDirectoryPath(string path)
		{
			string result = String.Empty;
			if (!String.IsNullOrEmpty(path))
			{
				result = path.Replace('/', Path.DirectorySeparatorChar);
				if (result[result.Length - 1] != Path.DirectorySeparatorChar)
				{
					result = String.Format("{0}{1}", result, Path.DirectorySeparatorChar);
				}
			}
			return result;
		}

		private static void CopyTo(Stream input, Stream output)
		{
			byte[] buffer = new byte[32768];
			int read;
			while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
			{
				output.Write(buffer, 0, read);
			}
		}

		private static string ReplaceLast(string text, string search, string replace)
		{
			int pos = text.LastIndexOf(search);
			if (pos < 0)
			{
				return text;
			}
			return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
		}
	}
}