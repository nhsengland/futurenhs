import { useMemo, useState } from 'react';
import { useRouter } from 'next/router';
import classNames from 'classnames';

import { actions as userActions } from '@constants/actions';
import { Link } from '@components/Link';
import { SVGIcon } from '@components/SVGIcon';
import { AriaLiveRegion } from '@components/AriaLiveRegion';
import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
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
    pendingMembers,
    members,
    entityText,
    contentText,
    image,
    pagination
}) => {

    const { asPath } = useRouter();
    const [membersList, setMembersList] = useState(members);
    const [dynamicPagination, setPagination] = useState(pagination);

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
            children: 'Accept'
        },
        {
            children: 'Reject'
        }
    ];
    const pendingMemberRowList = useMemo(() => pendingMembers?.map(({ 
        fullName,
        email, 
        requestDate }) => {

            const rows = [
                {
                    children: fullName,
                    className: 'u-w-1/4'
                },
                {
                    children: email,
                    className: 'u-w-1/4'
                },
                {
                    children: `${dateTime({ value: requestDate })}`,
                    className: 'u-w-1/4'
                },
                {
                    children: <a href="#">Accept</a>,
                    className: 'u-w-1/8'
                },
                {
                    children: <a href="#">Reject</a>,
                    className: 'u-w-1/8'
                }
            ];

            return rows;

    }), [pendingMembers]);

    const memberColumnList = [
        {
            children: 'Name'
        },
        {
            children: 'Role'
        },
        {
            children: 'Date joined'
        },
        {
            children: 'Last logged in'
        }
    ];

    if(shouldRenderMemberEditColumn){

        memberColumnList.push({
            children: `Edit`
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
                    ['u-w-full tablet:u-w-1/4']: true
                }),
                role: classNames({
                    ['u-flex-col u-w-full tablet:u-w-1/4']: true
                }),
                joinDate: classNames({
                    ['u-flex-col u-w-full tablet:u-w-1/6']: true,
                }),
                lastLoginDate: classNames({
                    ['u-flex-col u-w-full tablet:u-w-1/6']: true,
                })
            };

            const generatedHeaderCellClasses = {
                name: classNames({
                    ['u-hidden']: true
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
                    children: <Link href={`${asPath}/${id}`}>{fullName || role}</Link>,
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

            if(shouldRenderMemberEditColumn){

                rows.push({
                    children: <Link href={`${asPath}/${id}`}><a><SVGIcon name="icon-edit" className="u-w-4 u-h-4 u-mr-1 u-fill-theme-0" />Edit</a></Link>,
                    className: 'u-w-full tablet:u-w-1/8',
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

    const { pendingMemberRequestsHeading, 
            membersHeading,
            noPendingMembers,
            noMembers } = contentText ?? {};

    return (

        <GroupLayout 
            id="members"
            user={user}
            actions={actions}
            text={entityText}
            image={image} 
            className="u-bg-theme-3">
                <LayoutColumn className="c-page-body">
                    {shouldRenderPendingMembersList &&
                        <div className="u-mb-12">
                            <h2>{pendingMemberRequestsHeading}</h2>
                            {hasPendingMembersList 
                            
                                ?   <DataGrid 
                                        id="group-table-pending-members"
                                        columnList={pendingMemberColumnList}
                                        rowList={pendingMemberRowList}
                                        text={{
                                            caption: `${pendingMemberRequestsHeading} list`
                                        }} 
                                        shouldRenderCaption={false}
                                        className="u-mb-11" />

                                :   <p>{noPendingMembers}</p>

                            }
                        </div>
                    }
                    <h2>{membersHeading}</h2>
                    <AriaLiveRegion>
                        {hasMembersList 
                        
                            ?   <>
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

                            :   <p>{noMembers}</p>

                        }
                    </AriaLiveRegion>
                </LayoutColumn>
        </GroupLayout>

    )

}