import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject';

declare interface Config {
    props: Record<any, any>;
}

export const handleSSRSuccessProps = ({ props }: Config): any => {

    return getJsonSafeObject({
        object: {
            props: props
        }
    });

} 