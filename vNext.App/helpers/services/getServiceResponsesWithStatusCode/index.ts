import { ServiceResponse } from '@appTypes/service';

declare interface Config {
    serviceResponseList: Array<ServiceResponse<any>>;
    statusCode: number;
}

export const getServiceResponsesWithStatusCode = ({
    serviceResponseList,
    statusCode
}: Config): Array<ServiceResponse<any>> => {

    let matchingServiceResponses: Array<ServiceResponse<any>> = [];

    serviceResponseList.forEach((serviceResponse) => {

        if(serviceResponse?.errors?.hasOwnProperty(statusCode)){

            matchingServiceResponses.push(serviceResponse);

        }

    });

    return matchingServiceResponses;

};
