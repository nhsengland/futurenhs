import { useState, useCallback, useRef } from 'react';
import { useRouter } from 'next/router';

import { postGroupDiscussionComment } from '@services/postGroupDiscussionComment';
import { postGroupDiscussionCommentReply } from '@services/postGroupDiscussionCommentReply';
import { selectFormErrors } from '@selectors/forms';
import { actions as actionsConstants } from '@constants/actions';
import { formTypes } from '@constants/forms';
import { dateTime } from '@helpers/formatters/dateTime';
import { initials } from '@helpers/formatters/initials';
import { routeParams } from '@constants/routes';
import { Link } from '@components/Link';
import { Accordion } from '@components/Accordion';
import { DynamicListContainer } from '@components/DynamicListContainer';
import { RichText } from '@components/RichText';
import { SVGIcon } from '@components/SVGIcon';
import { Comment } from '@components/Comment';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { Form } from '@components/Form';
import { ErrorSummary } from '@components/ErrorSummary';
import { ErrorBoundary } from '@components/ErrorBoundary';
import { PaginationWithStatus } from '@components/PaginationWithStatus';
import { BackLink } from '@components/BackLink';
import { UserMeta } from '@components/UserMeta';
import { getGroupDiscussionCommentsWithReplies } from '@services/getGroupDiscussionCommentsWithReplies';
import { getRouteToParam } from '@helpers/routing/getRouteToParam';
import { FormErrors } from '@appTypes/form';

import { Props } from './interfaces';

/**
 * Group discussion template
 */
export const GroupDiscussionTemplate: (props: Props) => JSX.Element = ({
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
        getGroupDiscussionCommentsWithReplies: getGroupDiscussionCommentsWithReplies,
        postGroupDiscussionComment: postGroupDiscussionComment,
        postGroupDiscussionCommentReply: postGroupDiscussionCommentReply
    }
}) => {

    const router = useRouter();
    const errorSummaryRef: any = useRef();

    const [errors, setErrors] = useState(Object.assign({}, selectFormErrors(forms, formTypes.CREATE_DISCUSSION_COMMENT), selectFormErrors(forms, formTypes.CREATE_DISCUSSION_COMMENT_REPLY)));
    const [dynamicDiscussionCommentsList, setDiscussionsList] = useState(discussionCommentsList);
    const [dynamicPagination, setPagination] = useState(pagination);
    const [isClient, setIsClient] = useState(false);

    const backLinkHref: string = getRouteToParam({
        router: router,
        paramName: routeParams.DISCUSSIONID
    });

    const {
        createdByLabel,
        lastCommentLabel,
        totalRecordsLabel,
        viewCountLabel,
        moreRepliesLabel,
        secondaryHeading,
        signedInLabel
    } = contentText ?? {};

    const fewerRepliesLabel: string = 'Fewer replies';

    const { id, text } = user ?? {};
    const { userName } = text ?? {};
    const { text: discussionText,
        created,
        createdBy,
        responseCount,
        modified,
        modifiedBy,
        viewCount } = discussion ?? {};
    const { title, body } = discussionText ?? {};
    const { totalRecords } = dynamicPagination ?? {};

    const shouldRenderCommentAndReplyForms: boolean = actions.includes(actionsConstants.GROUPS_COMMENTS_ADD);
    const shouldEnableLoadMore: boolean = true;
    const hasDiscussionComments: boolean = dynamicDiscussionCommentsList?.length > 0;
    const creatorUserInitials: string = initials({ value: createdBy?.text?.userName });
    const creatorUserName: string = createdBy?.text?.userName;
    const creatorUserId: string = createdBy?.id;
    const createdDate: string = dateTime({ value: created });
    const lastCommentUserName: string = modifiedBy?.text?.userName;
    const lastCommentDate: string = dateTime({ value: modified });
    const createCommentfields = forms?.[formTypes.CREATE_DISCUSSION_COMMENT]?.steps[0]?.fields;

    /**
     * Handle likes on comments
     */
    const handleLike = useCallback((commentId: string, isLiked: boolean): any => {



    }, []);

    /**
     * Handle client-side validation failure in forms
     */
    const handleValidationFailure = (errors: FormErrors): void => {

        setErrors(errors);
        errorSummaryRef?.current?.focus?.();

    };

    /**
     * Handle client-side comment submission
     */
    const handleCommentSubmit = async (formData: FormData): Promise<FormErrors> => {

        return new Promise((resolve) => {

            services.postGroupDiscussionComment({ groupId, discussionId, user, csrfToken, body: formData }).then(() => {

                const targetPageNumber: number = Math.ceil((Number(dynamicPagination.totalRecords) + 1) / dynamicPagination.pageSize);

                setErrors({});
                handleGetPage({
                    pageNumber: targetPageNumber,
                    pageSize: dynamicPagination.pageSize
                });

                resolve({});

            })
                .catch((error) => {

                    const errors: FormErrors = {
                        [error.data.status]: error.data.statusText
                    };

                    setErrors(errors);
                    resolve(errors);

                });

        });

    };

    /**
     * Handle client-side comment reply submission
     */
    const handleCommentReplySubmit = async (formData: FormData): Promise<FormErrors> => {

        return new Promise((resolve) => {

            const commentId: any = formData.get('_instance-id');

            services.postGroupDiscussionCommentReply({ groupId, discussionId, user, csrfToken, commentId, body: formData }).then(() => {

                setErrors({});
                handleGetPage(dynamicPagination as any);

                resolve({});

            })
                .catch((error) => {

                    const errors: FormErrors = {
                        [error.data.status]: error.data.statusText
                    };

                    setErrors(errors);
                    resolve(errors);

                });

        });

    };

    /**
     * Handle client-side pagination
     */
    const handleGetPage = async ({
        pageNumber: requestedPageNumber,
        pageSize: requestedPageSize
    }) => {

        try {

            const { data: newComments, pagination } = await services.getGroupDiscussionCommentsWithReplies({
                user, groupId, discussionId, pagination: {
                    pageNumber: requestedPageNumber,
                    pageSize: requestedPageSize
                }
            });

            setDiscussionsList(newComments);
            setPagination(pagination);

        } catch (error) {

            console.log(error);

        }

    };

    /**
     * Render replies to individual comments
     */
    const renderReplies = ({ replies }) => {

        return replies?.map(({
            commentId,
            created,
            createdBy,
            text,
            likeCount,
            isLiked }) => {

            const replyingUserInitials: string = initials({ value: createdBy?.text?.userName });
            const replyingUserName: string = createdBy?.text?.userName;
            const replyingUserId: string = createdBy?.id;
            const replyCreatedDate: string = dateTime({ value: created });

            const { body } = text ?? {};

            return (

                <li key={commentId} className="c-comment_reply-container u-m-0 u-py-6">
                    <Comment
                        csrfToken={csrfToken}
                        initialErrors={errors}
                        id={`reply-${commentId}`}
                        commentId={commentId}
                        text={{
                            userName: replyingUserName,
                            initials: replyingUserInitials,
                            body: body
                        }}
                        userProfileLink={`${routes.groupMembersRoot}/${replyingUserId}`}
                        date={replyCreatedDate}
                        shouldEnableReplies={shouldRenderCommentAndReplyForms}
                        replyValidationFailAction={handleValidationFailure}
                        replySubmitAction={handleCommentReplySubmit}
                        shouldEnableLikes={shouldRenderCommentAndReplyForms}
                        likeCount={likeCount}
                        isLiked={isLiked}
                        likeAction={handleLike}
                        className="c-comment--reply u-border-l-theme-8" />
                </li>

            )

        })
    };

    /**
     * Render
     */
    return (

        <>
            <LayoutColumn className="c-page-body">
                {shouldRenderCommentAndReplyForms &&
                    <ErrorSummary
                        ref={errorSummaryRef}
                        errors={errors}
                        text={{
                            body: 'There is a problem'
                        }}
                        className="u-mb-10" />
                }
                <BackLink
                    href={backLinkHref}
                    text={{
                        link: "Back to discussions"
                    }} />
                <h2 className="nhsuk-heading-xl">{title}</h2>
                {body &&
                    <RichText bodyHtml={body} wrapperElementType="div" className="u-mb-8" />
                }
                <LayoutColumnContainer>
                    <LayoutColumn tablet={8}>
                        <UserMeta
                            image={null}
                            text={{
                                initials: creatorUserInitials
                            }}
                            className="u-m-0 u-text-theme-7">
                            <span className="u-text-bold u-block">{createdByLabel} <Link href={`${routes.groupMembersRoot}/${creatorUserId}`}><a>{creatorUserName}</a></Link> {createdDate}</span>
                            {(responseCount > 0 && lastCommentUserName) &&
                                <span className="u-block u-mt-1">{lastCommentLabel} <Link href={`${routes.groupMembersRoot}/${creatorUserId}`}><a>{lastCommentUserName}</a></Link> {lastCommentDate}</span>
                            }
                        </UserMeta>
                    </LayoutColumn>
                    <LayoutColumn tablet={4} className="u-self-end tablet:u-text-right u-text-theme-7 u-text-bold u-mt-4">
                        {totalRecords > 0 &&
                            <span className="u-mr-5"><SVGIcon name="icon-comments" className="u-h-5 u-w-5 u-fill-theme-8 u-mr-1 u-align-middle" />{totalRecordsLabel}: {totalRecords}</span>
                        }
                        {viewCount > 0 &&
                            <><SVGIcon name="icon-view" className="u-h-5 u-w-5 u-fill-theme-8 u-mr-1 u-align-middle" />{viewCountLabel}: {viewCount}</>
                        }
                    </LayoutColumn>
                </LayoutColumnContainer>
                <hr />
                {totalRecords > 0 &&
                    <p className="u-hidden tablet:u-block u-text-lead u-text-bold">
                        {`Comments: ${totalRecords}`}
                    </p>
                }
                <ErrorBoundary boundaryId="group-discussion-comments">
                    {hasDiscussionComments &&
                        <DynamicListContainer
                            containerElementType="ul"
                            shouldFocusLatest={shouldEnableLoadMore}
                            className="u-list-none u-p-0">
                            {dynamicDiscussionCommentsList?.map(({
                                commentId,
                                created,
                                createdBy,
                                text,
                                likeCount,
                                isLiked,
                                replies
                            }, index) => {

                                const commenterUserInitials: string = initials({ value: createdBy?.text?.userName });
                                const commenterUserName: string = createdBy?.text?.userName;
                                const commenterUserId: string = createdBy?.id;
                                const commentCreatedDate: string = dateTime({ value: created });
                                const hasReply: boolean = replies?.length > 0;
                                const hasReplies: boolean = replies?.length > 1;
                                const repliesComponents: Array<JSX.Element> = renderReplies({ replies });
                                const additionalRepliesAccordionId: string = `${commentId}-replies`;

                                const { body } = text ?? {};

                                return (

                                    <li key={index}>
                                        <Comment
                                            commentId={commentId}
                                            csrfToken={csrfToken}
                                            initialErrors={errors}
                                            text={{
                                                userName: commenterUserName,
                                                initials: commenterUserInitials,
                                                body: body
                                            }}
                                            userProfileLink={`${routes.groupMembersRoot}/${commenterUserId}`}
                                            date={commentCreatedDate}
                                            shouldEnableReplies={shouldRenderCommentAndReplyForms}
                                            replyValidationFailAction={handleValidationFailure}
                                            replySubmitAction={handleCommentReplySubmit}
                                            shouldEnableLikes={shouldRenderCommentAndReplyForms}
                                            likeCount={likeCount}
                                            isLiked={isLiked}
                                            likeAction={handleLike}
                                            className="u-border-l-theme-8">
                                            {hasReply &&
                                                <ul className="u-list-none c-comment_replies-list u-p-0">
                                                    {repliesComponents[0]}
                                                </ul>
                                            }
                                            {hasReplies &&
                                                <Accordion
                                                    id={additionalRepliesAccordionId}
                                                    toggleOpenChildren={<span>{fewerRepliesLabel}</span>}
                                                    toggleClosedChildren={<span>{moreRepliesLabel}</span>}
                                                    toggleClassName="c-comment_replies-toggle u-text-bold">
                                                    <ul className="u-list-none u-m-0 u-p-0">
                                                        {repliesComponents.splice(1)}
                                                    </ul>
                                                </Accordion>
                                            }
                                        </Comment>
                                    </li>

                                )

                            })}
                        </DynamicListContainer>
                    }
                    <PaginationWithStatus
                        id="discussion-list-pagination"
                        shouldEnableLoadMore={false}
                        getPageAction={handleGetPage}
                        {...dynamicPagination}
                        className="u-mb-10" />
                </ErrorBoundary>
                {shouldRenderCommentAndReplyForms &&
                    <>
                        <h3 className="nhsuk-heading-l">{secondaryHeading}</h3>
                        <p className="u-text-bold">{signedInLabel} <Link href={`${routes.groupMembersRoot}/${id}`}><a>{userName}</a></Link></p>
                        <Form
                            csrfToken={csrfToken}
                            formId={formTypes.CREATE_DISCUSSION_COMMENT}
                            fields={createCommentfields}
                            initialErrors={errors}
                            text={{
                                submitButton: 'Add Comment'
                            }}
                            validationFailAction={handleValidationFailure}
                            submitAction={handleCommentSubmit} />
                    </>
                }
            </LayoutColumn>
        </>

    )

}
