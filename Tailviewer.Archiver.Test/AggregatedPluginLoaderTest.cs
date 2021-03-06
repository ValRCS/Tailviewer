﻿using FluentAssertions;
using Moq;
using NUnit.Framework;
using Tailviewer.Archiver.Plugins;
using Tailviewer.BusinessLogic.Analysis;
using Tailviewer.Ui.Analysis;

namespace Tailviewer.Archiver.Test
{
	[TestFixture]
	public sealed class AggregatedPluginLoaderTest
	{
		[Test]
		public void TestLoadPluginEmpty()
		{
			var loader = new AggregatedPluginLoader();
			loader.LoadAllOfType<IWidgetPlugin>().Should().BeEmpty();
			loader.LoadAllOfType<ILogAnalyserPlugin>().Should().BeEmpty();
		}

		[Test]
		public void TestLoadTwoLoaders()
		{
			var loader1 = new PluginRegistry();
			var plugin1 = CreateWidgetPlugin();
			loader1.Register(plugin1);

			var loader2 = new PluginRegistry();
			var plugin2 = CreateWidgetPlugin();
			loader2.Register(plugin2);

			var aggregatedLoader = new  AggregatedPluginLoader(loader1, loader2);
			var plugins = aggregatedLoader.LoadAllOfType<IWidgetPlugin>();
			plugins.Should().BeEquivalentTo(plugin1, plugin2);
		}

		private IWidgetPlugin CreateWidgetPlugin()
		{
			var plugin = new Mock<IWidgetPlugin>();
			return plugin.Object;
		}
	}
}
