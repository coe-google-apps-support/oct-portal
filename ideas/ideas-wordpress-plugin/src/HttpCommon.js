import axios from 'axios'
import config from './config'

var x = axios.create({
  baseURL: config.IDEAS_API,
  headers: {
    AuthorizationEncrypted: 'true'
  }
})

x.setUserInfo = function (userInfo) {
  axios.defaults.headers.common['Authorization'] = 'Bearer ' + (userInfo || {}).auth
}

export const HTTP = x
