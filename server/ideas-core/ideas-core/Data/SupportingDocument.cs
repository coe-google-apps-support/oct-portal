
using CoE.Ideas.Shared.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.Data
{
	public class SupportingDocument : Entity<int>
	{


		private SupportingDocument() : base() { }

		public int InitiativeId { get; private set; }
		public void SetInitiativeId(int newInitiativeId)
		{
			InitiativeId = newInitiativeId;
		}

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

		public static SupportingDocument Create(int initiativeId, string title, string url, SupportingDocumentsType type)
		{
			return new SupportingDocument() { InitiativeId = initiativeId, Title = title, URL = url, Type = type};
		}
	}
}