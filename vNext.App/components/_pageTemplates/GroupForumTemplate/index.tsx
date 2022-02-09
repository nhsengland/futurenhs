import { useState } from 'react';
import { useRouter } from 'next/router';

import { actions as actionConstants } from '@constants/actions';
import { routeParams } from '@constants/routes';
import { getRouteToParam } from '@helpers/routing/getRouteToParam';
import { initials } from '@helpers/formatters/initials';
import { dateTime } from '@helpers/formatters/dateTime';
import { Link } from '@components/Link';
import { DynamicListContainer } from '@components/DynamicListContainer';
import { AriaLiveRegion } from '@components/AriaLiveRegion';
import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
import { UserMeta } from '@components/UserMeta';
import { LayoutColumn } from '@components/LayoutColumn';
import { PaginationWithStatus } from '@components/PaginationWithStatus';
import { Card } from '@components/Card';
import { SVGIcon } from '@components/SVGIcon';
import { getGroupDiscussions } from '@services/getGroupDiscussions';

import { Props } from './interfaces';

export const GroupForumTemplate: (props: Props) => JSX.Element = ({
    groupId,
    user,
    contentText,
    entityText,
    image,
    pagination,
    actions,
    discussionsList
}) => {

    const router = useRouter();

    const [dynamicDiscussionsList, setDiscussionsList] = useState(discussionsList);
    const [dynamicPagination, setPagination] = useState(pagination);

    const hasDiscussions: boolean = dynamicDiscussionsList.length > 0;

    const groupBasePath: string = getRouteToParam({
        router: router,
        paramName: routeParams.GROUPID,
        shouldIncludeParam: true
    });

    const { discussionsHeading,
        noDiscussions,
        createDiscussion } = contentText ?? {};

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

            if (!errors || !Object.keys(errors).length) {

                setDiscussionsList([...dynamicDiscussionsList, ...additionalDiscussions]);
                setPagination(pagination);

            }

        } catch (error) {

            console.log(error);

        }

    };

    return (

        <GroupLayout
            id="forum"
            user={user}
            actions={actions}
            text={entityText}
            image={image}
            className="u-bg-theme-3">
                <div className="u-w-full u-flex u-flex-col-reverse tablet:u-flex-row">
                    <LayoutColumn tablet={8} className="c-page-body">
                        <h2>{discussionsHeading}</h2>
                        <AriaLiveRegion>
                            {hasDiscussions

                                ? <DynamicListContainer>
                                    {dynamicDiscussionsList?.map?.(({
                                        text,
                                        discussionId,
                                        viewCount,
                                        responseCount,
                                        created,
                                        createdBy,
                                        modified,
                                        modifiedBy,
                                        isSticky
                                    }, index) => {

                                        const { title } = text ?? {};
                                        const creatorUserInitials: string = initials({ value: createdBy.text.userName });
                                        const creatorUserName: string = createdBy.text.userName;
                                        const creatorUserId: string = createdBy.id;
                                        const createdDate: string = dateTime({ value: created });
                                        const lastCommentUserName: string = modifiedBy.text.userName;
                                        const lastCommentDate: string = dateTime({ value: modified });

                                        return (

                                            <Card key={index} className="u-border-bottom-theme-10 u-mb-4">
                                                <h3 className="c-card_heading desktop:u-mb-4">
                                                    <Link href={`${router.asPath}/${discussionId}`}>
                                                        <a>
                                                            {isSticky && <span className="u-sr-only">Sticky: </span>}
                                                            {title}
                                                        </a>
                                                    </Link>
                                                </h3>
                                                <UserMeta
                                                    image={null}
                                                    text={{
                                                        initials: creatorUserInitials
                                                    }}
                                                    className="u-text-theme-7">
                                                    <span className="u-text-bold u-block">Created by <Link href={`${groupBasePath}/members/${creatorUserId}`}>{creatorUserName}</Link> {createdDate}</span>
                                                    {responseCount > 0 &&
                                                        <span className="u-block u-mt-1">Last comment by <Link href={`${groupBasePath}/members/${creatorUserId}`}>{lastCommentUserName}</Link> {lastCommentDate}</span>
                                                    }
                                                </UserMeta>
                                                <div className="c-card_footer u-text-theme-0">
                                                    <p className="c-card_footer-item">
                                                        <SVGIcon name="icon-comments" className="c-card_footer-icon u-fill-theme-0" />
                                                        <span>{`${responseCount} Comments`}</span>
                                                    </p>
                                                    <p className="c-card_footer-item">
                                                        <SVGIcon name="icon-view" className="c-card_footer-icon u-fill-theme-0" />
                                                        <span>{`${viewCount} Views`}</span>
                                                    </p>
                                                    {isSticky &&
                                                        <SVGIcon name="icon-pin" className="c-card_footer-icon u-fill-theme-0 u-float-right u-w-4 u-h-4 u-m-0" />
                                                    }
                                                </div>
                                            </Card>

                                        )

                                    })}
                                </DynamicListContainer>

                                : <p>{noDiscussions}</p>

                            }
                        </AriaLiveRegion>
                        <PaginationWithStatus
                            id="group-list-pagination"
                            shouldEnableLoadMore={true}
                            getPageAction={handleGetPage}
                            {...dynamicPagination} />
                    </LayoutColumn>
                    <LayoutColumn tablet={4} className="c-page-body">
                        {actions.includes(actionConstants.GROUPS_DISCUSSIONS_ADD) &&
                            <Link href={`${router.asPath}/create`}>
                                <a className="c-button u-w-full">{createDiscussion}</a>
                            </Link>
                        }
                    </LayoutColumn>
                </div>
        </GroupLayout>

    )

}
