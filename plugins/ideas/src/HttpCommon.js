import axios from 'axios'

var x = axios.create({
  headers: {
    AuthorizationEncrypted: 'true'
  }
})

x.setUserInfo = function (userInfo) {
  axios.defaults.headers.common['Authorization'] = 'Bearer ' + (userInfo || {}).auth
}

export const HTTP = x
