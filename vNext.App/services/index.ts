export * as services from './';

declare interface ServiceErrorData {
    status: number;
    statusText: string;
    body?: any;
}

export class ServiceError extends Error {

    data: ServiceErrorData = {
        status: null,
        statusText: null,
        body: null
    };

    constructor(message: string, data: ServiceErrorData) {

        super(message);
        this.name = 'ServiceError';
        this.data = data;

    }

};
