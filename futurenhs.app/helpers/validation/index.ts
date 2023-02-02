import { isEmail, isRequired } from './rules'

export interface VMessage {
    error: boolean
    name: string
    message: string | null
}

export enum VRules {
    EMAIL = 'isEmail',
    REQUIRED = 'isRequired',
}

const ruleSet = {
    isEmail,
    isRequired,
}

export const validate = (value: string, rules: Array<VRules>) => {
    console.log(rules)
    const ret = rules.map((rule) => {
        const testRule = ruleSet[rule]
        return testRule(value)
    })
    console.log(ret)
    return ret
}

export const approve = (rule: VRules): VMessage => ({
    error: false,
    name: rule,
    message: null,
})
export const deny = (rule: VRules, message: string): VMessage => ({
    error: true,
    name: rule,
    message,
})
