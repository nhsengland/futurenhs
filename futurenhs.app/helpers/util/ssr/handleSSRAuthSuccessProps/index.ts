import { GetServerSidePropsContext } from '@appTypes/next'
import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject'

declare interface Config {
    props: Record<any, any>
    context: GetServerSidePropsContext
}

export const handleSSRAuthSuccessProps = ({ props, context }: Config): any => {
    return getJsonSafeObject({
        object: {
            props: props,
        },
    })
}
