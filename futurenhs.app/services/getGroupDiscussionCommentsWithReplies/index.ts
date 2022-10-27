import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch'
import { ServiceError } from '..'
import { getGroupDiscussionCommentReplies as getGroupDiscussionCommentRepliesService } from '@services/getGroupDiscussionCommentReplies'
import { getApiPaginationQueryParams } from '@helpers/routing/getApiPaginationQueryParams'
import { getClientPaginationFromApi } from '@helpers/routing/getClientPaginationFromApi'
import { ServicePaginatedResponse } from '@appTypes/service'
import { FetchResponse } from '@appTypes/fetch'
import { ApiResponse } from '@appTypes/service'
import { DiscussionComment } from '@appTypes/discussion'
import { User } from '@appTypes/user'
import { Pagination } from '@appTypes/pagination'
import { mapToProfileImageObject } from '@helpers/util/data'
import jwtHeader from '@helpers/util/jwt/jwtHeader'

declare type Options = {
    groupId: string
    discussionId: string
    user: User
    pagination: Pagination
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
    getGroupDiscussionCommentReplies: any
}

export const getGroupDiscussionCommentsWithReplies = async (
    { groupId, discussionId, user, pagination }: Options,
    dependencies?: Dependencies
): Promise<ServicePaginatedResponse<Array<DiscussionComment>>> => {
    const serviceResponse: ServicePaginatedResponse<Array<DiscussionComment>> =
        {
            data: [],
        }

    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper
    const getGroupDiscussionCommentReplies =
        dependencies?.getGroupDiscussionCommentReplies ??
        getGroupDiscussionCommentRepliesService

    const { id } = user
    const paginationQueryParams: string = getApiPaginationQueryParams({
        pagination,
        defaults: {
            pageNumber: 1,
            pageSize: 10,
        },
    })

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/groups/${groupId}/discussions/${discussionId}/comments?${paginationQueryParams}`
    const authHeader = jwtHeader(user.accessToken)
    const apiHeaders = setFetchOptions({
        method: requestMethods.GET,
        headers: authHeader,
    })
    const apiResponse: FetchResponse = await fetchJSON(
        apiUrl,
        apiHeaders,
        defaultTimeOutMillis
    )

    const apiData: ApiResponse<any> = apiResponse.json
    const apiMeta: any = apiResponse.meta

    const { ok, status, statusText } = apiMeta

    if (!ok) {
        throw new ServiceError(
            'An unexpected error occurred when attempting to get the group discussion comments with replies',
            {
                serviceId: services.GET_GROUP_DISCUSSION_COMMENTS_WITH_REPLIES,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    const commentIdsWithReplies: Array<string> = []
    const commentRepliesRequests: Array<any> = []

    apiData.data?.forEach(async (datum) => {
        if (datum?.repliesCount > 0) {
            commentIdsWithReplies.push(datum.id)
            commentRepliesRequests.push(
                getGroupDiscussionCommentReplies({
                    user: user,
                    groupId: groupId,
                    discussionId: discussionId,
                    commentId: datum.id,
                    pagination: {
                        pageNumber: 1,
                        pageSize: 30,
                    },
                })
            )
        }

        serviceResponse.data.push({
            commentId: datum.id,
            text: {
                body: datum.content,
            },
            createdBy: {
                id: datum.firstRegistered?.by?.id ?? '',
                text: {
                    userName: datum.firstRegistered?.by?.name ?? '',
                },
                image: mapToProfileImageObject(datum.firstRegistered?.by?.image, 'Profile image')
            },
            created: datum.firstRegistered?.atUtc ?? '',
            replyCount: datum.repliesCount ?? 0,
            likeCount: datum.likesCount ?? 0,
            isLiked: datum.currentUser?.liked,
            replies: [],
        })
    })

    const [...commentReplies] = await Promise.all(commentRepliesRequests)

    commentIdsWithReplies.forEach((commentId: string, index: number) => {
        const parentComment: any = serviceResponse.data.find(
            (comment) => comment.commentId === commentId
        )

        if (parentComment && commentReplies[index]?.data?.length > 0) {
            parentComment.replies = commentReplies[index]?.data
        }
    })

    serviceResponse.pagination = getClientPaginationFromApi({
        apiPaginatedResponse: apiData,
    })

    return serviceResponse
}
