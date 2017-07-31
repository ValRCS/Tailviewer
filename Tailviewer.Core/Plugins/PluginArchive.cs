﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Tailviewer.Core.Plugins
{
	/// <summary>
	///     Responsible for loading a plugin from a tailviewer plugin package.
	/// </summary>
	public sealed class PluginArchive
		: IDisposable
	{
		private readonly ZipArchive _archive;
		private readonly PluginPackageIndex _index;
		private readonly Dictionary<ZipArchiveEntry, Assembly> _assemblyCache;

		private PluginArchive(ZipArchive archive)
		{
			if (archive == null)
				throw new ArgumentNullException(nameof(archive));

			var sw = new Stopwatch();

			_archive = archive;
			_index = new PluginPackageIndex();
			_assemblyCache = new Dictionary<ZipArchiveEntry, Assembly>();

			var index = _archive.GetEntry("index.xml");
			using (var stream = index.Open())
			using (var reader = new StreamReader(stream))
			{
				var serializer = new XmlSerializer(typeof(PluginPackageIndex));
				sw.Restart();
				_index = serializer.Deserialize(reader) as PluginPackageIndex;
			}
			sw.Stop();
			Console.WriteLine("Deserialize index: {0}ms", sw.ElapsedMilliseconds);
		}

		public IPluginPackageIndex Index => _index;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entryName"></param>
		/// <returns></returns>
		public Assembly LoadAssembly(string entryName)
		{
			var entry = _archive.GetEntry(entryName);

			Assembly assembly;
			if (!_assemblyCache.TryGetValue(entry, out assembly))
			{
				var rawAssembly = new byte[entry.Length];
				using (var stream = entry.Open())
				{
					int read = 0;
					int offset = 0;
					int size;
					do
					{
						offset += read;
						size = Math.Min(4096, rawAssembly.Length - offset);
					} while ((read = stream.Read(rawAssembly, offset, size)) > 0);
				}
				assembly = Assembly.Load(rawAssembly);
				_assemblyCache.Add(entry, assembly);
			}

			return assembly;
		}

		/// <inheritdoc />
		public void Dispose()
		{
			_archive?.Dispose();
		}

		/// <summary>
		/// </summary>
		/// <param name="fname"></param>
		/// <returns></returns>
		public static PluginArchive OpenRead(string fname)
		{
			return new PluginArchive(ZipFile.OpenRead(fname));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="leaveOpen"></param>
		/// <returns></returns>
		public static PluginArchive OpenRead(Stream stream, bool leaveOpen = false)
		{
			return new PluginArchive(new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen, Encoding.UTF8));
		}
	}
}