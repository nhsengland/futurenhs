import { useState } from 'react';
import Head from 'next/head';
import { useRouter } from 'next/router';

import { defaultGroupLogos } from '@constants/icons';
import { Link } from '@components/Link';
import { AriaLiveRegion } from '@components/AriaLiveRegion';
import { StandardLayout } from '@components/_pageLayouts/StandardLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { PageHeader } from '@components/PageHeader';
import { PageBody } from '@components/PageBody';
import { Card } from '@components/Card';
import { SVGIcon } from '@components/SVGIcon';
import { PaginationWithStatus } from '@components/PaginationWithStatus';
import { getGroups } from '@services/getGroups';

import { Props } from './interfaces';

/**
 * Group listing template
 */
export const GroupListingTemplate: (props: Props) => JSX.Element = ({
    user,
    text,
    groupsList,
    pagination,
    errors
}) => {

    const { pathname } = useRouter();
    const [dynamicGroupsList, setGroupsList] = useState(groupsList);
    const [dynamicPagination, setPagination] = useState(pagination);

    const { title, 
            metaDescription, 
            mainHeading,
            intro,
            secondaryHeading,
            navMenuTitle } = text ?? {};

    const handleGetPage = async ({ 
        pageNumber: requestedPageNumber, 
        pageSize: requestedPageSize 
    }) => {

        const { data: additionalGroups, pagination } = await getGroups({
            user: user,
            pagination: {
                pageNumber: requestedPageNumber,
                pageSize: requestedPageSize
            }
        });

        setGroupsList([...dynamicGroupsList, ...additionalGroups]);
        setPagination(pagination);

    };

    return (

        <StandardLayout user={user} className="u-bg-theme-3">
            <Head>
                <title>{title}</title>
                <meta name="description" content={metaDescription} />
            </Head>
            <LayoutColumnContainer>
                <PageHeader 
                    id="my-groups"
                    text={{
                        mainHeading: mainHeading, 
                        description: intro,
                        navMenuTitle: navMenuTitle
                    }} 
                    navMenuList={[
                        {
                            url: '/groups',
                            text: 'My groups',
                            isActive: pathname === '/groups'
                        },
                        {
                            url: '/groups/discover',
                            text: 'Discover new groups',
                            isActive: pathname === '/groups/discover'
                        }
                    ]}
                    className="u-bg-theme-14" />
                <PageBody>
                    <LayoutColumn tablet={8}>
                        <h2>{secondaryHeading}</h2>
                        {intro &&
                            <p className="u-text-lead u-text-theme-7 u-mb-4">{intro}</p>
                        }
                        <AriaLiveRegion>
                            <ul className="u-list-none u-p-0">
                                {dynamicGroupsList?.map?.(({ 
                                    image, 
                                    text, 
                                    groupId, 
                                    totalDiscussionCount, 
                                    totalMemberCount 
                                }, index) => {

                                    const { mainHeading, strapLine } = text ?? {};
                                    
                                    const imageToUse = image ? image : defaultGroupLogos.large;

                                    return (

                                        <li key={index}>
                                            <Card image={imageToUse} className="u-border-bottom-theme-8 u-mb-4">
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
                                        </li>

                                    )

                                })}
                            </ul>
                        </AriaLiveRegion>
                        <PaginationWithStatus 
                            id="group-list-pagination"
                            shouldEnableLoadMore={true}
                            getPageAction={handleGetPage}
                            {...dynamicPagination} />
                    </LayoutColumn>
                </PageBody>
            </LayoutColumnContainer>
        </StandardLayout>

    )

}
