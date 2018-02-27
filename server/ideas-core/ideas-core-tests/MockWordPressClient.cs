using CoE.Ideas.Core.WordPress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core.Tests
{
    public class MockWordPressClient : IWordPressClient
    {
        public MockWordPressClient()
        {
            Posts = new List<WordPressPost>();
        }


        public ICollection<WordPressPost> Posts { get; private set; }
        public ClaimsPrincipal User { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private static WordPressUser _mockWordPressUser = new WordPressUser()
        {
            Id = 1,
            Name = "Snow White",
            FirstName = "Snow",
            LastName = "White",
            Description = "Snow White",
            Email = "snow.white@edmonton.ca",
            Username = "snow.white@edmonton.ca",
            Nickname = "snow-white",
            Roles = new string[] { "Subscriber" },
            Locale = "en-CA",
            Url = new Uri("https://octportal.edmonton.ca/author/snow-white/")
        };

        public Task<WordPressUser> GetCurrentUserAsync()
        {
            return Task.FromResult(_mockWordPressUser);
        }

        public Task<WordPressUser> GetUserAsync(int wordPressuserId)
        {
            if (wordPressuserId == _mockWordPressUser.Id)
                return Task.FromResult(_mockWordPressUser);
            else
                throw new KeyNotFoundException($"MockWordPressClient only supports 1 user which has id { _mockWordPressUser.Id }. '{ wordPressuserId }' was requested.");
        }

        private static int _wordPressPostCount = 0;
        public Task<WordPressPost> PostIdeaAsync(Idea idea)
        {
            var publishedDate = new DateTime();
            var newPost = new WordPressPost()
            {
                Id = ++_wordPressPostCount,
                Author = _mockWordPressUser.Id,
                CategoryIds = new int[] { },
                Date = publishedDate,
                DateGmt = publishedDate.ToUniversalTime(),
                Link = "https://octportal.edmonton.ca/initiatives/andrews-idea/",
                Type = "Initiative",
                Slug = GenerateSlug(idea)
            };
            Posts.Add(newPost);
            return Task.FromResult(newPost);
        }

        private string GenerateSlug(Idea idea)
        {
            if (idea == null)
                return string.Empty;
            if (string.IsNullOrWhiteSpace(idea.Title))
                return string.Empty;
            return idea.Title.Replace(" ", "_");
        }

        public Task<WordPressPost> GetPostForInitativeSlug(string slug)
        {
            return Task.FromResult(Posts.SingleOrDefault(x => x.Slug == slug));
        }
    }
}
