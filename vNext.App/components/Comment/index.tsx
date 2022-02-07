import React from 'react';
import classNames from 'classnames';

import { UserMeta } from '@components/UserMeta';
import { RichText } from '@components/RichText';
import { Like } from '@components/Like';
import { Link } from '@components/Link';

import { Props } from './interfaces';

export const Comment: (props: Props) => JSX.Element = ({
    commentId,
    image,
    text,
    userProfileLink,
    date,
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

        <div className={generatedClasses.wrapper}>
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
            <RichText bodyHtml={body} className="u-mb-6" />
            <footer>
                <Like
                    id={commentId}
                    likeCount={likeCount}
                    isLiked={isLiked}
                    shouldEnable={true}
                    likeAction={console.log}
                    text={{
                        countSingular: 'like',
                        countPlural: 'likes',
                        like: 'like',
                        removeLike: 'Remove like'
                    }} />
            </footer>
            {children}
        </div>

    );
}