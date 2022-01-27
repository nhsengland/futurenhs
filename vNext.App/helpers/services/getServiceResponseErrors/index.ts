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

    serviceResponseList?.forEach((serviceResponse) => {

        serviceResponse?.errors?.forEach((error) => {

            const errorCode: any = Object.keys(error)[0];

            matcher(errorCode) && matchingServiceResponses.push(serviceResponse);

        });

    });

    return matchingServiceResponses;

};
