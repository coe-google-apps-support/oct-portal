using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core
{
    public interface IUpdatableIdeaRepository : IIdeaRepository
    {
        #region Initiatives

        /// <summary>
        /// Updates an idea.
        /// </summary>
        /// <param name="idea">The idea to update</param>
        /// <returns>The idea after being updated</returns>
        Task<Idea> UpdateIdeaAsync(Idea idea);

        /// <summary>
        /// Adds a new idea to the database.
        /// </summary>
        /// <param name="idea">The idea to add.</param>
        /// <returns>The idea after its been added to the data (with Id)</returns>
        Task<Idea> AddIdeaAsync(Idea idea, ClaimsPrincipal owner);

        Task<Idea> SetWordPressItemAsync(long ideaId, WordPress.WordPressPost post);

        /// <summary>
        /// Deletes an idea from the database.
        /// </summary>
        /// <param name="id">The Id of the idea to delete</param>
        /// <returns>The idea from the database, just before it's deleted.</returns>
        Task<Idea> DeleteIdeaAsync(long id);

        Task<Idea> SetWorkItemTicketIdAsync(long id, string workItemId);

        Task<Idea> SetWorkItemStatusAsync(long id, InitiativeStatus status);

        Task<Idea> SetInitiativeAssignee(long ideaId, Person person);
        #endregion

        #region Tags
        /// <summary>
        /// Updates a tag.
        /// </summary>
        /// <param name="tag">The tag to update</param>
        /// <returns>The tag after being updated</returns>
        Task<Tag> UpdateTagAsync(Tag tag);

        /// <summary>
        /// Adds a new tag to the database.
        /// </summary>
        /// <param name="tag">The tag to add.</param>
        /// <returns>The tag after its been added to the data (with Id)</returns>
        Task<Tag> AddTagAsync(Tag tag);

        /// <summary>
        /// Deletes a tag from the database.
        /// </summary>
        /// <param name="id">The Id of the tag to delete</param>
        /// <returns>The tag from the database, just before it's deleted.</returns>
        Task<Tag> DeleteTagAsync(long id);

        #endregion
    }
}
