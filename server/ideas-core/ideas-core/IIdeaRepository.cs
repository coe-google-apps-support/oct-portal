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
        /// Retrieves all ideas where the current user is a stakeholder
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Idea>> GetIdeasByStakeholderEmailAsync(string emailAddress);

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
        /// Retrieves a single idea from the database.
        /// </summary>
        /// <param name="id">The id of the work item relating to an idea</param>
        /// <returns>The idea for the specified id.</returns>
        Task<Idea> GetIdeaByWorkItemIdAsync(string workItemId);
        #endregion

        #region People
        Task<Person> GetPersonByEmail(string email);
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

        #endregion


        #region Branches and Departments
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
