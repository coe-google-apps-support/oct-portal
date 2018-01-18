const handlebars = require('handlebars');

module.exports = function (callback, templateText, ideaDataSerialized) {
    var template = handlebars.compile(templateText);

    var renderTemplate = function (ideaData) {
        return template(ideaData);
    }

    callback(null, renderTemplate(JSON.parse(ideaDataSerialized)));
}