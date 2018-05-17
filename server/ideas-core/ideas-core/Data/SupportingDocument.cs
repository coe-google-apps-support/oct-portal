
using CoE.Ideas.Shared.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.Data
{
	public class SupportingDocument : Entity<int>
	{


		private SupportingDocument() : base() { }

		public string Title { get; private set; }
		public void SetTitle(string newTitle)
		{
			Title = newTitle;
		}

		public string URL { get; private set; }
		public void SetURL(string newURL)
		{
			URL = newURL;
		}

		public SupportingDocumentsType Type { get; private set; }

		public void SetType(SupportingDocumentsType newType)
		{
			Type = newType;
		}

		public static SupportingDocument Create(string title, string url, SupportingDocumentsType type)
		{
			return new SupportingDocument() {Title = title, URL = url, Type = type};
		}
	}
}