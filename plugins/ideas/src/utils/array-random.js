//
// Copyright (c) City of Edmonton. All rights reserved.
// Licensed under GPL. See LICENSE file in the project root for full license information.
// @author j-rewerts
//

/**
 * Returns a random element from an array.
 * @param {Array} array An array.
 * @return {*} Random element from array.
 */
export default function (array) {
  let index = Math.floor(Math.random() * array.length)
  return array[index]
}
