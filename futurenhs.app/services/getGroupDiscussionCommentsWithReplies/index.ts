import { setFetchOpts as setFetchOptionsHelper, fetchJSON as fetchJSONHelper } from '@helpers/fetch';
import { ServiceError } from '..';
import { getGroupDiscussionCommentReplies as getGroupDiscussionCommentRepliesService } from '@services/getGroupDiscussionCommentReplies';
import { getApiPaginationQueryParams } from '@helpers/routing/getApiPaginationQueryParams';
import { getClientPaginationFromApi } from '@helpers/routing/getClientPaginationFromApi';
import { ServicePaginatedResponse } from '@appTypes/service';
import { FetchResponse } from '@appTypes/fetch';
import { ApiResponse } from '@appTypes/service';
import { DiscussionComment } from '@appTypes/discussion';
import { User } from '@appTypes/user';
import { Pagination } from '@appTypes/pagination';

declare type Options = ({
    groupId: string;
    discussionId: string;
    user: User;
    pagination: Pagination;
});

declare type Dependencies = ({
    setFetchOptions: any;
    fetchJSON: any;
    getGroupDiscussionCommentReplies: any;
});

export const getGroupDiscussionCommentsWithReplies = async ({
    groupId,
    discussionId,
    user,
    pagination
}: Options, dependencies?: Dependencies): Promise<ServicePaginatedResponse<Array<DiscussionComment>>> => {

    const serviceResponse: ServicePaginatedResponse<Array<DiscussionComment>> = {
        data: []
    };

    const setFetchOptions = dependencies?.setFetchOptions ?? setFetchOptionsHelper;
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper;
    const getGroupDiscussionCommentReplies = dependencies?.getGroupDiscussionCommentReplies ?? getGroupDiscussionCommentRepliesService;

    const { id } = user;
    const paginationQueryParams: string = getApiPaginationQueryParams({
        pagination,
        defaults: {
            pageNumber: 1,
            pageSize: 10
        }
    });

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/groups/${groupId}/discussions/${discussionId}/comments?${paginationQueryParams}`;
    const apiResponse: FetchResponse = await fetchJSON(apiUrl, setFetchOptions({ method: 'GET' }), 30000);
    const apiData: ApiResponse<any> = apiResponse.json;
    const apiMeta: any = apiResponse.meta;

    const { ok, status, statusText } = apiMeta;

    if(!ok){

        throw new ServiceError('Error getting group discussion comments with replies', {
            status: status,
            statusText: statusText,
            body: apiData
        });

    }

    const commentIdsWithReplies: Array<string> = [];
    const commentRepliesRequests: Array<any> = [];

    apiData.data?.forEach(async (datum) => {

        if (datum?.repliesCount > 0) {

            commentIdsWithReplies.push(datum.id);
            commentRepliesRequests.push(getGroupDiscussionCommentReplies({
                user: user,
                groupId: groupId,
                discussionId: discussionId,
                commentId: datum.id,
                pagination: {
                    pageNumber: 1,
                    pageSize: 30
                }
            }));

        }

        serviceResponse.data.push({
            commentId: datum.id,
            text: {
                body: datum.content
            },
            createdBy: {
                id: datum.firstRegistered?.by?.id ?? '',
                text: {
                    userName: datum.firstRegistered?.by?.name ?? ''
                }
            },
            created: datum.firstRegistered?.atUtc ?? '',
            replyCount: datum.repliesCount ?? 0,
            likeCount: datum.likesCount ?? 0,
            isLiked: datum.currentUser?.liked,
            replies: []
        });

    });

    const [...commentReplies] = await Promise.all(commentRepliesRequests);

    commentIdsWithReplies.forEach((commentId: string, index: number) => {

        const parentDiscussion: any = serviceResponse.data.find((comment) => comment.commentId === commentId);

        if (parentDiscussion && commentReplies[index]?.data?.length > 0) {

            parentDiscussion.replies = commentReplies[index]?.data;

        }

    });

    serviceResponse.pagination = getClientPaginationFromApi({ apiPaginatedResponse: apiData });

    return serviceResponse;

}