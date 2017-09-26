﻿using Tailviewer.BusinessLogic.Analysis;

namespace Tailviewer.Ui.Controls.MainPanel.Analyse.Widgets.QuickInfo
{
	public class QuickInfoWidgetViewModel
		: AbstractWidgetViewModel
	{
		public QuickInfoWidgetViewModel(IDataSourceAnalyser dataSourceAnalyser)
			: base(dataSourceAnalyser)
		{}
	}
}