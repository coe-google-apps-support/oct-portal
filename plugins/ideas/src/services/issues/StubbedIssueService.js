const fakeIssues = {
  data: [{
    'id': 0,
    'title': 'Ticket 1',
    'description': 'My laptop broke! Nothing I do works, everything is broken.. help please please. I\'ve got work to do and I cannot do anything until this laptop is fixed oh dear...',
    'date': 'May 13 2018 10:03:03 GMT-0700 (Mountain Standard Time)',
    'status': 'Submitted'
  },
  {
    'id': 1,
    'title': 'Ticket 2',
    'description': 'I didn\'t get paid!',
    'date': 'June 23 2018 10:03:03 GMT-0700 (Mountain Standard Time)',
    'status': 'In Review'
  },
  {
    'id': 2,
    'title': 'Ticket 3',
    'description': 'New monitor for computer.',
    'date': 'March 15 2018 10:03:03 GMT-0700 (Mountain Standard Time)',
    'status': 'Completed'
  },
  {
    'id': 3,
    'title': 'Ticket 4',
    'description': 'Weird smell in building.',
    'date': 'July 3 2018 10:03:03 GMT-0700 (Mountain Standard Time)',
    'status': 'In Review'
  }
  ]
}

const QUERY_TIMEOUT = 1000

/**
 * This acts as a stubbed out class for the ideas service.
 * Idea format:
 * idea.id
 * idea.url
 * idea.title
 * idea.description
 * idea.createdDate
 * idea.stakeholders
 * idea.stakeholders[0].userName
 */
let x = class StubbedIssueService {
  /**
   * Returns a Promise that resolves with a list of ideas.
   * @param {Number} page The 1-indexed page number.
   * @param {Number} pageSize The number of results to return.
   * @param {String} contains A search string to apply.
   * @returns {Promise} Resolved with an array of ideas.
   */
  static getIssues (page, pageSize, contains) {
    let issues = { data: [] }

    // TODO Use array.splice to make this cool.
    for (let i = (page - 1) * pageSize; i < page * pageSize && i < fakeIssues.data.length; i++) {
      if (contains && (fakeIssues.data[i].title + fakeIssues.data[i].description).indexOf(contains) !== -1) {
        issues.data.push(fakeIssues.data[i])
      } else if (!contains) {
        issues.data.push(fakeIssues.data[i])
      }
    }

    return new Promise((resolve, reject) => {
      setTimeout(() => {
        resolve(issues)
      }, QUERY_TIMEOUT)
    })
  }

  /**
   * Returns a Promise that resolves with a list of ideas.
   * @param {Number} page The 1-indexed page number.
   * @param {Number} pageSize The number of results to return.
   * @param {String} contains A search string to apply.
   * @returns {Promise} Resolved with an array of ideas.
   */
  static getMyIssues (page, pageSize, contains) {
    let myIssues = { data: null }
    myIssues.data = [fakeIssues.data[0], fakeIssues.data[1], fakeIssues.data[2], fakeIssues.data[3]]

    return new Promise((resolve, reject) => {
      setTimeout(() => {
        resolve(myIssues)
      }, QUERY_TIMEOUT)
    })
  }

  /**
   * Returns a Promise that resolves with an issue.
   * @param {string} id The id of the issue.
   * @returns {Promise} Resolved with an issue.
   */
  static getIssue (id) {
    id = id.toString()
    const issue = fakeIssues.data.find((el) => {
      return id === el.id.toString()
    })

    return new Promise((resolve, reject) => {
      setTimeout(() => {
        if (issue) {
          resolve(issue)
        } else {
          reject(new Error(`No issue with id ${id}.`))
        }
      }, QUERY_TIMEOUT)
    })
  }
}

export const StubbedIssueService = x
