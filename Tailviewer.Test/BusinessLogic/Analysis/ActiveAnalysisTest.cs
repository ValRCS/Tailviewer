﻿using System;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Tailviewer.BusinessLogic.Analysis;
using Tailviewer.BusinessLogic.LogFiles;
using Tailviewer.Core.Analysis;

namespace Tailviewer.Test.BusinessLogic.Analysis
{
	[TestFixture]
	public sealed class ActiveAnalysisTest
	{
		private ManualTaskScheduler _taskScheduler;
		private Mock<ILogAnalyserEngine> _analysisEngine;
		private AnalysisTemplate _template;

		[SetUp]
		public void Setup()
		{
			_taskScheduler = new ManualTaskScheduler();
			_analysisEngine = new Mock<ILogAnalyserEngine>();
			_analysisEngine.Setup(x => x.CreateAnalysis(It.IsAny<ILogFile>(),
					It.IsAny<DataSourceAnalysisConfiguration>(),
					It.IsAny<IDataSourceAnalysisListener>()))
				.Returns(() => new Mock<IDataSourceAnalysisHandle>().Object);

			_template = new AnalysisTemplate();
		}

		[Test]
		public void TestAdd1()
		{
			var activeAnalysis = new ActiveAnalysis(AnalysisId.CreateNew(), _template, _taskScheduler, _analysisEngine.Object, TimeSpan.Zero);
			_template.Analysers.Should().BeEmpty();

			var configuration = new Mock<ILogAnalyserConfiguration>().Object;
			var analyser = activeAnalysis.Add(new LogAnalyserFactoryId("foobar"), configuration);

			_template.Analysers.Should().HaveCount(1);
			var template = _template.Analysers.First();
			template.Id.Should().Be(analyser.Id);
			template.FactoryId.Should().Be(new LogAnalyserFactoryId("foobar"));
			template.Configuration.Should().Be(configuration);
		}

		[Test]
		public void TestTryGetAnalyser()
		{
			var activeAnalysis = new ActiveAnalysis(AnalysisId.CreateNew(), _template, _taskScheduler, _analysisEngine.Object, TimeSpan.Zero);
			_template.Analysers.Should().BeEmpty();

			var configuration = new Mock<ILogAnalyserConfiguration>().Object;
			var analyser = activeAnalysis.Add(new LogAnalyserFactoryId("foobar"), configuration);
			activeAnalysis.TryGetAnalyser(analyser.Id, out var actualAnalyser).Should().BeTrue();
			actualAnalyser.Should().BeSameAs(analyser);
		}

		[Test]
		public void TestTryGetNonExistentAnalyser()
		{
			var activeAnalysis = new ActiveAnalysis(AnalysisId.CreateNew(), _template, _taskScheduler, _analysisEngine.Object, TimeSpan.Zero);
			_template.Analysers.Should().BeEmpty();

			var configuration = new Mock<ILogAnalyserConfiguration>().Object;
			activeAnalysis.Add(new LogAnalyserFactoryId("foobar"), configuration);
			activeAnalysis.TryGetAnalyser(AnalyserId.CreateNew(), out var actualAnalyser).Should().BeFalse();
			actualAnalyser.Should().BeNull();
		}

		[Test]
		public void TestAddRemove1()
		{
			var activeAnalysis = new ActiveAnalysis(AnalysisId.CreateNew(), _template, _taskScheduler, _analysisEngine.Object, TimeSpan.Zero);
			_template.Analysers.Should().BeEmpty();

			var analyser = activeAnalysis.Add(new LogAnalyserFactoryId("foobar"), null);
			_template.Analysers.Should().HaveCount(1);

			activeAnalysis.Remove(analyser);
			_template.Analysers.Should().BeEmpty();
		}

		[Test]
		public void TestDispose()
		{
			var activeAnalysis = new ActiveAnalysis(AnalysisId.CreateNew(), _template, _taskScheduler, _analysisEngine.Object, TimeSpan.Zero);
			activeAnalysis.Dispose();

			_taskScheduler.PeriodicTaskCount.Should()
				.Be(0, "because all tasks should've been stopped when the group is disposed of");
		}
	}
}