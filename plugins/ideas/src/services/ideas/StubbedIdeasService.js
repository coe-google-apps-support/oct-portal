const fakeIdeas = {
  data: [{
    id: 1,
    url: 'ViewIdeas/1',
    title: 'Idea #1',
    description: 'Easily one of the greatest ideas ever made, this idea does things for people that people really really need. Game changing. Mind blowing. World altering. Welcome to Idea #1.',
    createdDate: 'Jan 14 2018 10:40:16 GMT-0700 (Mountain Standard Time)',
    stakeholders: [{
      'userName': 'gregory.onuczko@edmonton.ca'
    }]
  },
  {
    id: 2,
    url: 'ViewIdeas/2',
    title: 'Idea #2',
    description: 'Easily one of the lamest ideas ever made, this idea does things for people that people really really do not need. Mediocre. Milquetoast. Moronic. Welcome to Idea #2.',
    createdDate: 'Dec 28 2017 10:40:16 GMT-0700 (Mountain Standard Time)',
    stakeholders: [{
      'userName': 'jared.rewerts@edmonton.ca'
    }]
  },
  {
    id: 3,
    url: 'ViewIdeas/3',
    title: 'Idea #3',
    description: 'Another idea! They\'re everywhere!',
    createdDate: 'Dec 30 2017 10:40:16 GMT-0700 (Mountain Standard Time)',
    stakeholders: [{
      'userName': 'daniel.chenier@edmonton.ca'
    }]
  },
  {
    id: 4,
    url: 'ViewIdeas/4',
    title: 'Idea #4',
    description: 'And another idea! They\'re everywhere!',
    createdDate: 'Dec 31 2017 10:40:16 GMT-0700 (Mountain Standard Time)',
    stakeholders: [{
      'userName': 'daniel.chenier@edmonton.ca'
    }]
  },
  {
    id: 5,
    url: 'ViewIdeas/5',
    title: 'I want to create an AI version of both Dan and Jared...',
    description: `I believe we can create single Android and implant both Dan and Jared's brains into this singularity. This new life form will then be used to develop a new Blockchain methodology . I want to own 50% of the initial ICO Token's prior to launch. I want this new AI being to make me very very rich. I will then rule the world. This new fully automatism being will hack into every government military mainframe and manipulate all launch code systems and use them against the oppressive regimes in order to gain global power over all humans and make me the supreme leader! I do see this being funded by the City.`,
    createdDate: 'Jan 16 2018 10:40:16 GMT-0700 (Mountain Standard Time)',
    stakeholders: [{
      'userName': 'stephen.mundy@edmonton.ca'
    }]
  }
  ]
}

const fakeInitiativeSteps = {
  data: [{
    step: 1,
    name: 'Submit',
    status: 'done',
    type: 'text',
    data: 'Congrats! You submitted this on January 14th, 2018.'
  },
  {
    step: 2,
    name: 'Review',
    status: 'done',
    type: 'chat',
    data: [{
      user: 'super.ba@edmonton.ca',
      created: 'Jan 14 2018 10:55:32 GMT-0700 (Mountain Standard Time)',
      data: 'Wow. What a great idea. Just consulting with my BA side-kick, bat-ba.'
    },
    {
      user: 'bat.ba@edmonton.ca',
      created: 'Jan 14 2018 11:07:00 GMT-0700 (Mountain Standard Time)',
      data: 'Agreed. Game changer.'
    },
    {
      user: 'super.ba@edmonton.ca',
      created: 'Jan 14 2018 11:11:00 GMT-0700 (Mountain Standard Time)',
      data: 'All systems go!!'
    }
    ]
  },
  {
    step: 3,
    name: 'Collaborate',
    status: 'done',
    type: 'chat',
    data: [{
      user: 'a.random@edmonton.ca',
      created: 'Jan 16 2018 7:03:32 GMT-0700 (Mountain Standard Time)',
      data: 'how do I get in on this? seems like this could have some potential'
    },
    {
      user: 'super.ba@edmonton.ca',
      created: 'Jan 16 2018 8:07:00 GMT-0700 (Mountain Standard Time)',
      data: 'If you\'d like, we could add you to the list of stakeholders?'
    },
    {
      user: 'a.random@edmonton.ca',
      created: 'Jan 17 2018 11:11:00 GMT-0700 (Mountain Standard Time)',
      data: 'ya please do that'
    },
    {
      user: 'gregory.onuczko@edmonton.ca',
      created: 'Jan 17 2018 11:19:00 GMT-0700 (Mountain Standard Time)',
      data: 'Glad to have you onboard, A Random!!'
    },
    {
      user: 'super.ba@edmonton.ca',
      created: 'Jan 18 2018 8:07:00 GMT-0700 (Mountain Standard Time)',
      data: 'Transitioning to our technical experts for solutioning.'
    }
    ]
  },
  {
    step: 4,
    name: 'Deliver',
    status: 'ongoing',
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
   * Returns a Promise that resolves with an initiatives steps.
   * @param {string} id The id of the initiative.
   * @returns {Promise} Resolved with an initiatives steps.
   */
  static getInitiativeSteps (id) {
    return new Promise((resolve, reject) => {
      setTimeout(() => {
        resolve(fakeInitiativeSteps)
      }, QUERY_TIMEOUT)
    })
  }
}

export const StubbedIdeasService = x
