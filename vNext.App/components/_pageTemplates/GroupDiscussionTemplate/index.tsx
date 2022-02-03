import { useMemo } from 'react';
import { useRouter } from 'next/router';

import { formTypes } from '@constants/forms';
import { dateTime } from '@helpers/formatters/dateTime';
import { initials } from '@helpers/formatters/initials';
import { routeParams } from '@constants/routes';
import { Link } from '@components/Link';
import { Accordion } from '@components/Accordion';
import { AriaLiveRegion } from '@components/AriaLiveRegion';
import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
import { RichText } from '@components/RichText';
import { Comment } from '@components/Comment';
import { LayoutColumn } from '@components/LayoutColumn';
import { ErrorBoundary } from '@components/ErrorBoundary';
import { BackLink } from '@components/BackLink';
import { UserMeta } from '@components/UserMeta';
import { Like } from '@components/Like';
import { FormWithErrorSummary } from '@components/FormWithErrorSummary';
import { getGroupDiscussion } from '@services/getGroupDiscussion';
import { getRouteToParam } from '@helpers/routing/getRouteToParam';

import { Props } from './interfaces';
import { DiscussionComment } from '@appTypes/discussion';

export const GroupDiscussionTemplate: (props: Props) => JSX.Element = ({
    groupId,
    user,
    contentText,
    entityText,
    image,
    actions,
    discussion,
    discussionComments,
    discussionCommentReplies,
    pagination,
    forms
}) => {

    const router = useRouter();

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

    const hasDiscussionComments: boolean = discussionComments?.length > 0;
    const creatorUserInitials: string = initials({ value: createdBy?.text?.userName });
    const creatorUserName: string = createdBy?.text?.userName;
    const creatorUserId: string = createdBy?.id;
    const createdDate: string = dateTime({ value: created });
    const lastCommentUserName: string = modifiedBy?.text?.userName;
    const lastCommentDate: string = dateTime({ value: modified });
    const createCommentfields = forms?.[formTypes.CREATE_DISCUSSION_COMMENT]?.steps[0]?.fields;

    const renderReplies = ({ commentId }) => {

        const replyData: Array<DiscussionComment> = discussionCommentReplies[commentId]?.data ?? [];

        return replyData.map(({
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

                <li key={commentId} className="u-pt-6 u-pb-6">
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
                                {discussionComments?.map(({
                                    commentId,
                                    created,
                                    createdBy,
                                    text,
                                    likeCount,
                                    isLiked
                                }, index) => {

                                    const commenterUserInitials: string = initials({ value: createdBy?.text?.userName });
                                    const commenterUserName: string = createdBy?.text?.userName;
                                    const commenterUserId: string = createdBy?.id;
                                    const commentCreatedDate: string = dateTime({ value: created });
                                    const replies: Array<JSX.Element> = renderReplies({ commentId });
                                    const hasReply: boolean = replies.length > 0;
                                    const hasReplies: boolean = replies.length > 1;
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
                                                        <li>
                                                            {replies[0]}
                                                        </li>
                                                    </ul>
                                                }
                                                {hasReplies &&
                                                    <Accordion
                                                        id={additionalRepliesAccordionId}
                                                        toggleChildren={<span>Show more replies</span>}
                                                        toggleClassName="c-comment_replies-toggle u-text-bold">
                                                        <ul className="u-list-none u-p-0">
                                                            {replies.splice(1)}
                                                        </ul>
                                                    </Accordion>
                                                }
                                            </Comment>
                                        </li>

                                    )

                                })}
                            </ul>
                        }
                    </AriaLiveRegion>
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
