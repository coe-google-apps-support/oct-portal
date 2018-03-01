using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Internal.Initiatives
{
    internal class StringTemplateService : IStringTemplateService
    {
        public StringTemplateService(IdeaContext ideaContext)
        {
            _ideaContext = ideaContext ?? throw new ArgumentNullException("ideaContext");
        }

        private readonly IdeaContext _ideaContext;

        private static IDictionary<TemplateKey, string> _stringTemplateCache = new Dictionary<TemplateKey, string>();
        public async Task<string> GetStatusChangeTextAsync(InitiativeStatusInternal status, PersonInternal assignee, bool isPastTense = false)
        {
            TemplateKey key = new TemplateKey()
            {
                Category = StringTemplateCategory.StatusDescription,
                Key = string.Concat(status.ToString(), isPastTense ? "_past" : "_present")
            };
            string template = _stringTemplateCache.ContainsKey(key) ? _stringTemplateCache[key] : null;
            if (string.IsNullOrWhiteSpace(template))
            {
                template = await _ideaContext.StringTemplates
                    .Where(x => x.Category == key.Category && x.Key == key.Key)
                    .Select(x => x.Text)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrWhiteSpace(template))
                    throw new KeyNotFoundException($"Unable to find a StringTemplate with category {key.Category} and key {key.Key}");

                _stringTemplateCache[key] = template;
            }

            string assigneeName = string.IsNullOrWhiteSpace(assignee?.UserName)
                ? "A representative" : assignee.UserName;

            return string.Format(template, assigneeName);
        }


        private class TemplateKey
        {
            public StringTemplateCategory Category { get; set; }
            public string Key { get; set; }

            public override bool Equals(object obj)
            {
                var obj2 = obj as TemplateKey;
                if (obj2 == null)
                    return false;

                return string.Equals(Key, obj2.Key) && Category == obj2.Category;
            }

            public override int GetHashCode()
            {
                int hash = 0;
                if (!string.IsNullOrWhiteSpace(Key))
                    hash = Key.GetHashCode();

                hash = hash ^ Category.GetHashCode();
                return hash;
            }
        }

    }
}
