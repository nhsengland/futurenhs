import { approve, deny, VRules } from '.'
import messages from './messages'

export const isEmail = (value) => {
    const denyMessage = messages[VRules.EMAIL]
    const isEmailRegExp = new RegExp(
        /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
    )
    return isEmailRegExp.test(value)
        ? approve(VRules.EMAIL)
        : deny(VRules.EMAIL, denyMessage)
}

export const isRequired = (value) => {
    const denyMessage = messages[VRules.REQUIRED]
    return !!value
        ? approve(VRules.REQUIRED)
        : deny(VRules.REQUIRED, denyMessage)
}
