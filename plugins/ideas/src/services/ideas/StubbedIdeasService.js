import getRandom from '@/utils/array-random'

const fakeIdeas = {
  data: [{
    id: 1,
    url: 'initiatives/idea-1',
    title: 'Idea #1',
    description: 'Easily one of the greatest ideas ever made, this idea does things for people that people really really need. Game changing. Mind blowing. World altering. Welcome to Idea #1.',
    createdDate: '2018-02-28 17:42:43.210965',
    stakeholders: [{
      'userName': 'gregory.onuczko@edmonton.ca'
    }],
    businessCaseURL: 'https://www.google.ca/'
  },
  {
    id: 2,
    url: 'initiatives/idea-2',
    title: 'Idea #2',
    description: 'Easily one of the lamest ideas ever made, this idea does things for people that people really really do not need. Mediocre. Milquetoast. Moronic. Welcome to Idea #2.',
    createdDate: 'Dec 28 2017 10:40:16 GMT-0700 (Mountain Standard Time)',
    stakeholders: [{
      'userName': 'jared.rewerts@edmonton.ca'
    }]
  },
  {
    id: 3,
    url: 'initiatives/idea-3',
    title: 'Idea #3',
    description: 'Another idea! They\'re everywhere!',
    createdDate: 'Dec 30 2017 10:40:16 GMT-0700 (Mountain Standard Time)',
    stakeholders: [{
      'userName': 'daniel.chenier@edmonton.ca'
    }],
    businessCaseURL: 'https://www.google.ca/'
  },
  {
    id: 4,
    url: 'initiatives/idea-4',
    title: 'Idea #4',
    description: 'And another idea! They\'re everywhere!',
    createdDate: 'Dec 31 2017 10:40:16 GMT-0700 (Mountain Standard Time)',
    stakeholders: [{
      'userName': 'daniel.chenier@edmonton.ca'
    }]
  },
  {
    id: 5,
    url: 'initiatives/i-want-to-create-an-ai-version',
    title: 'I want to create an AI version of both Dan and Jared...',
    description: `I believe we can create single Android and implant both Dan and Jared's brains into this singularity. This new life form will then be used to develop a new Blockchain methodology . I want to own 50% of the initial ICO Token's prior to launch. I want this new AI being to make me very very rich. I will then rule the world. This new fully automatism being will hack into every government military mainframe and manipulate all launch code systems and use them against the oppressive regimes in order to gain global power over all humans and make me the supreme leader! I do see this being funded by the City.`,
    createdDate: 'Jan 16 2018 10:40:16 GMT-0700 (Mountain Standard Time)',
    stakeholders: [{
      'userName': 'stephen.mundy@edmonton.ca'
    }],
    businessCaseURL: 'https://www.google.ca/'
  },
  {
    id: 6,
    url: 'initiatives/idea-22',
    title: 'Idea #2',
    description: 'Easily one of the lamest ideas ever made, this idea does things for people that people really really do not need. Mediocre. Milquetoast. Moronic. Welcome to Idea #2.',
    createdDate: 'Dec 28 2017 10:40:16 GMT-0700 (Mountain Standard Time)',
    stakeholders: [{
      'userName': 'jared.rewerts@edmonton.ca'
    }]
  },
  {
    id: 7,
    url: 'initiatives/idea-32',
    title: 'Idea #3',
    description: 'Another idea! They\'re everywhere!',
    createdDate: 'Dec 30 2017 10:40:16 GMT-0700 (Mountain Standard Time)',
    stakeholders: [{
      'userName': 'daniel.chenier@edmonton.ca'
    }]
  },
  {
    id: 8,
    url: 'initiatives/idea-42',
    title: 'Idea #4',
    description: 'And another idea! They\'re everywhere!',
    createdDate: 'Dec 31 2017 10:40:16 GMT-0700 (Mountain Standard Time)',
    stakeholders: [{
      'userName': 'daniel.chenier@edmonton.ca'
    }]
  }
  ]
}

const steps1 = {
  data: [{
    'title': 'Submitted',
    'description': 'Proin auctor pretium sem, ut malesuada dolor porttitor vel. Donec at auctor libero. Nunc nec massa condimentum, mattis diam vel, rutrum ante. Nam sed semper metus.',
    'startDate': 'Jan 28 2018 10:03:03 GMT-0700 (Mountain Standard Time)',
    'completionDate': 'Jan 29 2018 12:23:47 GMT-0700 (Mountain Standard Time)'
  },
  {
    'title': 'In Review',
    'description': 'Etiam id vehicula metus. Fusce tristique vestibulum nulla, vitae vestibulum mi scelerisque ut. Sed consequat elit in lacus tristique, id mattis elit eleifend. Vestibulum nec augue maximus, feugiat justo sollicitudin, interdum metus.',
    'startDate': 'Jan 29 2018 12:23:47 GMT-0700 (Mountain Standard Time)',
    'completionDate': null
  },
  {
    'title': 'In Collaboration',
    'description': 'Maecenas non enim a eros imperdiet scelerisque et a urna. Nunc at tincidunt massa, sit amet faucibus neque.',
    'startDate': null,
    'completionDate': null
  },
  {
    'title': 'In Delivery',
    'description': 'Pellentesque ut neque tempus, placerat purus volutpat, scelerisque velit. Vivamus porta urna vel ligula lobortis, id porttitor quam maximus.',
    'startDate': null,
    'completionDate': null
  }
  ]
}

const steps2 = {
  data: [{
    'title': 'Submitted',
    'description': 'Proin auctor pretium sem, ut malesuada dolor porttitor vel. Donec at auctor libero. Nunc nec massa condimentum, mattis diam vel, rutrum ante. Nam sed semper metus.',
    'startDate': 'Feb 13 2018 10:03:03 GMT-0700 (Mountain Standard Time)',
    'completionDate': 'Feb 14 2018 12:23:47 GMT-0700 (Mountain Standard Time)'
  },
  {
    'title': 'In Review',
    'description': 'Etiam id vehicula metus. Fusce tristique vestibulum nulla, vitae vestibulum mi scelerisque ut. Sed consequat elit in lacus tristique, id mattis elit eleifend. Vestibulum nec augue maximus, feugiat justo sollicitudin, interdum metus.',
    'startDate': 'Feb 14 2018 12:23:47 GMT-0700 (Mountain Standard Time)',
    'completionDate': 'Feb 17 2018 12:23:47 GMT-0700 (Mountain Standard Time)'
  },
  {
    'title': 'In Collaboration',
    'description': 'Maecenas non enim a eros imperdiet scelerisque et a urna. Nunc at tincidunt massa, sit amet faucibus neque.',
    'startDate': 'Feb 17 2018 12:23:47 GMT-0700 (Mountain Standard Time)',
    'completionDate': null
  },
  {
    'title': 'In Delivery',
    'description': 'Pellentesque ut neque tempus, placerat purus volutpat, scelerisque velit. Vivamus porta urna vel ligula lobortis, id porttitor quam maximus.',
    'startDate': null,
    'completionDate': null
  }
  ]
}

const fakeAssignee = {
  data: {
    'name': 'Super BA',
    'email': 'super.ba@edmonton.ca',
    'phoneNumber': '555-555-5555',
    'avatarURL': 'https://i.imgur.com/FD51R30.png'
  }
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
let x = class StubbedIdeasService {
  /**
   * Returns a Promise that resolves with a list of ideas.
   * @returns {Promise} Resolved with an array of ideas.
   */
  static getIdeas () {
    return new Promise((resolve, reject) => {
      setTimeout(() => {
        resolve(fakeIdeas)
      }, QUERY_TIMEOUT)
    })
  }

  /**
   * Returns a Promise that resolves with an initiative.
   * @param {string} id The id of the initiative.
   * @returns {Promise} Resolved with an initiative.
   */
  static getInitiative (id) {
    id = id.toString()
    const initiative = fakeIdeas.data.find((el) => {
      return id === el.id.toString()
    })

    return new Promise((resolve, reject) => {
      setTimeout(() => {
        if (initiative) {
          resolve(initiative)
        } else {
          reject(new Error(`No initiative with id ${id}.`))
        }
      }, QUERY_TIMEOUT)
    })
  }

  /**
   * Returns a Promise that resolves with an initiative.
   * @param {string} slug The slug of the initiative.
   * @returns {Promise} Resolved with an initiative.
   */
  static getInitiativeBySlug (slug) {
    return new Promise((resolve, reject) => {
      setTimeout(() => {
        resolve(getRandom(fakeIdeas.data))
      }, QUERY_TIMEOUT)
    })
  }

  /**
   * Returns a Promise that resolves with an initiatives steps.
   * @param {string} id The id of the initiative.
   * @returns {Promise} Resolved with an initiatives steps.
   */
  static getInitiativeSteps (id) {
    let steps = [
      steps1,
      steps2
    ]

    return new Promise((resolve, reject) => {
      setTimeout(() => {
        resolve(getRandom(steps))
      }, QUERY_TIMEOUT)
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
    return new Promise((resolve, reject) => {
      setTimeout(() => {
        resolve({
          data: {
            url: 'https://google.com'
          }
        })
      }, QUERY_TIMEOUT)
    })
  }

  /**
   * Gets the assignee for the given initiative.
   * @param {string} id The id of the initiative.
   * @return {Promise} A Promise that resolves with the information of the assignee.
   */
  static getAssignee (id) {
    return new Promise((resolve, reject) => {
      setTimeout(() => {
        resolve(fakeAssignee)
      }, QUERY_TIMEOUT)
    })
  }

  /**
   * Causes an initiative to be updated.
   * @param {Object} initiative The initiative to update.
   * @return {Promise} A promise resolved with the updated initiative.
   */
  static updateInitiative (initiative) {
    return new Promise((resolve, reject) => {
      setTimeout(() => {
        resolve(initiative)
      }, QUERY_TIMEOUT)
    })
  }
}

export const StubbedIdeasService = x
