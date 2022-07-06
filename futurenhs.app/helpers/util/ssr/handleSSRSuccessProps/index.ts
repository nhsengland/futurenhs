import { GetServerSidePropsContext } from '@appTypes/next'
import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject'
import { selectCsrfToken } from '@selectors/context'

declare interface Config {
    props: Record<any, any>
    context: GetServerSidePropsContext
}

export const handleSSRSuccessProps = ({ props, context }: Config): any => {
    const csrfToken: string = selectCsrfToken(context)
    props.csrfToken = csrfToken

    return getJsonSafeObject({
        object: {
            props: props,
        },
    })
}
