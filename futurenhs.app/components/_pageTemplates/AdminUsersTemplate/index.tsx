import { useState, useMemo } from 'react';
import { useRouter } from 'next/router';
import classNames from 'classnames';

import { capitalise } from '@helpers/formatters/capitalise';
import { dateTime } from '@helpers/formatters/dateTime';
import { actions as actionConstants } from '@constants/actions';
import { SVGIcon } from '@components/SVGIcon';
import { Link } from '@components/Link';
import { PaginationWithStatus } from '@components/PaginationWithStatus';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { DynamicListContainer } from '@components/DynamicListContainer';
import { DataGrid } from '@components/DataGrid';
import { getSiteUsers } from '@services/getSiteUsers';

import { Props } from './interfaces';

/**
 * Admin users dashboard template
 */
export const AdminUsersTemplate: (props: Props) => JSX.Element = ({
    contentText,
    actions,
    pagination,
    usersList,
    user
}) => {

    const router = useRouter();

    const [dynamicUsersList, setUsersList] = useState(usersList);
    const [dynamicPagination, setPagination] = useState(pagination);

    const { secondaryHeading, noUsers, inviteUser } = contentText ?? {};

    const hasUsers: boolean = true;
    const shouldEnableLoadMore: boolean = true;
    const shouldRenderCreateUserLink: boolean = actions.includes(actionConstants.SITE_ADMIN_MEMBERS_ADD);

    const columnList = [
        {
            children: 'Name',
            className: ''
        },
        {
            children: 'Role',
            className: ''
        },
        {
            children: 'Date joined',
            className: ''
        },
        {
            children: 'Last logged in',
            className: ''
        },
        {
            children: `Actions`,
            className: 'tablet:u-text-right'
        }
    ];

    const rowList = useMemo(() => dynamicUsersList?.map(({
        id,
        fullName,
        role,
        joinDate,
        lastLogInDate }) => {

        const generatedCellClasses = {
            name: classNames({
                ['u-justify-between u-w-full tablet:u-w-1/4 o-truncated-text-lines-1']: true
            }),
            role: classNames({
                ['u-justify-between u-w-full tablet:u-w-1/4']: true
            }),
            joinDate: classNames({
                ['u-justify-between u-w-full tablet:u-w-1/6']: true,
            }),
            lastLoginDate: classNames({
                ['u-justify-between u-w-full tablet:u-w-1/6']: true,
            })
        };

        const generatedHeaderCellClasses = {
            name: classNames({
                ['u-text-bold']: true
            }),
            role: classNames({
                ['u-text-bold']: true
            }),
            joinDate: classNames({
                ['u-text-bold']: true
            }),
            lastLoginDate: classNames({
                ['u-text-bold']: true
            })
        };

        const rows = [
            {
                children: <Link href={`/users/${id}`}>{fullName || role}</Link>,
                className: generatedCellClasses.name,
                headerClassName: generatedHeaderCellClasses.name
            },
            {
                children: `${capitalise({ value: role })}`,
                className: generatedCellClasses.role,
                headerClassName: generatedHeaderCellClasses.role,
                shouldRenderCellHeader: true
            },
            {
                children: `${dateTime({ value: joinDate })}`,
                className: generatedCellClasses.joinDate,
                headerClassName: generatedHeaderCellClasses.joinDate,
                shouldRenderCellHeader: true
            },
            {
                children: `${dateTime({ value: lastLogInDate })}`,
                className: generatedCellClasses.lastLoginDate,
                headerClassName: generatedHeaderCellClasses.lastLoginDate,
                shouldRenderCellHeader: true
            },
            {
                children: <Link href={{
                    pathname: `/users/${id}`,
                    query: { 
                        edit: 'true' 
                    }
                } as any}><a aria-label={`Edit user ${fullName || role}`}><SVGIcon name="icon-edit" className="u-w-4 u-h-4 u-mr-1 u-fill-theme-0"/>Edit user</a></Link>,
                className: 'u-w-full tablet:u-w-1/8 tablet:u-text-right',
                headerClassName: 'u-hidden'
            }
        ];

        return rows;

    }), [dynamicUsersList]);

    /**
     * Handle client-side pagination
     */
    const handleGetPage = async ({
        pageNumber: requestedPageNumber,
        pageSize: requestedPageSize
    }) => {

        const { data: additionalUsers, pagination } = await getSiteUsers({
            user: user,
            pagination: {
                pageNumber: requestedPageNumber,
                pageSize: requestedPageSize
            }
        });

        setUsersList([...dynamicUsersList, ...additionalUsers]);
        setPagination(pagination);

    };

    return (

        <>
            <LayoutColumnContainer className="u-w-full u-flex u-flex-col-reverse tablet:u-flex-row">
                <LayoutColumn tablet={9} desktop={10} className="c-page-body">
                    <h2 className="nhsuk-heading-l">{secondaryHeading}</h2>
                    {hasUsers

                        ?   <DynamicListContainer
                                containerElementType="ul"
                                shouldEnableLoadMore={shouldEnableLoadMore}
                                className="u-list-none u-p-0">
                                <DataGrid
                                    columnList={columnList}
                                    rowList={rowList}
                                    text={{
                                        caption: 'Site users'
                                    }} />
                            </DynamicListContainer>

                        :   <p>{noUsers}</p>

                    }
                    <PaginationWithStatus
                        id="group-list-pagination"
                        shouldEnableLoadMore={shouldEnableLoadMore}
                        getPageAction={handleGetPage}
                        {...dynamicPagination} />
                </LayoutColumn>
                {shouldRenderCreateUserLink &&
                    <LayoutColumn tablet={3} desktop={2} className="c-page-body">
                        <Link href={`${router.asPath}/create`}>
                            <a className="c-button u-w-full">{inviteUser}</a>
                        </Link>
                    </LayoutColumn>
                }
            </LayoutColumnContainer>
        </>

    )

}