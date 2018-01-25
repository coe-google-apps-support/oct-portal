/**
 * Formats a date string so it shows the time since the date.
 * @param {string} dateString The date string to format.
 * @return {string} A string showing the time since Date.
 */
export default function formatDateSince (dateString) {
  const emptyDate = new Date('0001-01-01T00:00:00.000Z')
  const now = new Date()
  const d = new Date(dateString)

  if (d.getTime() === emptyDate.getTime()) {
    return ''
  } else if (d > now) {
    console.log('This date is in the future.')
    return ''
  } else {
    var timeDiff = Math.abs(d.getTime() - now.getTime())
    var seconds = timeDiff / 1000
    if (seconds < 60) {
      return '' + Math.floor(seconds) + ' secs ago'
    } else {
      var minutes = seconds / 60
      if (minutes === 1) {
        return '1 min ago'
      } else if (minutes < 60) {
        return '' + Math.floor(minutes) + ' mins ago'
      } else {
        var hours = minutes / 24
        if (hours === 1) {
          return '1 hour ago'
        } else if (hours < 24) {
          return '' + Math.floor(hours) + ' hours ago'
        } else {
          var days = hours / 24
          if (days === 1) {
            return '1 day ago'
          } else if (days < 365) {
            return '' + Math.floor(days) + ' days ago'
          } else {
            var years = days / 365 // close enough
            if (years === 1) {
              return '1 year ago'
            } else {
              return '' + Math.floor(years) + ' years ago'
            }
          }
        }
      }
    }
  }
}
