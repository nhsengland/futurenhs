import { setGetFetchOpts as setGetFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { getEnvVar } from '@helpers/util/env';
import { ServiceResponse } from '@appTypes/service';
import { Group } from '@appTypes/group';
import { User } from '@appTypes/user';

declare type Options = ({
    user: User;
    slug: string;
    page: 'home' | 'forum' | 'files' | 'members'
});

declare type Dependencies = ({
    setGetFetchOptions: any;
    fetchJSON: any;
});

export const getGroup = async ({
    user,
    slug,
    page
}: Options, dependencies?: Dependencies): Promise<ServiceResponse<Group>> => {

    try {

        const setGetFetchOptions = dependencies?.setGetFetchOptions ?? setGetFetchOptionsHelper;
        const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

        const id: string = user.id;
        const apiUrl: string = `${getEnvVar({ name: 'NEXT_PUBLIC_API_BASE_URL' })}/${id}/groups/${slug}?page=${page}`;
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
                titleText: json.pageHeader.nameText, 
                metaDescriptionText: 'A Future NHS group',
                mainHeadingHtml: json.pageHeader.nameText,
                strapLineText: json.pageHeader.strapLineText
            },
            image: json.pageHeader?.image ? {
                src: `${process.env.NEXT_PUBLIC_API_ORIGIN}${json.pageHeader.image?.source}`,
                height: json.pageHeader?.image?.height ?? null,
                width: json.pageHeader?.image?.width ?? null,
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