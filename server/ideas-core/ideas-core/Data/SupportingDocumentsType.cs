using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.Data
{
	/// <summary>
	/// Based on the task 8295
	/// The type of SupportingDocumentsType: Business Cases, Technology Investment Form, Other.
	/// </summary>
	public enum SupportingDocumentsType
	{
		/// <summary>
		/// Business Cases
		/// </summary>
		BusinessCases = 1,

		/// <summary>
		/// Technology Investment Form
		/// </summary>
		TechnologyInvestmentForm = 2,

		/// <summary>
		/// Other
		/// </summary>
		Other = 3
	}
}