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
import { AriaLiveRegion } from '@components/AriaLiveRegion';
import { DynamicListContainer } from '@components/DynamicListContainer';
import { DataGrid } from '@components/DataGrid';

import { Props } from './interfaces';

/**
 * Admin users dashboard template
 */
export const AdminUsersTemplate: (props: Props) => JSX.Element = ({
    contentText,
    actions,
    pagination,
    usersList
}) => {

    const router = useRouter();

    const [dynamicUsersList, setUsersList] = useState(usersList);
    const [dynamicPagination, setPagination] = useState(pagination);

    const { secondaryHeading, noUsers, createUser } = contentText ?? {};

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
                ['u-justify-between u-w-full tablet:u-w-1/4']: true
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
                headerClassName: generatedHeaderCellClasses.role
            },
            {
                children: `${dateTime({ value: joinDate })}`,
                className: generatedCellClasses.joinDate,
                headerClassName: generatedHeaderCellClasses.joinDate
            },
            {
                children: `${dateTime({ value: lastLogInDate })}`,
                className: generatedCellClasses.lastLoginDate,
                headerClassName: generatedHeaderCellClasses.lastLoginDate
            },
            {
                children: <Link href={`/users/${id}`}><a><SVGIcon name="icon-edit" className="u-w-4 u-h-4 u-mr-1 u-fill-theme-0" />Edit</a></Link>,
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

        // TODO

        //setUsersList([...dynamicUsersList, ...additionalUsers]);
        //setPagination(pagination);

    };

    return (

        <>
            <LayoutColumnContainer className="u-w-full u-flex u-flex-col-reverse tablet:u-flex-row">
                <LayoutColumn tablet={8} className="c-page-body">
                    <h2 className="nhsuk-heading-l">{secondaryHeading}</h2>
                    <AriaLiveRegion>
                        {hasUsers

                            ? <DynamicListContainer
                                containerElementType="ul"
                                shouldFocusLatest={shouldEnableLoadMore}
                                className="u-list-none u-p-0">
                                <DataGrid
                                    columnList={columnList}
                                    rowList={rowList}
                                    text={{
                                        caption: 'Site users'
                                    }} />
                            </DynamicListContainer>

                            : <p>{noUsers}</p>

                        }
                    </AriaLiveRegion>
                    <PaginationWithStatus
                        id="group-list-pagination"
                        shouldEnableLoadMore={shouldEnableLoadMore}
                        getPageAction={handleGetPage}
                        {...dynamicPagination} />
                </LayoutColumn>
                {shouldRenderCreateUserLink &&
                    <LayoutColumn tablet={4} className="c-page-body">
                        <Link href={`${router.asPath}/create`}>
                            <a className="c-button u-w-full">{createUser}</a>
                        </Link>
                    </LayoutColumn>
                }
            </LayoutColumnContainer>
        </>

    )

}