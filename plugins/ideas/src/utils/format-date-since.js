import TimeAgo from 'javascript-time-ago'
import en from 'javascript-time-ago/locale/en'

TimeAgo.locale(en)
const timeAgo = new TimeAgo('en-US')

/**
 * Formats a date string so it shows the time since the date.
 * @param {string} dateString The date string to format.
 * @return {string} A string showing the time since Date.
 */
export default function formatDateSince (dateString) {
  let then = new Date(dateString)

  return timeAgo.format(then)
}
