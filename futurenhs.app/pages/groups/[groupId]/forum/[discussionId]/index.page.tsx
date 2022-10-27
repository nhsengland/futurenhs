import { GetServerSideProps } from 'next'
import { useState, useCallback, useRef } from 'react'
import { useRouter } from 'next/router'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { layoutIds, groupTabIds } from '@constants/routes'
import { requestMethods } from '@constants/fetch'
import { withUser } from '@helpers/hofs/withUser'
import { withRoutes } from '@helpers/hofs/withRoutes'
import { withGroup } from '@helpers/hofs/withGroup'
import { withTextContent } from '@helpers/hofs/withTextContent'
import { getGroupDiscussion } from '@services/getGroupDiscussion'
import {
    selectUser,
    selectParam,
    selectPagination,
    selectFormData,
    selectRequestMethod,
    selectCsrfToken,
    selectPageProps,
} from '@helpers/selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import { Pagination } from '@appTypes/pagination'
import { ServerSideFormData } from '@helpers/util/form'
import { postGroupDiscussionComment } from '@services/postGroupDiscussionComment'
import { postGroupDiscussionCommentReply } from '@services/postGroupDiscussionCommentReply'
import { putGroupDiscussionCommentLike } from '@services/putGroupDiscussionCommentLike'
import { selectFormErrors } from '@helpers/selectors/forms'
import { actions as actionsConstants } from '@constants/actions'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { getGenericFormError } from '@helpers/util/form'
import { formTypes } from '@constants/forms'
import { dateTime } from '@helpers/formatters/dateTime'
import { initials } from '@helpers/formatters/initials'
import { getFormattedCommentId } from '@helpers/dom'
import { routeParams } from '@constants/routes'
import { Link } from '@components/generic/Link'
import { Accordion } from '@components/generic/Accordion'
import { DynamicListContainer } from '@components/layouts/DynamicListContainer'
import { RichText } from '@components/generic/RichText'
import { SVGIcon } from '@components/generic/SVGIcon'
import { Comment } from '@components/blocks/Comment'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { Form } from '@components/forms/Form'
import { ErrorSummary } from '@components/generic/ErrorSummary'
import { ErrorBoundary } from '@components/layouts/ErrorBoundary'
import { PaginationWithStatus } from '@components/generic/PaginationWithStatus'
import { UserMeta } from '@components/blocks/UserMeta'
import { getGroupDiscussionCommentsWithReplies } from '@services/getGroupDiscussionCommentsWithReplies'
import { getRouteToParam } from '@helpers/routing/getRouteToParam'
import { FormErrors, FormConfig } from '@appTypes/form'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { useFormConfig } from '@helpers/hooks/useForm'
import { Image } from '@appTypes/image'
import { ActionLink } from '@components/generic/ActionLink'
import { GroupPage } from '@appTypes/page'
import { Discussion, DiscussionComment } from '@appTypes/discussion'
import { GroupsPageTextContent } from '@appTypes/content'

declare interface ContentText extends GroupsPageTextContent {
    createdByLabel?: string
    lastCommentLabel?: string
    totalRecordsLabel?: string
    viewCountLabel?: string
    moreRepliesLabel?: string
    fewerRepliesLabel?: string
    signedInLabel?: string
}

export interface Props extends GroupPage {
    contentText: ContentText
    discussionId: string
    discussion: Discussion
    discussionCommentsList: Array<DiscussionComment>
}

/**
 * Group discussion template
 */
export const GroupDiscussionPage: (props: Props) => JSX.Element = ({
    groupId,
    discussionId,
    user,
    csrfToken,
    contentText,
    actions,
    routes,
    discussion,
    discussionCommentsList,
    pagination,
    forms,
    services = {
        getGroupDiscussionCommentsWithReplies:
            getGroupDiscussionCommentsWithReplies,
        postGroupDiscussionComment: postGroupDiscussionComment,
        postGroupDiscussionCommentReply: postGroupDiscussionCommentReply,
        putGroupDiscussionCommentLike: putGroupDiscussionCommentLike,
    },
}) => {
    const router = useRouter()
    const errorSummaryRef: any = useRef()
    const commentButtonRef: any = useRef(null)

    const commentFormConfig: FormConfig = useFormConfig(
        formTypes.CREATE_DISCUSSION_COMMENT,
        forms[formTypes.CREATE_DISCUSSION_COMMENT]
    )
    const [errors, setErrors] = useState(
        Object.assign(
            {},
            selectFormErrors(forms, formTypes.CREATE_DISCUSSION_COMMENT),
            selectFormErrors(forms, formTypes.CREATE_DISCUSSION_COMMENT_REPLY)
        )
    )
    const [dynamicDiscussionCommentsList, setDiscussionsList] = useState(
        discussionCommentsList
    )
    const [dynamicPagination, setPagination] = useState(pagination)
    const [newReplyId, setNewReplyId] = useState(null)

    const backLinkHref: string = getRouteToParam({
        router: router,
        paramName: routeParams.DISCUSSIONID,
    })

    const {
        createdByLabel,
        lastCommentLabel,
        totalRecordsLabel,
        viewCountLabel,
        moreRepliesLabel,
        fewerRepliesLabel,
        secondaryHeading,
        signedInLabel,
    } = contentText ?? {}

    const { id, text } = user ?? {}
    const { userName } = text ?? {}
    const {
        text: discussionText,
        created,
        createdBy,
        responseCount,
        modified,
        modifiedBy,
        viewCount,
    } = discussion ?? {}
    const { title, body: discussionBody } = discussionText ?? {}

    const formattedDiscussionid: string = getFormattedCommentId(discussionId)
    const shouldRenderCommentAndReplyForms: boolean = actions.includes(
        actionsConstants.GROUPS_COMMENTS_ADD
    )
    const shouldEnableLoadMore: boolean = false
    const hasDiscussionComments: boolean =
        dynamicDiscussionCommentsList?.length > 0
    const creatorUserInitials: string = initials({
        value: createdBy?.text?.userName,
    })
    const creatorProfileImage: Image = createdBy?.image
    const creatorUserName: string = createdBy?.text?.userName
    const creatorUserId: string = createdBy?.id
    const createdDate: string = dateTime({ value: created })
    const lastCommentUserName: string = modifiedBy?.text?.userName
    const lastCommentDate: string = dateTime({ value: modified })
    let renderBackToTopIcon: boolean = false
    /**
     * Handle likes on comments
     */
    const handleLike = useCallback(
        async (commentId: string, isLiked: boolean): Promise<void> => {
            try {
                await services.putGroupDiscussionCommentLike({
                    user,
                    groupId,
                    discussionId,
                    commentId,
                    shouldLike: isLiked,
                })
            } catch (error) {
                console.log(error)
            }
        },
        []
    )

    /**
     * Handle client-side validation failure in forms
     */
    const handleValidationFailure = (errors: FormErrors): void => {
        setErrors(errors)
        errorSummaryRef?.current?.focus?.()
    }

    /**
     * Handle client-side add comment click
     */
    const handleAddCommentClick = () => {
        commentButtonRef.current.scrollIntoView({ behavior: 'smooth' })
    }

    /**
     * Handle client-side comment submission
     */
    const handleCommentSubmit = async (
        formData: FormData
    ): Promise<FormErrors> => {
        return new Promise((resolve) => {
            const headers: any = getStandardServiceHeaders({
                csrfToken,
                accessToken: user.accessToken,
            })

            services
                .postGroupDiscussionComment({
                    groupId,
                    discussionId,
                    user,
                    headers,
                    body: formData,
                })
                .then(() => {
                    const targetPageNumber: number = Math.ceil(
                        (Number(dynamicPagination.totalRecords) + 1) /
                            dynamicPagination.pageSize
                    )

                    setErrors({})
                    handleGetPage({
                        pageNumber: targetPageNumber,
                        pageSize: dynamicPagination.pageSize,
                    })

                    resolve({})
                })
                .catch((error) => {
                    const errors: FormErrors =
                        getServiceErrorDataValidationErrors(error) ||
                        getGenericFormError(error)

                    setErrors(errors)
                    resolve(errors)
                })
        })
    }

    /**
     * Handle client-side comment reply submission
     */
    const handleCommentReplySubmit = async (
        formData: FormData
    ): Promise<FormErrors> => {
        return new Promise((resolve) => {
            const commentId: any = formData.get('_instance-id')
            const headers: any = getStandardServiceHeaders({
                csrfToken,
                accessToken: user.accessToken,
            })

            services
                .postGroupDiscussionCommentReply({
                    groupId,
                    discussionId,
                    user,
                    commentId,
                    headers,
                    body: formData,
                })
                .then((newReplyId) => {
                    setErrors({})
                    handleGetPage(dynamicPagination as any).then(() => {
                        setNewReplyId(newReplyId)
                    })

                    resolve({})
                })
                .catch((error) => {
                    const errors: FormErrors =
                        getServiceErrorDataValidationErrors(error) ||
                        getGenericFormError(error)

                    setErrors(errors)
                    resolve(errors)
                })
        })
    }

    /**
     * Handle client-side pagination
     */
    const handleGetPage = async ({
        pageNumber: requestedPageNumber,
        pageSize: requestedPageSize,
    }) => {
        try {
            const { data: newComments, pagination } =
                await services.getGroupDiscussionCommentsWithReplies({
                    user,
                    groupId,
                    discussionId,
                    pagination: {
                        pageNumber: requestedPageNumber,
                        pageSize: requestedPageSize,
                    },
                })

            setDiscussionsList(newComments)
            setPagination(pagination)
        } catch (error) {
            console.log(error)
        }
    }

    /**
     * Render replies to individual comments
     */
    const renderReplies = ({
        replies,
    }: Partial<DiscussionComment>): Array<JSX.Element> => {
        /**
         * Get the origin comment for the reply
         */
        const recursiveCommentMapper = (
            comments: Array<DiscussionComment>,
            callBack: (comment: DiscussionComment) => boolean | void
        ): void => {
            comments?.every((comment: DiscussionComment) => {
                const { replies } = comment

                if (callBack(comment) === true) {
                    return false
                }

                replies?.length && recursiveCommentMapper(replies, callBack)

                return true
            })
        }

        return replies?.map(
            ({
                commentId,
                originCommentId,
                created,
                createdBy,
                text,
                likeCount,
                isLiked,
            }) => {
                const formattedCommentId: string =
                    getFormattedCommentId(commentId)
                const replyingUserInitials: string = initials({
                    value: createdBy?.text?.userName,
                })
                const replyingUserName: string = createdBy?.text?.userName
                const replyingUserId: string = createdBy?.id
                const replyingUserProfileImage: Image = createdBy?.image
                const replyCreatedDate: string = dateTime({ value: created })
                const shouldEnableLikes: boolean =
                    shouldRenderCommentAndReplyForms && createdBy.id !== id

                const { body } = text ?? {}

                let originComment: DiscussionComment = null

                recursiveCommentMapper(
                    dynamicDiscussionCommentsList,
                    (comment: DiscussionComment) => {
                        if (comment.commentId === originCommentId) {
                            originComment = comment
                            return true
                        }

                        return false
                    }
                )

                return (
                    <li
                        key={commentId}
                        className="c-comment_reply-container u-m-0 u-py-6"
                    >
                        <Comment
                            id={formattedCommentId}
                            image={replyingUserProfileImage}
                            commentId={commentId}
                            csrfToken={csrfToken}
                            initialErrors={errors}
                            originComment={originComment}
                            text={{
                                userName: replyingUserName,
                                initials: replyingUserInitials,
                                body: body,
                            }}
                            userProfileLink={`${routes.groupMembersRoot}/${replyingUserId}`}
                            date={replyCreatedDate}
                            shouldEnableReplies={
                                shouldRenderCommentAndReplyForms
                            }
                            replyValidationFailAction={handleValidationFailure}
                            replySubmitAction={handleCommentReplySubmit}
                            shouldEnableLikes={shouldEnableLikes}
                            likeCount={likeCount}
                            isLiked={isLiked}
                            likeAction={handleLike}
                            className="c-comment--reply u-border-l-theme-8"
                        />
                    </li>
                )
            }
        )
    }

    /**
     * Render
     */
    return (
        <>
            <LayoutColumn className="c-page-body">
                {shouldRenderCommentAndReplyForms && (
                    <ErrorSummary
                        ref={errorSummaryRef}
                        errors={errors}
                        className="u-mb-10"
                    />
                )}

                <ActionLink
                    href={backLinkHref}
                    iconName="icon-chevron-left"
                    className="u-mb-8"
                    text={{
                        body: 'Back to discussions',
                        ariaLabel: 'Go back to list of group discussions',
                    }}
                />

                <div
                    id={formattedDiscussionid}
                    tabIndex={-1}
                    className="nhsuk-card focus:u-outline-none u-border-b-4 u-border-b-theme-8"
                >
                    <div className="nhsuk-card__content">
                        <h3 className="nhsuk-card__heading">{title}</h3>
                        {discussionBody && (
                            <RichText
                                bodyHtml={discussionBody}
                                wrapperElementType="div"
                                className="u-mb-8"
                            />
                        )}

                        <LayoutColumnContainer>
                            <LayoutColumn tablet={8}>
                                <UserMeta
                                    image={creatorProfileImage}
                                    text={{
                                        initials: creatorUserInitials,
                                    }}
                                    className="u-m-0 u-text-theme-7"
                                >
                                    <span className="u-text-bold u-block">
                                        {createdByLabel}{' '}
                                        <Link
                                            href={`${routes.groupMembersRoot}/${creatorUserId}`}
                                        >
                                            <a>{creatorUserName}</a>
                                        </Link>{' '}
                                        {createdDate}
                                    </span>
                                    {responseCount > 0 && lastCommentUserName && (
                                        <span className="u-block u-mt-1">
                                            {lastCommentLabel}{' '}
                                            <Link
                                                href={`${routes.groupMembersRoot}/${creatorUserId}`}
                                            >
                                                <a>{lastCommentUserName}</a>
                                            </Link>{' '}
                                            {lastCommentDate}
                                        </span>
                                    )}
                                </UserMeta>
                            </LayoutColumn>
                            <LayoutColumn
                                tablet={4}
                                className="u-self-end tablet:u-text-right u-text-theme-7 u-text-bold u-mt-4"
                            >
                                {responseCount > 0 && (
                                    <span className="u-mr-5">
                                        <SVGIcon
                                            name="icon-comments"
                                            className="u-h-5 u-w-5 u-fill-theme-8 u-mr-1 u-align-middle"
                                        />
                                        {totalRecordsLabel}: {responseCount}
                                    </span>
                                )}
                                {/* {viewCount > 0 &&
                            <><SVGIcon name="icon-view" className="u-h-5 u-w-5 u-fill-theme-8 u-mr-1 u-align-middle" />{viewCountLabel}: {viewCount}</>
                        } */}
                            </LayoutColumn>
                        </LayoutColumnContainer>
                    </div>
                </div>

                <hr />
                {responseCount > 0 && (
                    <p className="u-hidden tablet:u-block u-text-lead u-text-bold">
                        {`Comments: ${responseCount}`}
                    </p>
                )}

                {responseCount > 2 && (
                    <div>
                        {(renderBackToTopIcon = true)}
                        <button
                            onClick={handleAddCommentClick}
                            className="c-form_submit-button c-button u-w-full tablet:u-w-auto u-mb-6"
                        >
                            Reply to this topic
                        </button>
                    </div>
                )}
                <ErrorBoundary boundaryId="group-discussion-comments">
                    {hasDiscussionComments && (
                        <DynamicListContainer
                            containerElementType="ul"
                            shouldEnableLoadMore={shouldEnableLoadMore}
                            className="u-list-none u-p-0"
                            nestedChildId={
                                newReplyId && `comment-${newReplyId}`
                            }
                        >
                            {dynamicDiscussionCommentsList?.map(
                                (
                                    {
                                        commentId,
                                        created,
                                        createdBy,
                                        text,
                                        likeCount,
                                        isLiked,
                                        replies,
                                    },
                                    index
                                ) => {
                                    const formattedCommentId: string =
                                        getFormattedCommentId(commentId)
                                    const originDiscussion: DiscussionComment =
                                        {
                                            commentId: discussionId,
                                            createdBy: {
                                                text: {
                                                    userName: creatorUserName,
                                                },
                                            },
                                            text: {
                                                body: discussionBody,
                                            },
                                        }
                                    const commenterUserInitials: string =
                                        initials({
                                            value: createdBy?.text?.userName,
                                        })
                                    const commenterUserName: string =
                                        createdBy?.text?.userName
                                    const commenterUserId: string =
                                        createdBy?.id
                                    const commenterProfileImage: Image =
                                        createdBy?.image
                                    const commentCreatedDate: string = dateTime(
                                        { value: created }
                                    )
                                    const hasReply: boolean =
                                        replies?.length > 0
                                    const hasReplies: boolean =
                                        replies?.length > 1
                                    const repliesComponents: Array<JSX.Element> =
                                        renderReplies({ replies })
                                    const additionalRepliesAccordionId: string = `${commentId}-replies`
                                    const hasNewReply: boolean =
                                        repliesComponents?.some(
                                            (reply) => reply.key === newReplyId
                                        )
                                    const shouldEnableLikes: boolean =
                                        shouldRenderCommentAndReplyForms &&
                                        createdBy.id !== id

                                    const { body: commentBody } = text ?? {}

                                    return (
                                        <li key={index}>
                                            <Comment
                                                id={formattedCommentId}
                                                commentId={commentId}
                                                originComment={originDiscussion}
                                                csrfToken={csrfToken}
                                                initialErrors={errors}
                                                image={commenterProfileImage}
                                                text={{
                                                    userName: commenterUserName,
                                                    initials:
                                                        commenterUserInitials,
                                                    body: commentBody,
                                                }}
                                                userProfileLink={`${routes.groupMembersRoot}/${commenterUserId}`}
                                                date={commentCreatedDate}
                                                shouldEnableReplies={
                                                    shouldRenderCommentAndReplyForms
                                                }
                                                replyValidationFailAction={
                                                    handleValidationFailure
                                                }
                                                replySubmitAction={
                                                    handleCommentReplySubmit
                                                }
                                                shouldEnableLikes={
                                                    shouldEnableLikes
                                                }
                                                likeCount={likeCount}
                                                isLiked={isLiked}
                                                likeAction={handleLike}
                                                className="u-border-l-theme-8"
                                            >
                                                {hasReply && (
                                                    <ul className="u-list-none c-comment_replies-list u-p-0">
                                                        {repliesComponents[0]}
                                                    </ul>
                                                )}
                                                {hasReplies && (
                                                    <Accordion
                                                        id={
                                                            additionalRepliesAccordionId
                                                        }
                                                        isOpen={hasNewReply}
                                                        toggleOpenChildren={
                                                            <span>
                                                                {
                                                                    fewerRepliesLabel
                                                                }
                                                            </span>
                                                        }
                                                        toggleClosedChildren={
                                                            <span>
                                                                {
                                                                    moreRepliesLabel
                                                                }
                                                            </span>
                                                        }
                                                        toggleClassName="c-comment_replies-toggle u-text-bold"
                                                    >
                                                        <ul className="u-list-none u-m-0 u-p-0">
                                                            {repliesComponents.splice(
                                                                1
                                                            )}
                                                        </ul>
                                                    </Accordion>
                                                )}
                                            </Comment>
                                        </li>
                                    )
                                }
                            )}
                        </DynamicListContainer>
                    )}
                    <PaginationWithStatus
                        id="discussion-list-pagination"
                        shouldEnableLoadMore={shouldEnableLoadMore}
                        getPageAction={handleGetPage}
                        {...dynamicPagination}
                        className="u-mb-10"
                    />
                </ErrorBoundary>
                {shouldRenderCommentAndReplyForms && (
                    <>
                        <h3 className="nhsuk-heading-l">{secondaryHeading}</h3>
                        <p className="u-text-bold">
                            {signedInLabel}{' '}
                            <Link href={`${routes.groupMembersRoot}/${id}`}>
                                <a>{userName}</a>
                            </Link>
                        </p>
                        <div ref={commentButtonRef}>
                            <Form
                                csrfToken={csrfToken}
                                formConfig={commentFormConfig}
                                text={{
                                    submitButton: 'Submit reply',
                                }}
                                shouldClearOnSubmitSuccess={true}
                                validationFailAction={handleValidationFailure}
                                submitAction={handleCommentSubmit}
                                shouldRenderBackToTopIcon={renderBackToTopIcon}
                            />
                        </div>
                    </>
                )}
            </LayoutColumn>
        </>
    )
}

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = async (
    context: GetServerSidePropsContext
) =>
    await pipeSSRProps(
        context,
        {
            routeId: 'f9658510-6950-43c4-beea-4ddeca277a5f',
        },
        [withUser, withRoutes, withGroup, withTextContent],
        async (context: GetServerSidePropsContext) => {
            /**
             * Get context data
             */
            const props: Partial<Props> = selectPageProps(context)
            const user: User = selectUser(context)
            const groupId: string = selectParam(context, routeParams.GROUPID)
            const discussionId: string = selectParam(
                context,
                routeParams.DISCUSSIONID
            )
            const pagination: Pagination = selectPagination(context)
            const formData: ServerSideFormData = selectFormData(context)
            const requestMethod: requestMethods = selectRequestMethod(context)
            const csrfToken: string = selectCsrfToken(context)

            props.forms = {
                [formTypes.CREATE_DISCUSSION_COMMENT]: {},
                [formTypes.CREATE_DISCUSSION_COMMENT_REPLY]: {},
            }

            const commentForm: any =
                props.forms[formTypes.CREATE_DISCUSSION_COMMENT]
            const replyForm: any =
                props.forms[formTypes.CREATE_DISCUSSION_COMMENT_REPLY]

            props.discussionId = discussionId
            props.layoutId = layoutIds.GROUP
            props.tabId = groupTabIds.FORUM

            if (formData && requestMethod === requestMethods.POST) {
                const formId = formData.get('_form-id')
                const commentId: string = formData.get('_instance-id')
                const headers = getStandardServiceHeaders({
                    csrfToken,
                    accessToken: user.accessToken,
                })

                try {
                    if (formId === formTypes.CREATE_DISCUSSION_COMMENT) {
                        commentForm.initialValues = formData.getAll()

                        await postGroupDiscussionComment({
                            groupId,
                            discussionId,
                            user,
                            headers,
                            body: formData,
                        })
                    } else if (
                        formId === formTypes.CREATE_DISCUSSION_COMMENT_REPLY
                    ) {
                        replyForm.initialValues = formData.getAll()

                        await postGroupDiscussionCommentReply({
                            groupId,
                            discussionId,
                            commentId,
                            user,
                            headers,
                            body: formData,
                        })
                    }
                } catch (error) {
                    const validationErrors: FormErrors =
                        getServiceErrorDataValidationErrors(error)

                    if (validationErrors) {
                        if (formId === formTypes.CREATE_DISCUSSION_COMMENT) {
                            commentForm.errors = validationErrors
                        } else if (
                            formId === formTypes.CREATE_DISCUSSION_COMMENT_REPLY
                        ) {
                            replyForm.errors = validationErrors
                        }
                    } else {
                        return handleSSRErrorProps({ props, error })
                    }
                }
            }

            /**
             * Get data from services
             */
            try {
                const [groupDiscussion, groupDiscussionComments] =
                    await Promise.all([
                        getGroupDiscussion({
                            user,
                            groupId,
                            discussionId,
                        }),
                        getGroupDiscussionCommentsWithReplies({
                            user,
                            groupId,
                            discussionId,
                            pagination,
                        }),
                    ])

                props.discussion = groupDiscussion.data
                props.discussionCommentsList = groupDiscussionComments.data
                props.pagination = groupDiscussionComments.pagination
                props.pageTitle = `${props.entityText.title} - ${props.discussion.text.title}`
            } catch (error) {
                return handleSSRErrorProps({ props, error })
            }

            /**
             * Return data to page template
             */
            return handleSSRSuccessProps({ props, context })
        }
    )

/**
 * Export page template
 */
export default GroupDiscussionPage
