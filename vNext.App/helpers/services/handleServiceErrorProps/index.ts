import { ServiceError } from '@services/index';

declare interface Config {
    props: Record<any, any>;
    error: Error;
}

export const handleServiceErrorProps = ({ props, error }: Record<any, any>) => {
    
    if (error.name === 'ServiceError') {

        props.errors = [{
            [error.data.status]: error.data.statusText
        }];

    }

} 