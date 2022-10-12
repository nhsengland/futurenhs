const gateway = require('./gateway')
const api = ({ app }) => {
    gateway({ app })
}

module.exports = api
