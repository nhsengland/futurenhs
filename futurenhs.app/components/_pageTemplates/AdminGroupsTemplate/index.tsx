import { useState, useMemo } from 'react';
import { useRouter } from 'next/router';
import classNames from 'classnames';

import { actions as actionConstants } from '@constants/actions';
import { SVGIcon } from '@components/SVGIcon';
import { Link } from '@components/Link';
import { PaginationWithStatus } from '@components/PaginationWithStatus';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { DynamicListContainer } from '@components/DynamicListContainer';
import { DataGrid } from '@components/DataGrid';

import { Props } from './interfaces';
import { getGroups } from '@services/getGroups';

/**
 * Admin groups dashboard template
 */
export const AdminGroupsTemplate: (props: Props) => JSX.Element = ({
    contentText,
    actions,
    pagination,
    groupsList,
    user
}) => {

    const router = useRouter();

    const [dynamicGroupsList, setGroupsList] = useState(groupsList);
    const [dynamicPagination, setPagination] = useState(pagination);

    const { secondaryHeading, noGroups, createGroup } = contentText ?? {};

    const hasUsers: boolean = true;
    const shouldEnableLoadMore: boolean = true;
    const shouldRenderCreateUserLink: boolean = actions.includes(actionConstants.SITE_ADMIN_MEMBERS_ADD);

    const columnList = [
        {
            children: 'Name',
            className: 'u-w-full tablet:u-w-2/5',
        },
        {
            children: 'Members',
            className: 'u-w-full tablet:u-w-1/5 tablet:u-text-center',
        },
        {
            children: 'Owner',
            className: 'u-w-full tablet:u-w-1/6',
        },
        {
            children: `Actions`,
            className: 'u-w-full tablet:u-w-1/6 tablet:u-text-right',
        }
    ];

    const rowList = useMemo(() => dynamicGroupsList?.map(({
        text,
        groupId,
        totalDiscussionCount,
        totalMemberCount
    }) => {

        const { mainHeading } = text ?? {};

        const generatedCellClasses = {
            name: classNames({
                ['u-justify-between u-w-full']: true
            }),
            members: classNames({
                ['u-justify-between u-w-full tablet:u-text-center']: true
            }),
            owner: classNames({
                ['u-justify-between u-w-full']: true,
            }),
            actions: classNames({
                ['u-justify-between u-w-full']: true,
            })
        };

        const generatedHeaderCellClasses = {
            name: classNames({
                ['u-text-bold']: true
            }),
            members: classNames({
                ['u-text-bold']: true
            }),
            owner: classNames({
                ['u-text-bold']: true
            }),
            actions: classNames({
                ['u-text-bold']: true
            })
        };

        const rows = [
            {
                children: <Link href={`/groups/${groupId}`}>{mainHeading}</Link>,
                className: generatedCellClasses.name,
                headerClassName: generatedHeaderCellClasses.name
            },
            {
                children: <Link href={`/groups/${groupId}/members`}><a aria-label={`Go to ${mainHeading} members list`}>{totalMemberCount}</a></Link>,
                className: generatedCellClasses.members,
                headerClassName: generatedHeaderCellClasses.members,
                shouldRenderCellHeader: true
            },
            {
                children: '',
                className: generatedCellClasses.owner,
                headerClassName: generatedHeaderCellClasses.owner,
                shouldRenderCellHeader: true
            },
            {
                children: <Link href={`/groups/${groupId}/update`}><a aria-label={`Edit group ${mainHeading}`}><SVGIcon name="icon-edit" className="u-w-4 u-h-4 u-mr-1 u-fill-theme-0" />Edit</a></Link>,
                className: 'u-w-full tablet:u-w-1/8 tablet:u-text-right',
                headerClassName: 'u-hidden',
            }
        ];

        return rows;

    }), [dynamicGroupsList]);

    /**
     * Handle client-side pagination
     */
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

        <>
            <LayoutColumnContainer className="u-w-full u-flex u-flex-col-reverse tablet:u-flex-row">
                <LayoutColumn tablet={9} className="c-page-body">
                    <h2 className="nhsuk-heading-l">{secondaryHeading}</h2>
                    {hasUsers

                        ? <DynamicListContainer
                            containerElementType="ul"
                            shouldEnableLoadMore={shouldEnableLoadMore}
                            className="u-list-none u-p-0">
                            <DataGrid
                                id="admin-table-groups"
                                columnList={columnList}
                                rowList={rowList}
                                text={{
                                    caption: 'Site users'
                                }} />
                        </DynamicListContainer>

                        : <p>{noGroups}</p>

                    }
                    <PaginationWithStatus
                        id="group-list-pagination"
                        shouldEnableLoadMore={shouldEnableLoadMore}
                        getPageAction={handleGetPage}
                        {...dynamicPagination} />
                </LayoutColumn>
                {shouldRenderCreateUserLink &&
                    <LayoutColumn tablet={3} className="c-page-body">
                        <Link href={`${router.asPath}/create`}>
                            <a className="c-button u-w-full">{createGroup}</a>
                        </Link>
                    </LayoutColumn>
                }
            </LayoutColumnContainer>
        </>

    )

}