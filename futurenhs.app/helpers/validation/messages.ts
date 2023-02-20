import { VRules } from '.'

type VStrings = {
    [key in VRules]: string
}
const messages: VStrings = {
    isEmail: 'A valid email address is required',
    isRequired: 'This field is required',
}

export default messages
