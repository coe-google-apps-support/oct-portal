import { HTTP } from '../../HttpCommon'

/**
 * Allows for easily pulling user information.
 * user.email: The user's email address.
 * user.id: The user's id.
 * user.name: The user's name.
 * user.roles: An array of strings that represent roles.
 * user.permissions: An array of strings that represent permissions.
 */
let x = class UserService {
  // Used to cache auth requests.
  me = null;

  /**
   * Returns a Promise that resolves with the current user.
   * This function caches this response value to speed up the application.
   * @returns {Promise} Resolved with the current user.
   */
  static getMe () {
    if (!UserService.me) {
      return HTTP.get('/user').then((response) => {
        UserService.me = response.data
        return response.data
      })
    } else {
      return Promise.resolve(UserService.me)
    }
  }
}

export const UserService = x
