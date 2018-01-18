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
        #region Ideas
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
        /// Retrieves a single idea from the database.
        /// </summary>
        /// <param name="id">The id of the wordpress post relating to an idea</param>
        /// <returns>The idea for the specified id.</returns>
        Task<Idea> GetIdeaByWordpressKeyAsync(int id);

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
        #endregion

        #region Tags
        /// <summary>
        /// Retrieves all tags in the database.
        /// </summary>
        /// <returns>
        /// A collection of tags.
        /// </returns>
        Task<IEnumerable<Tag>> GetTagsAsync();

        /// <summary>
        /// Retrieves a single tag from the database.
        /// </summary>
        /// <param name="id">The Idea id to get</param>
        /// <returns>The idea for the specified id.</returns>
        Task<Tag> GetTagAsync(long id);

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

        #region Branches and Tags
        /// <summary>
        /// Retrieves all branches in the database.
        /// </summary>
        /// <returns>
        /// A collection of branches.
        /// </returns>
        Task<IEnumerable<Branch>> GetBranchesAsync();

        /// <summary>
        ///  Retrieves a single branch from the database.
        /// </summary>
        /// <param name="id">The Branch id to get</param>
        /// <returns>The branch for the specified id.</returns>
        Task<Branch> GetBranchAsync(long id);

        /// <summary>
        /// Retrieves all departments in a given branch
        /// </summary>
        /// <returns>
        /// A collection of departments.
        /// </returns>
        Task<IEnumerable<Department>> GetDepartmentsForBranchAsync(long branchId);


        #endregion
    }
}
