import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch'
import { ServiceError } from '..'
import { getApiPaginationQueryParams } from '@helpers/routing/getApiPaginationQueryParams'
import { getClientPaginationFromApi } from '@helpers/routing/getClientPaginationFromApi'
import { ServicePaginatedResponse, ServiceResponse } from '@appTypes/service'
import { FetchResponse } from '@appTypes/fetch'
import { ApiResponse } from '@appTypes/service'
import { DiscussionComment } from '@appTypes/discussion'
import { User } from '@appTypes/user'
import { Pagination } from '@appTypes/pagination'
import { mapToProfileImageObject } from '@helpers/util/data'
import jwtHeader from '@helpers/util/jwt/jwtHeader'
import {
    CommentLike,
    getGroupDiscussionCommentLikes,
} from '@services/getGroupDiscussionCommentLikes'

declare type Options = {
    groupId: string
    discussionId: string
    user: User
    pagination: Pagination
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const getGroupDiscussionComments = async (
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

    const { id } = user
    const paginationQueryParams: string = getApiPaginationQueryParams({
        pagination,
        defaults: {
            pageNumber: 1,
            pageSize: 30,
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
            'An unexpected error occurred when attempting to get the group discussion comments',
            {
                serviceId: services.GET_GROUP_DISCUSSION_COMMENTS,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }
    const commentLikeRequests: Array<Promise<ServiceResponse<CommentLike[]>>> =
        []

    apiData.data?.forEach((datum) => {
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
                image: mapToProfileImageObject(
                    datum.firstRegistered?.by?.image,
                    'Profile image'
                ),
            },
            created: datum.firstRegistered?.atUtc ?? '',
            replyCount: datum.repliesCount ?? 0,
            likeCount: datum.likesCount ?? 0,
            isLiked: datum.currentUser?.liked,
            likes: [],
        })
        commentLikeRequests.push(
            getGroupDiscussionCommentLikes({
                groupId,
                discussionId,
                commentId: datum.id,
                user,
                pagination,
            })
        )
    })
    const getAllLikes = await Promise.all(commentLikeRequests)
    const likesCollection = getAllLikes.map(
        (serviceResponse) => serviceResponse.data
    )

    serviceResponse.data.forEach(({ commentId }, i) => {
        const commentLikes = likesCollection.find((commentLikes) => {
            const commentHasLikes = !!commentLikes[0]
            const likesAreThisComment = commentLikes[0].id === commentId
            return commentHasLikes && likesAreThisComment
        })
        serviceResponse.data[i].likes = commentLikes
    })

    serviceResponse.pagination = getClientPaginationFromApi({
        apiPaginatedResponse: apiData,
    })
    return serviceResponse
}
