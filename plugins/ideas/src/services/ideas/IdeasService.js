import { HTTP } from '../../HttpCommon'

/**
 * Allows for easily pulling ideas.
 * Idea format:
 * idea.id
 * idea.url
 * idea.title
 * idea.description
 * idea.createdDate
 * idea.stakeholders
 * idea.stakeholders[0].userName
 */
let x = class IdeasService {
  /**
   * Returns a Promise that resolves with a list of ideas.
   * @param {Number} page The 1-indexed page number.
   * @param {Number} pageSize The number of results to return.
   * @param {String} contains A search string to apply.
   * @returns {Promise} Resolved with an array of ideas.
   */
  static getIdeas (page, pageSize, contains) {
    return HTTP.get('/plugins/initiatives/api', {
      params: {
        page,
        pageSize,
        contains
      }
    })
  }

  /**
   * Returns a Promise that resolves with a list of my initiatives.
   * @param {Number} page The 1-indexed page number.
   * @param {Number} pageSize The number of results to return.
   * @param {String} contains A search string to apply.
   * @returns {Promise} Resolved with an array of my initiatives.
   */
  static getMyInitiatives (page, pageSize, contains) {
    return HTTP.get('/plugins/initiatives/api', {
      params: {
        view: 'Mine',
        page,
        pageSize,
        contains
      }
    })
  }

  /**
   * Returns a Promise that resolves with an initiative.
   * @param {string} id The id of the initiative.
   * @returns {Promise} Resolved with an initiative.
   */
  static getInitiative (id) {
    return HTTP.get(`/plugins/initiatives/api/${id}`).then((response) => {
      return response.data
    }, (err) => {
      console.error(`Failed at route /${id}`)
      console.error(err)
    })
  }

  /**
   * Returns a Promise that resolves with an initiative.
   * @param {string} slug The slug of the initiative.
   * @returns {Promise} Resolved with an initiative.
   */
  static getInitiativeBySlug (slug) {
    return HTTP.get(`/plugins/initiatives/api/${slug}?type=slug`).then((response) => {
      return response.data
    }, (err) => {
      console.error(`Failed at route /${slug}?type=slug`)
      console.error(err)
    })
  }

  /**
   * Returns a Promise that resolves with an initiatives steps.
   * @param {string} id The id of the initiative.
   * @returns {Promise} Resolved with an initiatives steps.
   */
  static getInitiativeSteps (id) {
    return HTTP.get(`/plugins/initiatives/api/${id}/steps`).then((response) => {
      if (response.data) {
        return response.data
      } else {
        return response
      }
    }, (err) => {
      console.error(`Failed at route /${id}/steps.`)
      console.error(err)
    })
  }

  /**
   * Creates a new supporting document for a specified initiative.
   * @param {string} id The id of the initiative
   * @param {string} title The title of the supporting document.
   * @param {string} url The url of the supporting document.
   * @param {string} type The type of the supporting document.
   */
  static createSupportingDoc (id, title, url, type) {
    return HTTP.post(`/plugins/initiatives/api/${id}/supportingdocuments`, {
      title,
      url,
      type
    })
  }
  /**
   * Returns a Promise that resolves with the supporting document(s).
   * @param {string} id The id of the initiative.
   * @returns {Promise} Resolved with the supporting document(s).
   */
  static getSupportingDoc (id) {
    return HTTP.get(`/plugins/initiatives/api/${id}/supportingdocuments`).catch((err) => {
      console.error(`Failed at route /${id}/supportingdocuments`)
      console.error(err)
    })
  }

  /**
   * Creates a new initiative.
   * @param {string} title The title of the initiative.
   * @param {string} description The description of the initiative.
   * @param {string} supportingDocuments A list of supporting documents of the initiative.
   * @param {string} businessSponsorEmail The email of the business sponsor.
   * @param {boolean} hasBudget Whether there is budget for this or not.
   * @param {Date} expectedTargetDate When should this be delivered?
   */
  static createInitiative (title, description, supportingDocuments, businessSponsorEmail, hasBudget, expectedTargetDate) {
    return HTTP.post('/plugins/initiatives/api', {
      title,
      description,
      supportingDocuments,
      businessSponsorEmail,
      hasBudget,
      expectedTargetDate
    })
  }

  /**
   * Gets the assignee for the given initiative.
   * @param {string} id The id of the initiative.
   * @return {Promise} A Promise that resolves with the information of the assignee.
   */
  static getAssignee (id) {
    return HTTP.get(`/plugins/initiatives/api/${id}/assignee`).then((assignee) => {
      return assignee
    }, (issue) => {
      if (issue.response && issue.response.status === 404) {
        console.log('no assignee')
        return null
      } else {
        console.error(issue)
      }
    })
  }

  /**
   * Gets the resources for the given initiative.
   * @param {string} id The id of the initiative.
   * @return {Promise} A Promise that resolves with the resources.
   */
  static getResources (id) {
    return HTTP.get(`/plugins/initiatives/api/${id}/resources`).then((response) => {
      console.log(response)
      return response.data
    }, (issue) => {
      if (issue.response && issue.response.status === 404) {
        console.log('no resources')
        return null
      } else {
        console.error(issue)
      }
    })
  }

  /**
   * Causes an initiative to be updated.
   * @param {Object} initiative The initiative to update.
   * @return {Promise} A promise resolved with the updated initiative.
   */
  static updateInitiative (initiative) {
    return HTTP.put(`/plugins/initiatives/api/${initiative.id}`, initiative)
  }

  /**
   * Updates a business case.
   * @param {String} id The id of the initiative.
   * @param {Object} url The url of the business case.
   * @return {Promise} A promise resolved with the url if successful.
   */
  static updateBusinessCase (id, url) {
    return HTTP.put(`/plugins/initiatives/api/${id}/businessCase`, {
      businessCaseUrl: url
    })
  }

  /**
   * Updates a investment form Url.
   * @param {String} id The id of the initiative.
   * @param {Object} url The url of the investment form.
   * @return {Promise} A promise resolved with the url if successful.
   */
  static updateInvestmentForm (id, url) {
    return HTTP.put(`/plugins/initiatives/api/${id}/investmentForm`, {
      investmentRequestFormUrl: url
    })
  }

  /**
   * Updates a steps status description.
   * @param {String} id The id of the initiative.
   * @param {String} stepId The id of the step.
   * @param {String} newDescription The new description.
   * @return {Promise} A promise resolved if the operation was successful.
   */
  static updateStatusDescription (id, stepId, newDescription) {
    return HTTP.put(`/plugins/initiatives/api/${id}/statusDescription`, { stepId, newDescription }).then(() => {
      console.log('Success')
    }, (err) => {
      console.error(`Failed at route ${id}/statusDescription`)
      throw err
    })
  }
}

export const IdeasService = x
