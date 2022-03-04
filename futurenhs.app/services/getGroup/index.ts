import { setFetchOpts as setFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { ServiceError } from '..';
import { FetchResponse } from '@appTypes/fetch';
import { ApiResponse, ServiceResponse } from '@appTypes/service';
import { Group } from '@appTypes/group';
import { User } from '@appTypes/user';

declare type Options = ({
    user: User;
    groupId: string;
});

declare type Dependencies = ({
    setFetchOptions: any;
    fetchJSON: any;
});

export type GetGroupService = (options: Options, dependencies?: Dependencies) => Promise<ServiceResponse<Group>>;

export const getGroup = async ({
    user,
    groupId
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<Group>> => {

    const setFetchOptions = dependencies?.setFetchOptions ?? setFetchOptionsHelper;
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

    const id: string = user.id;

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/groups/${groupId}`;
    const apiResponse: FetchResponse = await fetchJSON(apiUrl, setFetchOptions({ method: 'GET' }), 30000);
    const apiData: ApiResponse<any> = apiResponse.json;
    const apiMeta: any = apiResponse.meta;

    const { ok, status, statusText } = apiMeta;

    if(!ok){

        throw new ServiceError('Error getting group', {
            status: status,
            statusText: statusText,
            body: apiData
        });

    }

    const data = {
        text: {
            title: apiData.name ?? null,
            metaDescription: 'A Future NHS group',
            mainHeading: apiData.name ?? null,
            strapLine: apiData?.strapLine ?? null
        },
        image: apiData.image ? {
            src: `${apiData.image?.source}`,
            height: apiData?.image?.height ?? null,
            width: apiData?.image?.width ?? null,
            altText: 'TBC'
        } : null,
        themeId: apiData.themeId
    };

    return {
        data: data
    };

}