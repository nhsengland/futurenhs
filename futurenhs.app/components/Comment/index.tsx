import React from 'react';
import classNames from 'classnames';

import { UserMeta } from '@components/UserMeta';
import { RichText } from '@components/RichText';
import { Card } from '@components/Card';
import { Reply } from '@components/Reply';
import { Like } from '@components/Like';
import { Link } from '@components/Link';

import { Props } from './interfaces';

export const Comment: (props: Props) => JSX.Element = ({
    commentId,
    csrfToken,
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

    const generatedClasses: any = {
        wrapper: classNames('c-comment', className),
        userMeta: classNames('u-text-theme-7')
    };

    return (

        <Card className={generatedClasses.wrapper}>
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
                        validationFailAction={replyValidationFailAction}
                        submitAction={replySubmitAction}
                        text={{
                            reply: 'Reply'
                        }} 
                        className="u-flex-grow"/>
                }
            </footer>
            {children}
        </Card>

    );
}