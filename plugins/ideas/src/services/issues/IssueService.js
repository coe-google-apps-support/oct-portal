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
let x = class IssueService {
  static getIssues (page, pageSize, contains) {
    return HTTP.get('/plugins/issues/api', {
      params: {
        page,
        pageSize,
        contains
      }
    })
  }

  static getMyIssues (page, pageSize, contains) {
    return HTTP.get('/plugins/issues/api', {
      params: {
        view: 'Mine',
        page,
        pageSize,
        contains
      }
    })
  }

  static getIssueByID (id) {
    return HTTP.get(`/plugins/issues/api/${id}`).then((response) => {
      return response.data
    }, (err) => {
      console.error(`Failed at route /${id}`)
      console.error(err)
    })
  }

  static createIssue (title, description) {
    return HTTP.post('/plugins/issues/api', {
      title,
      description
    })
  }
}

export const IssueService = x
