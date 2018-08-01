using CoE.Ideas.Core.Data;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Services
{
    internal class StringTemplateService : IStringTemplateService
    {
        public StringTemplateService(InitiativeContext ideaContext)
        {
            _ideaContext = ideaContext ?? throw new ArgumentNullException("ideaContext");
        }

        private readonly InitiativeContext _ideaContext;

        private static IDictionary<TemplateKey, string> _stringTemplateCache = new Dictionary<TemplateKey, string>();
        public async Task<string> GetStatusChangeTextAsync(InitiativeStatus status, bool isPastTense = false)
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

            return template;

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
