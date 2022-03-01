import { useState } from 'react';
import Head from 'next/head';
import classNames from 'classnames';
import { useRouter } from 'next/router';

import { selectThemeAccentId } from '@selectors/themes';
import { defaultGroupLogos } from '@constants/icons';
import { themes } from '@constants/themes';
import { Link } from '@components/Link';
import { AriaLiveRegion } from '@components/AriaLiveRegion';
import { DynamicListContainer } from '@components/DynamicListContainer';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { GroupPageHeader } from '@components/GroupPageHeader';
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
    contentText,
    isGroupMember,
    groupsList,
    pagination,
}) => {

    const { pathname } = useRouter();

    const [dynamicGroupsList, setGroupsList] = useState(groupsList);
    const [dynamicPagination, setPagination] = useState(pagination);

    const shouldEnableLoadMore: boolean = true;

    const { title,
        metaDescription,
        mainHeading,
        intro,
        secondaryHeading,
        navMenuTitle } = contentText ?? {};

    /**
     * Handle client-side pagination
     */
    const handleGetPage = async ({
        pageNumber: requestedPageNumber,
        pageSize: requestedPageSize
    }) => {

        const { data: additionalGroups, pagination } = await getGroups({
            user: user,
            isMember: isGroupMember,
            pagination: {
                pageNumber: requestedPageNumber,
                pageSize: requestedPageSize
            }
        });

        setGroupsList([...dynamicGroupsList, ...additionalGroups]);
        setPagination(pagination);

    };

    /**
     * Render
     */
    return (

        <>
            <Head>
                <title>{title}</title>
                <meta name="description" content={metaDescription} />
            </Head>
            <LayoutColumnContainer>
                <GroupPageHeader
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
                    <LayoutColumn desktop={8}>
                        <h2 className="nhsuk-heading-l">{secondaryHeading}</h2>
                        {intro &&
                            <p className="u-text-lead u-text-theme-7 u-mb-4">{intro}</p>
                        }
                        <AriaLiveRegion>
                            <DynamicListContainer
                                containerElementType="ul"
                                shouldFocusLatest={shouldEnableLoadMore}
                                className="u-list-none u-p-0">
                                {dynamicGroupsList?.map?.(({
                                    image,
                                    text,
                                    groupId,
                                    themeId,
                                    totalDiscussionCount,
                                    totalMemberCount
                                }, index) => {

                                    const { mainHeading, strapLine } = text ?? {};

                                    const imageToUse = image ? image : defaultGroupLogos.large;
                                    const cardLinkHref: string = `/groups/${groupId}`;
                                    const themeAccentId: number = selectThemeAccentId(themes, themeId);

                                    const generatedClasses = {
                                        wrapper: classNames('u-mb-4', `u-border-b-theme-${themeAccentId}`, `hover:u-border-b-theme-${themeAccentId}-darker`)
                                    }

                                    return (

                                        <li key={index}>
                                            <Card image={imageToUse} clickableHref={cardLinkHref} className={generatedClasses.wrapper}>
                                                <h3 className="c-card_heading o-truncated-text-lines-3">
                                                    <Link href={cardLinkHref}>
                                                        <a>{mainHeading}</a>
                                                    </Link>
                                                </h3>
                                                <div className="c-card_body">
                                                    <p className="c-card_content u-text-theme-7 o-truncated-text-lines-2">
                                                        {strapLine}
                                                    </p>
                                                    <div className="c-card_footer u-text-theme-0">
                                                        <p className="c-card_footer-item">
                                                            <SVGIcon name="icon-member" className="c-card_footer-icon u-fill-theme-0" />
                                                            <span>{`${totalMemberCount} Members`}</span>
                                                        </p>
                                                        <p className="c-card_footer-item">
                                                            <SVGIcon name="icon-discussion" className="c-card_footer-icon u-fill-theme-0" />
                                                            <span>{`${totalDiscussionCount} Discussions`}</span>
                                                        </p>
                                                    </div>
                                                </div>
                                            </Card>
                                        </li>

                                    )

                                })}
                            </DynamicListContainer>
                        </AriaLiveRegion>
                        <PaginationWithStatus
                            id="group-list-pagination"
                            shouldEnableLoadMore={shouldEnableLoadMore}
                            getPageAction={handleGetPage}
                            {...dynamicPagination} />
                    </LayoutColumn>
                </PageBody>
            </LayoutColumnContainer>
        </>

    )

}
