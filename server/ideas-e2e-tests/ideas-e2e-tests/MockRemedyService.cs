﻿using CoE.Ideas.Core;
using CoE.Ideas.Core.WordPress;
using CoE.Ideas.Remedy;
using RemedyServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.EndToEnd.Tests
{
    internal class MockRemedyService : IRemedyService
    {
        public MockRemedyService()
        {

        }

        private ICollection<OutputMapping1GetListValues> remedyItems = new List<OutputMapping1GetListValues>();
        public ICollection<OutputMapping1GetListValues> Items
        {
            get
            {
                return remedyItems;
            }
        }

        public Task<string> PostNewIdeaAsync(Idea idea, WordPressUser user, string user3and3)
        {
            var newRemedyItem = new OutputMapping1GetListValues();
            newRemedyItem.WorkOrderID = Guid.NewGuid().ToString();
            newRemedyItem.Description = idea.Title;
            newRemedyItem.Detailed_Description = idea.Description;
            remedyItems.Add(newRemedyItem);

            return Task.FromResult(newRemedyItem.WorkOrderID);
        }

    }
}
