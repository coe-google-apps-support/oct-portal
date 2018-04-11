/* global gapi */
/* global google */

export default function (baseOpts) {
  baseOpts = baseOpts || {}
  gapi.load('picker')
  gapi.load('auth')

  baseOpts.scope = baseOpts.scope || 'https://www.googleapis.com/auth/drive'
  baseOpts.origin = baseOpts.origin || (window.location.protocol + '//' + window.location.host)
  baseOpts.locale = baseOpts.locale || 'en'

  return function pick (opts, cb) {
    if (arguments.length === 1) {
      cb = opts
      opts = {}
    }
    console.log('pick')
    opts = opts || {}
    Object.keys(baseOpts).forEach(function (key) {
      if (!opts.hasOwnProperty(key)) {
        opts[key] = baseOpts[key]
      }
    })

    gapi.load('picker')
    gapi.load('auth2', function () {
      gapi.auth2.authorize({
        client_id: opts.clientId,
        scope: opts.scope,
        immediate: false
      }, handleAuth)
    })

    function handleAuth (result) {
      if (!result) return cb(new Error('Google Picker: unable to authorize'))
      if (result.error) cb(result.error)
      setTimeout(openPicker, 1000, result.access_token)
    }

    function openPicker (token) {
      console.log('opening picker')
      var picker = new google.picker.PickerBuilder()
        .addView(google.picker.ViewId.DOCUMENTS)
        .setLocale(opts.locale)
        .setDeveloperKey(opts.apiKey)
        .setOAuthToken(token)
        .setCallback(function (data) {
          if (data.action === google.picker.Action.PICKED) {
            cb(null, data.docs)
          }
        })
        .setOrigin(opts.origin)

      if (opts.features && opts.features.length > 0) {
        opts.features.forEach(function (feature) {
          picker.enableFeature(google.picker.Feature[feature])
        })
      }

      if (opts.views && opts.views.length > 0) {
        opts.views.forEach(function (view) {
          picker.addView(view)
        })
      }

      picker.build().setVisible(true)
    }
  }
}
