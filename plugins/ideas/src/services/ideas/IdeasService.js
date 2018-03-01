import { HTTP } from '../../HttpCommon'

const fakeInitiativeSteps = {
  data: [{
    step: 1,
    name: 'Submit',
    status: 'done',
    completedDate: 'Jan 14 2018 9:17:00 GMT-0700 (Mountain Standard Time)',
    type: 'text',
    data: 'Congrats! You submitted this on January 14th, 2018.'
  },
  {
    step: 2,
    name: 'Review',
    status: 'done',
    completedDate: 'Jan 14 2018 11:17:00 GMT-0700 (Mountain Standard Time)',
    type: 'resource',
    data: [{
      user: 'super.ba@edmonton.ca',
      assignedOn: 'Jan 14 2018 10:55:32 GMT-0700 (Mountain Standard Time)',
      avatarURL: `${process.env.STATIC_ASSETS}/assets/avatar/avatar1.png`
    }
    ]
  },
  {
    step: 3,
    name: 'Collaborate',
    status: 'done',
    completedDate: 'Jan 18 2018 11:17:00 GMT-0700 (Mountain Standard Time)',
    type: 'resource',
    data: [{
      user: 'super.ba@edmonton.ca',
      assignedOn: 'Jan 14 2018 12:31:55 GMT-0700 (Mountain Standard Time)',
      avatarURL: `${process.env.STATIC_ASSETS}/assets/avatar/avatar1.png`
    }
    ]
  },
  {
    step: 4,
    name: 'Deliver',
    status: 'ongoing',
    completedDate: null,
    type: 'burndown',
    url: 'https://github.com/coe-google-apps-support/oct-portal',
    initialWork: 24,
    data: [
      {
        date: 'Jan 18 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 0,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-18..2018-01-18&type=Issues'
      },
      {
        date: 'Jan 19 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 2,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-19..2018-01-19&type=Issues'
      },
      {
        date: 'Jan 20 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 1,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-20..2018-01-20&type=Issues'
      },
      {
        date: 'Jan 21 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 1,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-21..2018-01-21&type=Issues'
      },
      {
        date: 'Jan 22 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 1,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-22..2018-01-22&type=Issues'
      },
      {
        date: 'Jan 23 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 1,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-23..2018-01-23&type=Issues'
      },
      {
        date: 'Jan 24 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 3,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-24..2018-01-24&type=Issues'
      },
      {
        date: 'Jan 25 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 3,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-25..2018-01-25&type=Issues'
      },
      {
        date: 'Jan 26 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 3,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-26..2018-01-26&type=Issues'
      },
      {
        date: 'Jan 27 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 3,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-27..2018-01-27&type=Issues'
      },
      {
        date: 'Jan 28 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 3,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-28..2018-01-28&type=Issues'
      },
      {
        date: 'Jan 29 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 23,
        workRemoved: 0,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-29..2018-01-29&type=Issues'
      },
      {
        date: 'Jan 30 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 1,
        workRemoved: 3,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-30..2018-01-30&type=Issues'
      },
      {
        date: 'Jan 31 2018 12:00:00 GMT-0700 (Mountain Standard Time)',
        workAdded: 0,
        workRemoved: 5,
        url: 'https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+closed%3A2018-01-31..2018-02-31&type=Issues'
      }
    ]
  }
  ]
}

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
   * @returns {Promise} Resolved with an array of ideas.
   */
  static getIdeas () {
    return HTTP.get('')
  }

  /**
   * Returns a Promise that resolves with a list of my initiatives.
   * @returns {Promise} Resolved with an array of my initiatives.
   */
  static getMyInitiatives () {
    return HTTP.get('?view=mine')
  }

  /**
   * Returns a Promise that resolves with an initiative.
   * @param {string} id The id of the initiative.
   * @returns {Promise} Resolved with an initiative.
   */
  static getInitiative (id) {
    return HTTP.get(`/${id}`).then((response) => {
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
    return HTTP.get(`/${slug}?type=slug`).then((response) => {
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
    return HTTP.get(`/${id}/steps`).then((response) => {
      return response.data
    }, (err) => {
      console.error(`Failed at route /${id}/steps.`)
      console.error(err)
      return fakeInitiativeSteps
    })
  }

  /**
   * Creates a new initiative.
   * @param {string} title The title of the initiative.
   * @param {string} description The description of the initiative.
   * @param {string} businessSponsorEmail The email of the business sponsor.
   * @param {boolean} hasBudget Whether there is budget for this or not.
   * @param {Date} expectedTargetDate When should this be delivered?
   */
  static createInitiative (title, description, businessSponsorEmail, hasBudget, expectedTargetDate) {
    return HTTP.post('', {
      title,
      description,
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
    return HTTP.get(`${id}/assignee`)
  }
}

export const IdeasService = x
