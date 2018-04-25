const QUERY_TIMEOUT = 1000

/**
 * Allows for easily pulling user information.
 * user.email: The user's email address.
 * user.id: The user's id.
 * user.name: The user's name.
 * user.roles: An array of strings that represent roles.
 * user.permissions: An array of strings that represent permissions.
 */
let x = class StubbedUserService {
  // Used to cache auth requests.
  me = {
    email: 'dev.person@edmonton.ca',
    id: 77,
    name: 'Dev Person',
    roles: [
      'horse_whisperer',
      'drain_unclogger'
    ],
    permissions: [
      'canWhisperHorses',
      'canUnclogDrains',
      'canEditStatusDescriptions'
    ]
  }

  /**
   * Returns a Promise that resolves with the current user.
   * This function caches this response value to speed up the application.
   * @returns {Promise} Resolved with the current user.
   */
  static getMe () {
    return new Promise((resolve, reject) => {
      setTimeout(() => {
        resolve(StubbedUserService.me)
      }, QUERY_TIMEOUT)
    })
  }
}

export const StubbedUserService = x
