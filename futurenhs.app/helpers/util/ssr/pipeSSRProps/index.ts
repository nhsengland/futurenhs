import { GetServerSideProps } from 'next'
import { GetServerSidePropsContext, HofConfig } from '@appTypes/next'

declare interface Config {
    routeId: string
    props: Record<any, any>
    getServerSideProps?: GetServerSideProps
    handlers: Array<(config: HofConfig) => Promise<HofConfig>>
}

export const pipeSSRProps = async (
    context,
    { routeId, props, handlers, getServerSideProps }: Config,
    callBack
) => {
    const output = await handlers.reduce(
        (chain, func) => chain.then(func),
        Promise.resolve({
            routeId,
            props,
            getServerSideProps,
        })
    )

    return callBack(output)
}
