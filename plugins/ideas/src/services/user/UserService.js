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
let x = class UserService {
  /**
   * Returns a Promise that resolves with a list of ideas.
   * @returns {Promise} Resolved with an array of ideas.
   */
  static getMe () {
    return HTTP.get('/user')
  }
}

export const UserService = x
