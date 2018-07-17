import axios from 'axios'

var x = axios.create({
  baseURL: process.env.ISSUES_API,
  headers: {
    AuthorizationEncrypted: 'true'
  }
})

x.setUserInfo = function (userInfo) {
  axios.defaults.headers.common['Authorization'] = 'Bearer ' + (userInfo || {}).auth
}

x.setIssuesApi = function (api) {
  if (api) {
    x.baseURL = api
  } else {
    axios.defaults.baseURL = process.env.ISSUES_API
  }
}

export const HTTP = x
