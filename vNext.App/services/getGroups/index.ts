import { setGetFetchOpts as setGetFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { getEnvVar } from '@helpers/util/env';
import { ServicePaginatedResponse } from '@appTypes/service';
import { Pagination } from '@appTypes/pagination';
import { Group } from '@appTypes/group';
import { User } from '@appTypes/user';

declare type Options = ({
    user: User;
    filters?: {
        isMember?: boolean;
    };
    pagination?: Pagination;
});

declare type Dependencies = ({
    setGetFetchOptions: any;
    fetchJSON: any;
});

export const getGroups = async ({
    user,
    filters,
    pagination
}: Options, dependencies?: Dependencies): Promise<ServicePaginatedResponse<Array<Group>>> => {

    try {

        const setGetFetchOptions = dependencies?.setGetFetchOptions ?? setGetFetchOptionsHelper;
        const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

        const { pageNumber, pageSize } = pagination;

        const id: string = user.id;
        const resource: string = filters?.isMember ? 'groups' : 'discover/groups';
        const apiUrl: string = `${getEnvVar({ name: 'NEXT_PUBLIC_API_BASE_URL' })}/v1/users/${id}/${resource}?pageNumber=${pageNumber}&pageSize=${pageSize}`;

        const { json, meta } = await fetchJSON(apiUrl, setGetFetchOptions({}), 30000);
        const { ok, status, statusText } = meta;

        if(!ok){

            return {
                errors: {
                    [status]: statusText
                }
            }

        }

        json.data.forEach((datum, index) => {

            json.data[index] = {
                content: {
                    mainHeadingHtml: datum.nameText,
                    strapLineText: datum.strapLineText
                },
                slug: datum.slug,
                totalMemberCount: datum.memberCount ?? 0,
                totalDiscussionCount: datum.discussionCount ?? 0,
                image: datum.image ? {
                    src: `${process.env.NEXT_PUBLIC_API_BASE_URL}${datum.image?.source}`,
                    height: datum.image?.height ?? null,
                    width: datum.image?.width ?? null,
                    altText: 'TBC'
                } : null
            } as Group;

        });

        json.pagination = {
            pageNumber: json.pageNumber,
            pageSize: json.pageSize,
            totalRecords: json.totalRecords
        }

        return json;

    } catch(error){

        const { message } = error;

        return {
            errors: { error: message },
        };

    }

}