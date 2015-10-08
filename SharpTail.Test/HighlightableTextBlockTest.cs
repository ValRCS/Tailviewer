﻿using System;
using System.Linq;
using System.Windows.Documents;
using FluentAssertions;
using NUnit.Framework;
using SharpTail.Ui.Controls;
using SharpTail.Ui.ViewModels;

namespace SharpTail.Test
{
	[TestFixture]
	public sealed class HighlightableTextBlockTest
	{
		[Test]
		[STAThread]
		public void TestFilterString1()
		{
			var block = new HighlightableTextBlock
				{
					LogEntry = new LogEntryViewModel("This is a test"),
					FilterString = "a"
				};
			block.Inlines.Count.Should().Be(3);
			block.Inlines.Select(x => ((Run) x).Text)
			     .Should().Equal(new[]
				     {
					     "This is ",
					     "a",
					     " test"
				     });
		}
	}
}