import { approve, deny } from '.'

const isEmail = (value) => {
    const name = 'isEmail'
    const denyMessage = ''
    const isEmailRegExp = new RegExp(
        /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
    )
    return isEmailRegExp.test(value) ? approve(name) : deny(name, denyMessage)
}

export default isEmail
