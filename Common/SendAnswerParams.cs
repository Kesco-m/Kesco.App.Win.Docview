using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kesco.App.Win.DocView.Common
{
	public class SendAnswerParams 
	{
		public string AnswerUrl { get; private set; }
		public string AnswerKey { get; private set; }
		public string AnswerControl { get; private set; }
		public string AnswerCommand { get; private set; }
		public System.Windows.Forms.Form Form;

		public SendAnswerParams(string answerControl, string answerKey, string answerUrl, string answerCommand, System.Windows.Forms.Form form = null)
		{
			AnswerControl = answerControl;
			AnswerKey = answerKey;
			AnswerUrl = answerUrl;
			AnswerCommand = answerCommand;
			Form = form;
		}

		
	}
}
