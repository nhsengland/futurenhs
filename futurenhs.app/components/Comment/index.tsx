import React from 'react';
import classNames from 'classnames';

import { scrollToComponentAndSetFocus } from '@helpers/dom';
import { truncate } from '@helpers/formatters/truncate';
import { UserMeta } from '@components/UserMeta';
import { RichText } from '@components/RichText';
import { Card } from '@components/Card';
import { Reply } from '@components/Reply';
import { Like } from '@components/Like';
import { Link } from '@components/Link';

import { Props } from './interfaces';

export const Comment: (props: Props) => JSX.Element = ({
    id,
    commentId,
    originComment,
    csrfToken,
    initialErrors,
    image,
    text,
    userProfileLink,
    date,
    shouldEnableReplies,
    replyValidationFailAction,
    replySubmitAction,
    shouldEnableLikes,
    likeAction,
    likeCount,
    isLiked,
    children,
    className
}) => {

    const { userName, initials, body } = text;

    const parentCommentUserName: string = originComment?.createdBy?.text?.userName;
    const parentCommentTeaserText: string = truncate({
        value: originComment?.text?.body,
        limit: 15
    });

    const handleOriginLinkClick = (event: any): void => {

        event.preventDefault();

        const targetId: string = event.target.href.split('#')[1];
        const targetElement: HTMLElement = document.getElementById(targetId);

        scrollToComponentAndSetFocus(targetElement, false, 60);

    }

    const generatedClasses: any = {
        wrapper: classNames('c-comment', 'focus:u-outline-none', className),
        userMeta: classNames('u-text-theme-7')
    };

    return (

        <Card id={id} className={generatedClasses.wrapper}>
            <header>
                <UserMeta
                    image={image}
                    text={{
                        initials: initials
                    }}
                    className={generatedClasses.userMeta}>
                        <span className="u-text-bold u-block">
                            <Link href={userProfileLink}>
                                <a>{userName}</a>
                            </Link>
                        </span>
                        <span className="u-block u-text-bold">{date}</span>
                </UserMeta>
            </header>
            {originComment &&
                <p className="nhsuk-body-s u-mb-5 u-text-theme-6 u-text-bold">
                    {`In response to ${parentCommentUserName}`} "<a href={`#comment-${originComment.commentId}`} onClick={handleOriginLinkClick}>{parentCommentTeaserText}</a>"
                </p>
            }
            <RichText bodyHtml={body} wrapperElementType="div" className="u-mb-6" />
            <footer className="u-flex u-flex-col tablet:u-flex-row u-items-start">
                <Like
                    targetId={commentId}
                    likeCount={likeCount}
                    isLiked={isLiked}
                    shouldEnable={shouldEnableLikes}
                    likeAction={likeAction}
                    text={{
                        countSingular: 'like',
                        countPlural: 'likes',
                        like: 'like',
                        removeLike: 'Remove like'
                    }}
                    className="u-mr-6" />
                {shouldEnableReplies &&
                    <Reply
                        targetId={commentId}
                        csrfToken={csrfToken}
                        initialErrors={initialErrors}
                        validationFailAction={replyValidationFailAction}
                        submitAction={replySubmitAction}
                        text={{
                            reply: 'Reply'
                        }} 
                        className="u-w-full tablet:u-flex-grow tablet:u-w-auto"/>
                }
            </footer>
            {children}
        </Card>

    );
}