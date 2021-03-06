﻿using System;
using System.Windows.Input;

namespace Tailviewer.Ui.Controls.ActionCenter
{
	public interface INotificationViewModel
	{
		void Update();
		event Action<INotificationViewModel> OnRemove;
		ICommand RemoveCommand { get; }
		string Title { get; }
		bool IsRead { get; }
	}
}