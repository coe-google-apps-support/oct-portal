const fakeIssues = {
  data: [{
    'id': 0,
    'title': 'Ticket 1 laptop oh no loptop noooo',
    'description': 'My laptop broke! Nothing I do works, everything is broken.. help please please. I\'ve got work to do and I cannot do anything until this laptop is fixed oh dear...',
    'assigneeEmail': 'harry.potter@edmonton.ca',
    'requestorName': 'Phil',
    'date': 'May 13 2018 10:03:03 GMT-0700 (Mountain Standard Time)',
    'remedyStatus': 'Submitted'
  },
  {
    'id': 1,
    'title': 'Ticket 2',
    'description': 'I didn\'t get paid!',
    'assigneeEmail': 'severus.snape@edmonton.ca',
    'requestorName': 'Lauren',
    'date': 'June 23 2018 10:03:03 GMT-0700 (Mountain Standard Time)',
    'remedyStatus': 'In Review'
  },
  {
    'id': 2,
    'title': 'Ticket 3',
    'description': 'New monitor for computer.',
    'assigneeEmail': 'ron.weasley@edmonton.ca',
    'requestorName': 'Dora',
    'date': 'March 15 2018 10:03:03 GMT-0700 (Mountain Standard Time)',
    'remedyStatus': 'Completed'
  },
  {
    'id': 3,
    'title': 'Ticket 4',
    'description': 'Weird smell in building.',
    'assigneeEmail': 'albus.dumbledore@edmonton.ca',
    'requestorName': 'Josh',
    'date': 'July 3 2018 10:03:03 GMT-0700 (Mountain Standard Time)',
    'remedyStatus': 'In Review'
  },
  {
    'id': 4,
    'title': 'Ticket 3',
    'description': 'New monitor for computer.',
    'assigneeEmail': 'ron.weasley@edmonton.ca',
    'requestorName': 'Dora',
    'date': 'March 15 2018 10:03:03 GMT-0700 (Mountain Standard Time)',
    'remedyStatus': 'Completed'
  },
  {
    'id': 5,
    'title': 'Ticket 3',
    'description': 'New monitor for computer.',
    'assigneeEmail': 'ron.weasley@edmonton.ca',
    'requestorName': 'Dora',
    'date': 'March 15 2018 10:03:03 GMT-0700 (Mountain Standard Time)',
    'remedyStatus': 'Completed'
  },
  {
    'id':6,
    'title':'Wireless Mobile Device Performance Issue (Smartphone, Cell Phone, Cellular Modem, Aircard, SIM Card)',
    'description':'Remedy UAT Testing - Midteir Upgrade\n\nOnsite Contact Name:\nOnsite Contact Number:\n\nAlternate Contact Name:\nAlternate Contact Number:\n\nService Address (Building, Floor, Address):\n\nVendor Account Number:\n\nCost Center:\nCost Center Owner:\n\nPlease provide which device type, make and model you are having issues with: (Aircard Non-Integrated, Cellular Modem, Cellular Phone, Smartphone, SIM Card)\n\n\n\nEnd User of Device:\nEnd User Contact Number:\n\nPhone number(s) having the issue:\n\nWorkstation serial number(s) (Aircrds Only):\n\nNumber of users affected:\n\nDate issue started to occur:\n\nIs this a re-occurring issue? (Yes\\No)  If yes, please provide details of past occurrences.\n\n\n\nIf this is for a repair, please indicate preferred dealer (Wireless City, Clearwest Solutions,or Alberta Mobility)\n\n\n\n\nPlease provide detailed information regarding the issue that is occurring. (Broken Screen, Not Receiving Calls, Battery Issue, Cannot access voicemail).',
    'createdDate':'2018-07-12T20:07:25',
    'assigneeEmail':'Linda.Hart@edmonton.ca',
    'assigneeGroup':'Telecom',
    'requestorName':'Linda Hart',
    'remedyStatus':'Pending',
    'referenceId':'INC000000192853'
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
    myIssues.data = [fakeIssues.data[0], fakeIssues.data[1], fakeIssues.data[2], fakeIssues.data[3], fakeIssues.data[4], fakeIssues.data[5]]

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
