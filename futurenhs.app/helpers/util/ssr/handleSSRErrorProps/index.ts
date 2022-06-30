import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject'
import { ServiceError } from '@services/index'
import { serializeError } from '@helpers/util/errors/serializeError';

declare interface Config {
    props: Record<any, any>
    error: Partial<ServiceError>
    shouldSurface?: boolean
}

export const handleSSRErrorProps = ({
    props,
    error,
    shouldSurface = true,
}: Config): any => {
    console.log(error)

    //TODO - send to error logging service
    const clonedProps: Record<any, any> = getJsonSafeObject({ object: props })

    if (shouldSurface) {
        if (error.name === 'ServiceError') {
            if (error.data?.status === 404) {
                return {
                    notFound: true,
                }
            } else {
                clonedProps.errors = [
                    {
                        [error.data.status]: error.data
                    },
                ]
            }
        } else {
            clonedProps.errors = [
                {
                    error: serializeError(error),
                },
            ]
        }
    }

    return {
        props: clonedProps,
    }
}
