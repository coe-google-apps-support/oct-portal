import dateFormat from 'dateformat'

class ResponseTransform {
  constructor (responses) {
    this.responses = responses
  }

  transformToBurndown (since) {
    let byDate = {}

    for (let i = 0; i < this.responses.length; i++) {
      let issue = this.responses[i]
      let openedDate = new Date(issue.created_at)
      this.addOne(byDate, 'workAdded', openedDate)

      if (issue.closed_at) {
        let closedDate = new Date(issue.closed_at)
        this.addOne(byDate, 'workRemoved', closedDate)
      }
    }

    let data = []
    for (let d = new Date(since); d <= new Date(); d.setDate(d.getDate() + 1)) {
      let key = `${d.getFullYear()}-${d.getMonth() + 1}-${d.getDate()}`
      let obj = {
        workAdded: 0,
        workRemoved: 0
      }
      obj.date = dateFormat(d, 'yyyy-mm-dd')

      if (!byDate[key]) {
        obj.workAdded = 0
        obj.workRemoved = 0
      } else {
        if (byDate[key].workAdded) {
          obj.workAdded = byDate[key].workAdded
        }
        if (byDate[key].workRemoved) {
          obj.workRemoved = byDate[key].workRemoved
        }
      }

      let tomorrow = new Date(d)
      tomorrow.setDate(d.getDate() + 1)
      obj.url = `https://github.com/search?utf8=%E2%9C%93&q=repo%3Acoe-google-apps-support%2Foct-portal+updated%3A${dateFormat(d, 'yyyy-mm-dd')}..${dateFormat(d, 'yyyy-mm-dd')}&type=Issues`
      data.push(obj)
    }

    let startingIssues = 0
    let sinceDate = new Date(since)
    for (let key in byDate) {
      let date = new Date(key)
      if (date < sinceDate) {
        if (byDate[key].workAdded) {
          startingIssues += byDate[key].workAdded
        }
        if (byDate[key].workRemoved) {
          startingIssues -= byDate[key].workRemoved
        }
      }
    }

    return {
      data: data,
      initialWork: startingIssues
    }
  }

  addOne (result, type, date) {
    let key = `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()}`

    if (!result[key]) {
      console.log('creating new object')
      result[key] = {}
    }
    if (!result[key][type]) {
      console.log('creating new ' + type)
      result[key][type] = 0
    }

    result[key][type] = result[key][type] + 1
  }
}

export default ResponseTransform
