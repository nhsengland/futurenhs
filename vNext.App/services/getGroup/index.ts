import { setGetFetchOpts as setGetFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { getEnvVar } from '@helpers/util/env';
import { ServiceResponse } from '@appTypes/service';
import { Group } from '@appTypes/group';

declare type Options = ({
    slug: string;
});

declare type Dependencies = ({
    setGetFetchOptions: any;
    fetchJSON: any;
});

export const getGroup = async ({
    slug
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<Group>> => {

    try {

        const setGetFetchOptions = dependencies?.setGetFetchOptions ?? setGetFetchOptionsHelper;
        const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

        const apiUrl: string = `${getEnvVar({ name: 'NEXT_PUBLIC_API_BASE_URL' })}/v1/groups/${slug}`;

        const { json, meta } = await fetchJSON(apiUrl, setGetFetchOptions({}), 30000);
        const { ok, status, statusText } = meta;

        if(!ok){

            return {
                errors: {
                    [status]: statusText
                }
            }

        }

        const data = {
            content: {
                titleText: json.name, 
                metaDescriptionText: 'A Future NHS group',
                mainHeadingHtml: json.name,
                strapLineText: 'Testing unreleased features of the FutureNHS platform'//json.pageHeader.strapLineText
            },
            image: json.image ? {
                src: `${process.env.NEXT_PUBLIC_API_BASE_URL}${json.image?.source}`,
                height: json?.image?.height ?? null,
                width: json?.image?.width ?? null,
                altText: 'TBC'
            } : null
        };

        return {
            data: data,
            errors: {}
        };

    } catch(error){

        const { message } = error;

        return {
            errors: { error: message },
        };

    }

}