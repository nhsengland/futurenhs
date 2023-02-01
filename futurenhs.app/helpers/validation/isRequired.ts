import { approve, deny } from '.'

const isRequired = (value) => {
    const name = 'isRequired'
    const denyMessage = ''
    return !!value ? approve(name) : deny(name, denyMessage)
}

export default isRequired
