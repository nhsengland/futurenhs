import { setGetFetchOpts as setGetFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { ServicePaginatedResponse } from '@appTypes/service';
import { File } from '@appTypes/file';
import { User } from '@appTypes/user';

import { mockFolderData } from './mockFolderData';
import { mockFileData } from './mockFileData';

declare type Options = ({
    user: User;
    filters?: {
        groupId?: string;
        folderId?: string;
        fileId?: string;
    };
    pagination: {
        pageNumber?: number;
        pageSize?: number;
    };
});

declare type Dependencies = ({
    setGetFetchOptions: any;
    fetchJSON: any;
});

export const getGroupFiles = async ({
    user,
    filters,
    pagination
}: Options, dependencies?: Dependencies): Promise<ServicePaginatedResponse<Array<File>>> => {

    try {

        const setGetFetchOptions = dependencies?.setGetFetchOptions ?? setGetFetchOptionsHelper;
        const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;

        const { pageNumber, pageSize } = pagination;

        const id: string = user.id;
        const resource: string = filters?.groupId ? filters?.groupId : '';
        const apiUrl: string = `${process.env.NEXT_PUBLIC_API_BASE_URL}/v1/users/${id}/${resource}?pageNumber=${pageNumber}&pageSize=${pageSize}`;

        // const { json, meta } = await fetchJSON(apiUrl, setGetFetchOptions({}), 30000);
        // const { ok, status, statusText } = meta;

        // if(!ok){

        //     return {
        //         errors: {
        //             [status]: statusText
        //         }
        //     }

        // }
        const sortedFiles: Array<any> = filters.folderId ? mockFileData.sort((a, b) => new Date(b.modified) as any - (new Date(a.modified) as any)) : [];
        const mockData: Array<any> = [...mockFolderData, ...sortedFiles].map((item) => {

            return {
                id: item.id,
                type: item.type,
                name: item.name,
                bodyHtml: item.description,
                modified: item.modified,
                createdBy: item.createdBy,
                modifiedBy: item.modifiedBy 
            }

        });

        const lowerBound: number = pageNumber === 1 ? 0 : (pageNumber - 1) * pageSize;
        const upperBound: number = pageNumber === 1 ? pageSize : pageNumber * pageSize;
        const json = {
            data: mockData.slice(lowerBound, upperBound),
            pagination: {
                pageNumber: pageNumber,
                pageSize: pageSize,
                totalRecords: mockData.length
            }
        };

        return json;

    } catch(error){

        const { message } = error;

        return {
            errors: { error: message },
        };

    }

}