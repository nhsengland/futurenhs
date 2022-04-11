import { GetServerSidePropsResult } from 'next'
import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject'
import { ServiceError } from '@services/index'

declare interface Config {
    props: Record<any, any>
    error: Partial<ServiceError>
}

export const handleSSRErrorProps = ({ props, error }: Config): any => {
    console.log(error)

    if (error.name === 'ServiceError') {
        if (error.data?.status === 404) {
            return {
                notFound: true,
            }
        } else {
            props.errors = [
                {
                    [error.data.status]: error.data.statusText,
                },
            ]
        }
    } else {
        props.errors = [
            {
                error: error.message,
            },
        ]
    }

    return getJsonSafeObject({
        object: {
            props: props,
        },
    })
}
