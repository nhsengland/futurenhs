import { useState } from 'react';
import { useRouter } from 'next/router';

import { Link } from '@components/Link';
import { AriaLiveRegion } from '@components/AriaLiveRegion';
import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
import { LayoutColumn } from '@components/LayoutColumn';
import { PaginationWithStatus } from '@components/PaginationWithStatus';
import { Card } from '@components/Card';
import { SVGIcon } from '@components/SVGIcon';
import { getDiscussions } from '@services/getDiscussions';

import { Props } from './interfaces';

export const GroupForumTemplate: (props: Props) => JSX.Element = ({
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

        const { data: additionalDiscussions, pagination } = await getDiscussions({
            user: user,
            pagination: {
                pageNumber: requestedPageNumber,
                pageSize: requestedPageSize
            }
        });

        setDiscussionsList([...dynamicDiscussionsList, ...additionalDiscussions]);
        setPagination(pagination);

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
                            image, 
                            content, 
                            groupId, 
                            totalDiscussionCount, 
                            totalMemberCount 
                        }, index) => {

                            const { mainHeading, strapLine } = content ?? {};

                            return (

                                <Card key={index} image={image} className="u-border-bottom-theme-8">
                                    <h3 className="c-card_heading">
                                        <Link href={`/groups/${groupId}`}>
                                            <a>{mainHeading}</a>
                                        </Link>        
                                    </h3>
                                    <p className="c-card_content u-text-theme-7 o-truncated-text-lines-2">
                                        {strapLine}
                                    </p>
                                    <div className="c-card_footer u-text-theme-7">
                                        <p className="c-card_footer-item">
                                            <SVGIcon name="icon-account" className="c-card_footer-icon u-fill-theme-8" />
                                            <span>{`${totalMemberCount} Members`}</span>
                                        </p>
                                        <p className="c-card_footer-item">
                                            <SVGIcon name="icon-discussion" className="c-card_footer-icon u-fill-theme-8" />
                                            <span>{`${totalDiscussionCount} Discussions`}</span>
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
