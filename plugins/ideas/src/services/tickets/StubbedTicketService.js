const fakeTickets = {
  data: [{
    'id': 0,
    'title': 'Ticket 1',
    'description': 'My laptop broke!',
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
let x = class StubbedIdeasService {
  
}