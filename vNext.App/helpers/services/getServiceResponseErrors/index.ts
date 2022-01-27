import { ServiceResponse } from '@appTypes/service';

declare interface Config {
    serviceResponseList: Array<ServiceResponse<any>>;
    matcher: (statusCode: number) => Boolean;
}

export const getServiceResponseErrors = ({
    serviceResponseList = [],
    matcher
}: Config): Array<ServiceResponse<any>> => {

    let matchingServiceResponses: Array<ServiceResponse<any>> = [];

    serviceResponseList.forEach((serviceResponse) => {

        if(serviceResponse?.errors){

            Object.keys(serviceResponse.errors).forEach((key: string) => {

                matcher(Number(key)) && matchingServiceResponses.push(serviceResponse);

            });

        }

    });

    return matchingServiceResponses;

};
