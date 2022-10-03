import { selectCsrfToken } from '@helpers/selectors/context'
import { Hof } from '@appTypes/hof'

export const withTokens: Hof = async (context) => {
    const csrfToken: string = selectCsrfToken(context)

    try {
        context.page.props.csrfToken = csrfToken
    } catch (error) {
        console.log(error)
    }
}
