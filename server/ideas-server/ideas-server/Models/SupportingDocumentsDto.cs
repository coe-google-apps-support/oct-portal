
using CoE.Ideas.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoE.Ideas.Server.Models
{
	public class SupportingDocumentsDto
	{

		public int InitiativeId { get; set; }
		public String Title { get; set; }

		public string Url { get; set; }

		public SupportingDocumentsType Type { get; set; }
	}
}