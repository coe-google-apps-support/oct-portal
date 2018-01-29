function get (url) {
  let pResolve, pReject
  let p = new Promise((resolve, reject) => {
    pResolve = resolve
    pReject = reject
  })
  let oReq = new XMLHttpRequest()
  oReq.addEventListener('load', (evt) => {
    if (evt.currentTarget.status !== 200) {
      pReject(evt.currentTarget.statusText)
    }

    console.log(oReq.getResponseHeader('link'))
    let links = parseHeader(oReq.getResponseHeader('link'))

    if (links['next']) {
      // There are more pages to be pulled
      get(links['next']).then((response) => {
        let mine = JSON.parse(evt.currentTarget.response)
        let theirs = response
        return mine.concat(theirs)
      }).then((response) => {
        pResolve(response)
      })
    } else {
      pResolve(JSON.parse(evt.currentTarget.response))
    }
  })
  oReq.open('GET', url)
  oReq.send()
  return p
}

function parseHeader (header) {
  let obj = {}
  let res1 = header.split(',')
  res1.map((jumbleStr) => {
    let temp = jumbleStr.split('; ')
    // Grabs the word from the phrase rel="next"
    let key = temp[1].match(/rel="(\w+)"/)[1]
    // "<https://api.github.com/repositories/114803173/issues?state=all&page=2>"
    let value = temp[0].match(/<([^>]+)>/)[1]
    obj[key] = value
  })

  return obj
}

let x = class GithubService {
  /**
   * Gets all issues associated with the oct-portal repository.
   * TODO: Add fallback for rate limiting.
   * @return {Promise} Resolved with the list of issues.
   */
  static getAllIssues () {
    console.log('test')
    return get('https://api.github.com/repos/coe-google-apps-support/oct-portal/issues?state=all').then((response) => {
      return response
    })
  }

  /**
   * Gets all issues associated with the oct-portal repository.
   * @param {Date} since The time to get issues since.
   * @return {Promise} Resolved with the list of issues.
   */
  static getAllIssuesSince (since) {
    let formatted = since.toString()
    console.log(formatted)
    return get(`https://api.github.com/repos/coe-google-apps-support/oct-portal/issues?state=all&since=${formatted}`)
  }

  /**
   * Gets all open issues associated with the oct-portal repo.
   * @return {Promise} Resolved with the list of issues.
   */
  static getOpenIssues () {
    return get('https://api.github.com/repos/coe-google-apps-support/oct-portal/issues?state=open')
  }

  /**
   * Gets all closed issue associated with the oct-portal repo.
   * @return {Promise} Resolved with the list of issues.
   */
  static getClosedIssues () {
    return get('https://api.github.com/repos/coe-google-apps-support/oct-portal/issues?state=closed')
  }
}

export const GithubService = x
