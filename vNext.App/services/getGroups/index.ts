import { setGetFetchOpts as setGetFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { getEnvVar } from '@helpers/util/env';
import { getApiPaginationQueryParams } from '@helpers/routing/getApiPaginationQueryParams';
import { getClientPaginationFromApi } from '@helpers/routing/getClientPaginationFromApi';
import { FetchResponse } from '@appTypes/fetch';
import { ApiPaginatedResponse, ServicePaginatedResponse } from '@appTypes/service';
import { Pagination } from '@appTypes/pagination';
import { Group } from '@appTypes/group';
import { User } from '@appTypes/user';

declare type Options = ({
    user: User;
    isMember?: boolean;
    pagination?: Pagination;
});

declare type Dependencies = ({
    setGetFetchOptions: any;
    fetchJSON: any;
});

export const getGroups = async ({
    user,
    isMember,
    pagination
}: Options, dependencies?: Dependencies): Promise<ServicePaginatedResponse<Array<Group>>> => {

    try {

        const serviceResponse: ServicePaginatedResponse<Array<Group>> = {
            data: []
        };

        const setGetFetchOptions = dependencies?.setGetFetchOptions ?? setGetFetchOptionsHelper;
        const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

        const id: string = user.id;
        const paginationQueryParams: string = getApiPaginationQueryParams({ pagination });
        const memberShipQueryParam: string = isMember ? '&ismember=true' : '&ismember=false';

        const apiUrl: string = `${getEnvVar({ name: 'NEXT_PUBLIC_API_BASE_URL' })}/v1/users/${id}/groups?${paginationQueryParams}${memberShipQueryParam}`;
        const apiResponse: FetchResponse = await fetchJSON(apiUrl, setGetFetchOptions({}), 30000);
        const apiData: ApiPaginatedResponse<any> = apiResponse.json;
        const apiMeta: any = apiResponse.meta;

        const { ok, status, statusText } = apiMeta;

        if(!ok){

            return {
                errors: {
                    [status]: statusText
                }
            }

        }

        apiData.data?.forEach((datum) => {

            serviceResponse.data.push({
                content: {
                    mainHeadingHtml: datum.nameText,
                    strapLineText: datum.strapLineText
                } as any,
                groupId: datum.slug,
                totalMemberCount: datum.memberCount ?? 0,
                totalDiscussionCount: datum.discussionCount ?? 0,
                image: datum.image ? {
                    src: `${process.env.NEXT_PUBLIC_API_BASE_URL}${datum.image?.source}`,
                    height: datum.image?.height ?? null,
                    width: datum.image?.width ?? null,
                    altText: 'TBC'
                } : null
            });

        });

        serviceResponse.pagination = getClientPaginationFromApi({ apiPaginatedResponse: apiData });

        return serviceResponse;

    } catch(error){

        const { message } = error;

        return {
            errors: { error: message },
        };

    }

}