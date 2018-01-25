/* eslint-disable */
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

    constructor() {
        
    }

    /**
     * Returns a Promise that resolves with a list of ideas.
     * @returns {Promise} Resolved with an array of ideas.
     */
    static getIdeas() {
      return HTTP.get('')
    }   

    /**
     * Returns a Promise that resolves with an initiative.
     * @param {string} id The id of the initiative.
     * @returns {Promise} Resolved with an initiative.
     */
    static getInitiative(id) {
        return HTTP.get(`/${id}`)
    }

    /**
     * Returns a Promise that resolves with an initiatives steps.
     * @param {string} id The id of the initiative.
     * @returns {Promise} Resolved with an initiatives steps.
     */
    static getInitiativeSteps(id) {
      return HTTP.get(`/${id}/steps`)
    }  
}

export const IdeasService = x