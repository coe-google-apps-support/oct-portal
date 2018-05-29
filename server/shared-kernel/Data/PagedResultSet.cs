using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Shared.Data
{
    public class PagedResultSet<T>
    {
        internal PagedResultSet() { }

        public int PageNumber { get; internal set; }
        public int PageSize { get; internal set; }
        public int ResultCount { get; internal set; }
        public IEnumerable<T> Results { get; internal set; }
        public int TotalCount { get; internal set; }
        public bool IsLastPage()
        {
            return (PageNumber - 1) * PageSize + ResultCount >= TotalCount;
        }


    }

    public static class PagedResultSet
    {
        public static PagedResultSet<T> Create<T>(IEnumerable<T> results, int pageNumber, int pageSize, int resultCount, int totalNumber)
        {
            var returnValue = new PagedResultSet<T>();
            returnValue.PageNumber = pageNumber;
            returnValue.PageSize = pageSize;
            returnValue.ResultCount = resultCount;
            returnValue.TotalCount = totalNumber;
            returnValue.Results = results;
            return returnValue;
        }
    }


}
