using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core
{
    /// <summary>
    /// Repository for interacting with Ideas
    /// </summary>
    public interface IIdeaRepository
    {
        /// <summary>
        /// Retrieves all ideas in the database.
        /// </summary>
        /// <returns>
        /// A collection of ideas.
        /// </returns>
        Task<IEnumerable<Idea>> GetIdeasAsync();

        /// <summary>
        /// Retrieves a single idea from the database.
        /// </summary>
        /// <param name="id">The Idea id to get</param>
        /// <returns>The idea for the specified id.</returns>
        Task<Idea> GetIdeaAsync(long id);

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
        Task<Idea> AddIdeaAsync(Idea idea);

        /// <summary>
        /// Deletes an idea from the database.
        /// </summary>
        /// <param name="id">The Id of the idea to delete</param>
        /// <returns>The idea from the database, just before it's deleted.</returns>
        Task<Idea> DeleteIdeaAsync(long id);

    }
}
