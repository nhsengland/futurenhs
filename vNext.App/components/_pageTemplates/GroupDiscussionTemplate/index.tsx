import { useState } from 'react';
import { useRouter } from 'next/router';

import { formTypes } from '@constants/forms';
import { dateTime } from '@helpers/formatters/dateTime';
import { initials } from '@helpers/formatters/initials';
import { routeParams } from '@constants/routes';
import { Link } from '@components/Link';
import { Accordion } from '@components/Accordion';
import { AriaLiveRegion } from '@components/AriaLiveRegion';
import { DynamicListContainer } from '@components/DynamicListContainer';
import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
import { RichText } from '@components/RichText';
import { Comment } from '@components/Comment';
import { LayoutColumn } from '@components/LayoutColumn';
import { ErrorBoundary } from '@components/ErrorBoundary';
import { PaginationWithStatus } from '@components/PaginationWithStatus';
import { BackLink } from '@components/BackLink';
import { UserMeta } from '@components/UserMeta';
import { FormWithErrorSummary } from '@components/FormWithErrorSummary';
import { getGroupDiscussionComments } from '@services/getGroupDiscussionComments';
import { getRouteToParam } from '@helpers/routing/getRouteToParam';

import { Props } from './interfaces';

export const GroupDiscussionTemplate: (props: Props) => JSX.Element = ({
    groupId,
    discussionId,
    user,
    contentText,
    entityText,
    image,
    actions,
    discussion,
    discussionCommentsList,
    pagination,
    forms
}) => {

    const router = useRouter();

    const [dynamicDiscussionCommentsList, setDiscussionsList] = useState(discussionCommentsList);
    const [dynamicPagination, setPagination] = useState(pagination);

    const backLinkHref: string = getRouteToParam({
        router: router,
        paramName: routeParams.DISCUSSIONID
    });

    const groupBasePath: string = getRouteToParam({
        router: router,
        paramName: routeParams.GROUPID,
        shouldIncludeParam: true
    });

    const { id, text } = user ?? {};
    const { userName } = text ?? {};
    const { text: discussionText, created, createdBy, responseCount, modified, modifiedBy } = discussion ?? {};
    const { title, body } = discussionText ?? {};
    const { totalRecords } = pagination ?? {};

    const hasDiscussionComments: boolean = dynamicDiscussionCommentsList?.length > 0;
    const creatorUserInitials: string = initials({ value: createdBy?.text?.userName });
    const creatorUserName: string = createdBy?.text?.userName;
    const creatorUserId: string = createdBy?.id;
    const createdDate: string = dateTime({ value: created });
    const lastCommentUserName: string = modifiedBy?.text?.userName;
    const lastCommentDate: string = dateTime({ value: modified });
    const createCommentfields = forms?.[formTypes.CREATE_DISCUSSION_COMMENT]?.steps[0]?.fields;

    const handleGetPage = async ({ 
        pageNumber: requestedPageNumber, 
        pageSize: requestedPageSize 
    }) => {

        try {

            const { data: additionalComments, pagination, errors } = await getGroupDiscussionComments({
                user: user,
                groupId: groupId,
                discussionId: discussionId,
                pagination: {
                    pageNumber: requestedPageNumber,
                    pageSize: requestedPageSize
                }
            });

            if(!errors || !Object.keys(errors).length){

                setDiscussionsList([...dynamicDiscussionCommentsList, ...additionalComments]);
                setPagination(pagination);

            }

        } catch(error){

            console.log(error);

        }

    };

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

                <li key={commentId} className="u-py-6">
                    <Comment
                        commentId={commentId}
                        text={{
                            userName: replyingUserName,
                            initials: replyingUserInitials,
                            body: body
                        }}
                        userProfileLink={`${groupBasePath}/members/${replyingUserId}`}
                        date={replyCreatedDate}
                        likeCount={likeCount}
                        isLiked={isLiked}
                        className="c-comment--reply" />
                </li>

            )

        })
    };

    return (

        <GroupLayout
            id="forum"
            user={user}
            actions={actions}
            text={entityText}
            image={image}
            className="u-bg-theme-3">
            <LayoutColumn className="c-page-body">
                <BackLink
                    href={backLinkHref}
                    text={{
                        link: "Back to discussions"
                    }} />
                <h2 className="u-text-5xl">{title}</h2>
                {body &&
                    <RichText bodyHtml={body} className="u-mb-8" />
                }
                <UserMeta
                    image={null}
                    text={{
                        initials: creatorUserInitials
                    }}
                    className="c-card_content u-text-theme-7 o-truncated-text-lines-2">
                    <span className="u-text-bold u-block">Created by <Link href={`${groupBasePath}/members/${creatorUserId}`}><a>{creatorUserName}</a></Link> {createdDate}</span>
                    {(responseCount > 0 && lastCommentUserName) &&
                        <span className="u-block u-mt-1">Last comment by <Link href={`${groupBasePath}/members/${creatorUserId}`}><a>{lastCommentUserName}</a></Link> {lastCommentDate}</span>
                    }
                </UserMeta>
                <hr />
                {totalRecords > 0 &&
                    <p className="u-text-lead u-text-bold">
                        {`${totalRecords} comments`}
                    </p>
                }
                <ErrorBoundary boundaryId="group-discussion-comments">
                    <AriaLiveRegion>
                        {hasDiscussionComments &&
                            <ul className="u-list-none u-p-0">
                                <DynamicListContainer>
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
                                                    text={{
                                                        userName: commenterUserName,
                                                        initials: commenterUserInitials,
                                                        body: body
                                                    }}
                                                    userProfileLink={`${groupBasePath}/members/${commenterUserId}`}
                                                    date={commentCreatedDate}
                                                    likeCount={likeCount}
                                                    isLiked={isLiked}>
                                                    {hasReply &&
                                                        <ul className="u-list-none u-p-0">
                                                            {repliesComponents[0]}
                                                        </ul>
                                                    }
                                                    {hasReplies &&
                                                        <Accordion
                                                            id={additionalRepliesAccordionId}
                                                            toggleChildren={<span>Show more replies</span>}
                                                            toggleClassName="c-comment_replies-toggle u-text-bold">
                                                            <ul className="u-list-none u-p-0">
                                                                {repliesComponents.splice(1)}
                                                            </ul>
                                                        </Accordion>
                                                    }
                                                </Comment>
                                            </li>

                                        )

                                    })}
                                </DynamicListContainer>
                            </ul>
                        }
                    </AriaLiveRegion>
                    <PaginationWithStatus 
                        id="discussion-list-pagination"
                        shouldEnableLoadMore={true}
                        getPageAction={handleGetPage}
                        {...dynamicPagination} />
                </ErrorBoundary>
                {user &&
                    <>
                        <h3 className="u-text-3xl">Join in the conversation</h3>
                        <p className="u-text-bold">You're signed in <Link href={`${groupBasePath}/members/${id}`}><a>{userName}</a></Link></p>
                        <FormWithErrorSummary
                            csrfToken=""
                            fields={createCommentfields}
                            errors={[]}
                            text={{
                                errorSummary: {
                                    body: ''
                                },
                                form: {
                                    submitButton: 'Add comment'
                                }
                            }}
                            submitAction={() => { }} />
                    </>
                }
            </LayoutColumn>
        </GroupLayout>

    )

}
