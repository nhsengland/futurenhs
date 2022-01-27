import { useState } from 'react';
import { useRouter } from 'next/router';

import { Link } from '@components/Link';
import { Avatar } from '@components/Avatar';
import { AriaLiveRegion } from '@components/AriaLiveRegion';
import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
import { LayoutColumn } from '@components/LayoutColumn';
import { PaginationWithStatus } from '@components/PaginationWithStatus';
import { Card } from '@components/Card';
import { SVGIcon } from '@components/SVGIcon';
import { getGroupDiscussions } from '@services/getGroupDiscussions';

import { Props } from './interfaces';

export const GroupForumTemplate: (props: Props) => JSX.Element = ({
    groupId,
    user,
    text,
    image,
    pagination,
    actions,
    discussionsList
}) => {

    const { asPath } = useRouter();

    const [dynamicDiscussionsList, setDiscussionsList] = useState(discussionsList);
    const [dynamicPagination, setPagination] = useState(pagination);

    const handleGetPage = async ({ 
        pageNumber: requestedPageNumber, 
        pageSize: requestedPageSize 
    }) => {

        try {

            const { data: additionalDiscussions, pagination, errors } = await getGroupDiscussions({
                user: user,
                groupId: groupId,
                pagination: {
                    pageNumber: requestedPageNumber,
                    pageSize: requestedPageSize
                }
            });

            if(!errors || !Object.keys(errors).length){

                setDiscussionsList([...dynamicDiscussionsList, ...additionalDiscussions]);
                setPagination(pagination);

            }

        } catch(error){

            console.log(error);

        }

    };

    return (

        <GroupLayout 
            id="forum"
            user={user}
            actions={actions}
            text={text}
            image={image} 
            className="u-bg-theme-3">
                <LayoutColumn tablet={8} className="c-page-body">
                    <h2>Latest Discussions</h2>
                    <AriaLiveRegion>
                        {dynamicDiscussionsList?.map?.(({ 
                            text, 
                            discussionId, 
                            totalViewCount, 
                            totalCommentCount 
                        }, index) => {

                            const { title } = text ?? {};

                            return (

                                <Card key={index} className="u-border-bottom-theme-10 u-mb-4">
                                    <h3 className="c-card_heading desktop:u-mb-4">
                                        <Link href={`${asPath}/${discussionId}`}>
                                            <a>{title}</a>
                                        </Link>        
                                    </h3>
                                    <p className="c-card_content u-text-theme-7 o-truncated-text-lines-2">
                                        <Avatar image={null} initials="RI" className="u-block u-h-12 u-w-12" />
                                    </p>
                                    <div className="c-card_footer u-text-theme-0">
                                        <p className="c-card_footer-item">
                                            <SVGIcon name="icon-comments" className="c-card_footer-icon u-fill-theme-0" />
                                            <span>{`${totalCommentCount} Comments`}</span>
                                        </p>
                                        <p className="c-card_footer-item">
                                            <SVGIcon name="icon-view" className="c-card_footer-icon u-fill-theme-0" />
                                            <span>{`${totalViewCount} Views`}</span>
                                        </p>
                                    </div>
                                </Card>

                            )

                        })}
                    </AriaLiveRegion>
                    <PaginationWithStatus 
                        id="group-list-pagination"
                        shouldEnableLoadMore={true}
                        getPageAction={handleGetPage}
                        {...dynamicPagination} />                
                </LayoutColumn>
                <LayoutColumn tablet={4} className="u-px-4 u-py-10">
                    <Link href={`${asPath}/create`}>
                        <a className="c-button u-w-full">New discussion</a>
                    </Link>
                </LayoutColumn>
        </GroupLayout>

    )

}
