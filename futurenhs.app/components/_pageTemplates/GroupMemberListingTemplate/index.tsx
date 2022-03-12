import { useMemo, useState } from 'react';
import classNames from 'classnames';

import { actions as userActions } from '@constants/actions';
import { Link } from '@components/Link';
import { SVGIcon } from '@components/SVGIcon';
import { LayoutColumn } from '@components/LayoutColumn';
import { DataGrid } from '@components/DataGrid';
import { PaginationWithStatus } from '@components/PaginationWithStatus';
import { getGroupMembers } from '@services/getGroupMembers';
import { dateTime } from '@helpers/formatters/dateTime';
import { capitalise } from '@helpers/formatters/capitalise';

import { Props } from './interfaces';

/**
 * Group member listing template
 */
export const GroupMemberListingTemplate: (props: Props) => JSX.Element = ({
    groupId,
    user,
    actions,
    routes,
    pendingMembers,
    members,
    contentText,
    pagination
}) => {

    const [membersList, setMembersList] = useState(members);
    const [dynamicPagination, setPagination] = useState(pagination);

    const { pendingMemberRequestsHeading,
            membersHeading,
            noPendingMembers,
            noMembers,
            acceptMember,
            rejectMember,
            editMember } = contentText ?? {};

    const shouldRenderMemberEditColumn: Boolean = actions?.includes(userActions.GROUPS_MEMBERS_EDIT);
    const shouldRenderPendingMembersList: Boolean = actions?.includes(userActions.GROUPS_MEMBERS_PENDING_VIEW);

    const pendingMemberColumnList = [
        {
            children: 'Name'
        },
        {
            children: 'Email'
        },
        {
            children: 'Request date'
        },
        {
            children: 'Actions'
        }
    ];

    const pendingMemberRowList = useMemo(() => pendingMembers?.map(({
        fullName,
        email,
        requestDate }) => {

            const generatedCellClasses = {
                name: classNames({
                    ['u-justify-between u-w-full tablet:u-w-1/4']: true
                }),
                email: classNames({
                    ['u-justify-between u-w-full tablet:u-w-1/4']: true
                }),
                requestDate: classNames({
                    ['u-justify-between u-w-full tablet:u-w-1/6']: true,
                }),
                actions: classNames({
                    ['u-w-full tablet:u-w-1/6']: true,
                })
            };

            const generatedHeaderCellClasses = {
                name: classNames({
                    ['u-text-bold']: true
                }),
                email: classNames({
                    ['u-text-bold']: true
                }),
                requestDate: classNames({
                    ['u-text-bold']: true
                }),
                actions: classNames({
                    ['u-hidden']: true
                })
            };

            const rows = [
                {
                    children: fullName,
                    className: generatedCellClasses.name,
                    headerClassName: generatedHeaderCellClasses.name
                },
                {
                    children: email,
                    className: generatedCellClasses.email,
                    headerClassName: generatedHeaderCellClasses.email
                },
                {
                    children: `${dateTime({ value: requestDate })}`,
                    className: generatedCellClasses.requestDate,
                    headerClassName: generatedHeaderCellClasses.requestDate
                },
                {
                    children: <span className="u-flex u-justify-between u-w-full"><a href="#">{acceptMember}</a><a href="#">{rejectMember}</a></span>,
                    className: generatedCellClasses.actions,
                    headerClassName: generatedHeaderCellClasses.actions
                }
            ];

            return rows;

    }), [pendingMembers]);

    const memberColumnList = [
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
        }
    ];

    if (shouldRenderMemberEditColumn) {

        memberColumnList.push({
            children: `Actions`,
            className: 'tablet:u-text-right'
        });

    }

    const memberRowList = useMemo(() => membersList?.map(({
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
                children: <Link href={`${routes.groupMembersRoot}/${id}`}><a className="o-truncated-text-lines-3">{fullName || role}</a></Link>,
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
            }
        ];

        if (shouldRenderMemberEditColumn) {

            rows.push({
                children: <Link href={{
                    pathname: `${routes.groupMembersRoot}/${id}`,
                    query: { 
                        edit: 'true' 
                    }
                } as any}><a><SVGIcon name="icon-edit" className="u-w-4 u-h-4 u-mr-1 u-fill-theme-0" />{editMember}</a></Link>,
                className: 'u-w-full tablet:u-w-1/8 tablet:u-text-right',
                headerClassName: 'u-hidden'
            });

        }

        return rows;

    }), [membersList]);

    const hasPendingMembersList: Boolean = pendingMemberRowList?.length > 0;
    const hasMembersList: Boolean = memberRowList?.length > 0;

    const handleGetPage = async ({
        pageNumber: requestedPageNumber,
        pageSize: requestedPageSize
    }) => {

        const { data: additionalMembers, pagination } = await getGroupMembers({
            user: user,
            groupId: groupId,
            pagination: {
                pageNumber: requestedPageNumber,
                pageSize: requestedPageSize
            }
        });

        setMembersList([...membersList, ...additionalMembers]);
        setPagination(pagination);

    };

    return (

        <>
            <LayoutColumn className="c-page-body">
                {shouldRenderPendingMembersList &&
                    <div className="u-mb-12">
                        <h2 className="nhsuk-heading-l">{pendingMemberRequestsHeading}</h2>
                        {hasPendingMembersList

                            ? <DataGrid
                                id="group-table-pending-members"
                                columnList={pendingMemberColumnList}
                                rowList={pendingMemberRowList}
                                text={{
                                    caption: `${pendingMemberRequestsHeading} list`
                                }}
                                shouldRenderCaption={false}
                                className="u-mb-11" />

                            : <p>{noPendingMembers}</p>

                        }
                    </div>
                }
                <h2 className="nhsuk-heading-l">{membersHeading}</h2>
                {hasMembersList

                    ? <>
                        <DataGrid
                            id="group-table-members"
                            columnList={memberColumnList}
                            rowList={memberRowList}
                            text={{
                                caption: `${membersHeading} list`
                            }}
                            shouldRenderCaption={false} />
                        <PaginationWithStatus
                            id="member-list-pagination"
                            shouldEnableLoadMore={true}
                            getPageAction={handleGetPage}
                            {...dynamicPagination} />
                    </>

                    : <p>{noMembers}</p>

                }
            </LayoutColumn>
        </>

    )

}