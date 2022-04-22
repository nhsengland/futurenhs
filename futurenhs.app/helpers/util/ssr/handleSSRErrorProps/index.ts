import { GetServerSidePropsResult } from 'next'
import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject'
import { ServiceError } from '@services/index'

declare interface Config {
    props: Record<any, any>
    error: Partial<ServiceError>,
    shouldSurface?: boolean;
}

export const handleSSRErrorProps = ({ 
    props, 
    error,
    shouldSurface = true 
}: Config): any => {

    console.log(error)

    //TODO - send to error logging service

    if(shouldSurface){

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

    }

    return getJsonSafeObject({
        object: {
            props: props,
        },
    })
}
