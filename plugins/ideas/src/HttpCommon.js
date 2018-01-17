import axios from 'axios'

var x = axios.create({
  baseURL: process.env.IDEAS_API,
  headers: {
    AuthorizationEncrypted: 'true'
  }
})

x.setUserInfo = function (userInfo) {
  axios.defaults.headers.common['Authorization'] = 'Bearer ' + (userInfo || {}).auth
}

x.setIdeaApi = function (api) {
  if (api) {
    x.baseURL = api
  } else {
    axios.defaults.baseURL = process.env.IDEAS_API
  }
}

export const HTTP = x
