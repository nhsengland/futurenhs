import { setFetchOpts as setFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { services } from '@constants/services';
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch';
import { ServiceError } from '..';
import { FetchResponse } from '@appTypes/fetch';
import { ApiPaginatedResponse, ServiceResponse } from '@appTypes/service';
import { User } from '@appTypes/user';
import { Option } from '@appTypes/option';

declare type Options = ({
    user: User;
    term: string;
});

declare type Dependencies = ({
    setFetchOptions: any;
    fetchJSON: any;
});

export const getSiteUsersByTerm = async ({
    user,
    term
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<Array<Option>>> => {

    const serviceResponse: ServiceResponse<Array<Option>> = {
        data: []
    };

    const setFetchOptions = dependencies?.setFetchOptions ?? setFetchOptionsHelper;
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

    const id: string = user.id;

    // const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/admin/users?term=${term}`;
    // const apiResponse: FetchResponse = await fetchJSON(apiUrl, setFetchOptions({ method: requestMethods.GET }), defaultTimeOutMillis);
    // const apiData: ApiPaginatedResponse<any> = apiResponse.json;
    // const apiMeta: any = apiResponse.meta;

    // const { ok, status, statusText } = apiMeta;

    // if(!ok){

    //     throw new ServiceError('An unexpected error occurred when attempting to get the site users', {
    //         serviceId: services.GET_SITE_USERS_BY_TERM,
    //         status: status,
    //         statusText: statusText,
    //         body: apiData
    //     });

    // }

    // apiData.data?.forEach((datum) => {

    //     serviceResponse.data.push({
    //         value: datum.id ?? '',
    //         label: datum.name ?? '',
    //     });

    // });

    // Temporary for testing until api is ready
    const suggestions: Array<Option> = [
        {
            value: "4277949c-9113-4a73-8d8d-a948d6199347",
            label: "Agustin Spridgeon"
        },
        {
            value: "9f8d3f87-c253-4fb4-909f-e2a240b815e8",
            label: "Francesca Blaske"
        },
        {
            value: "683350f7-e1b8-4f63-9f05-054661b4bc72",
            label: "Neala Camili"
        },
        {
            value: "aa120a38-e87c-45d5-9196-3e8102f2cf43",
            label: "Mylo Caroli"
        },
        {
            value: "4dc7afe1-0b0e-4c40-b698-acd3f546979c",
            label: "Robin Heugle"
        },
        {
            value: "44cb3905-5268-4c6e-b82f-bd6fe1991cbf",
            label: "Morie Ambrogelli"
        },
        {
            value: "e8b4b10b-d804-47b8-96a4-06cb716cb1d5",
            label: "Elvin Merrgen"
        },
        {
            value: "8d399cda-f8ce-433f-9131-d442d9d73394",
            label: "Susanetta Durie"
        },
        {
            value: "c8d8de1a-b6d4-4ea8-94ad-10549bae5758",
            label: "Lorrie Kanzler"
        },
        {
            value: "c62521dc-a399-4296-8afc-2f512361f449",
            label: "Lynna Stein"
        },
        {
            value: "066d7700-bbd6-4db4-a8b5-4f3f4e04aa0a",
            label: "Nataniel Happer"
        },
        {
            value: "8db49190-9c99-471b-a956-f3a12eb6815f",
            label: "Olag Whittingham"
        },
        {
            value: "e64b0181-a337-4864-89df-7571569f4a46",
            label: "Kele Robertz"
        },
        {
            value: "1be01515-2c18-4f98-a058-5f0560725928",
            label: "Loralyn Lintot"
        },
        {
            value: "91c209d1-f01c-4ce6-8d75-b86352406712",
            label: "Lynelle Wartonby"
        },
        {
            value: "a7ebc6e2-4138-4585-93ab-ec58ee20da18",
            label: "Garald Theakston"
        },
        {
            value: "9fa6a42b-9f58-46df-a50b-e0c3d15c7dce",
            label: "Octavia Gyurkovics"
        },
        {
            value: "5a33d66b-f293-48ac-bcbb-579a82c47a45",
            label: "Heindrick Dugan"
        },
        {
            value: "3e202368-1dc2-4d61-ac70-d1d52d765e2f",
            label: "Devland Bubbings"
        },
        {
            value: "64352f83-88b8-479f-a21b-5eab30d22efe",
            label: "Nap Caverhill"
        },
        {
            value: "d4fcdd78-5945-4641-b1d4-2103554ec389",
            label: "Erny MacKaig"
        },
        {
            value: "29b37aa8-242e-4ec4-a1fe-ad6a6e37e297",
            label: "Robbin Apfel"
        },
        {
            value: "9c754d25-81ad-475c-9863-6dc297a22c0d",
            label: "Charo Sieb"
        },
        {
            value: "4d7c46bb-265e-46c9-98eb-fa72d470d656",
            label: "Duffie Meegan"
        },
        {
            value: "895efbc2-d9a6-4256-976d-3e0e83a323a3",
            label: "Rozanna Briand"
        }
    ];

    const results: Array<Option> = suggestions.filter((suggestion) => {

        return suggestion.label.toLowerCase().indexOf(term?.toLowerCase()) > -1;

    });

    serviceResponse.data = results;

    return serviceResponse;

}