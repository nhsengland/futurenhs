import { useMemo, useState } from 'react';
import { useRouter } from 'next/router';

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
    text,
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
        id, 
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
                    children: `${dateTime({})(requestDate)}`,
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

            const rows = [
                {
                    children: <Link href={`${asPath}/${id}`}>{fullName || role}</Link>,
                    className: 'u-w-1/4'
                },
                {
                    children: `${capitalise()(role)}`,
                    className: 'u-w-1/8'
                },
                {
                    children: `${dateTime({})(joinDate)}`,
                    className: 'u-w-1/8'
                },
                {
                    children: `${dateTime({})(lastLogInDate)}`,
                    className: 'u-w-1/4'
                }
            ];

            if(shouldRenderMemberEditColumn){

                rows.push({
                    children: <Link href={`${asPath}/${id}`}><a><SVGIcon name="icon-edit" className="u-w-4 u-h-4 u-mr-1 u-fill-theme-0" />Edit</a></Link>,
                    className: 'u-w-1/8'
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

    const { pendingMemberRequestsHeading, membersHeading } = text;

    return (

        <GroupLayout 
            id="members"
            user={user}
            actions={actions}
            text={text}
            image={image} 
            className="u-bg-theme-3">
                <LayoutColumn className="c-page-body">
                    {shouldRenderPendingMembersList &&
                        <div className="u-mb-12">
                            <h2>{pendingMemberRequestsHeading}</h2>
                            {hasPendingMembersList 
                            
                                ?   <DataGrid 
                                        columnList={pendingMemberColumnList}
                                        rowList={pendingMemberRowList}
                                        text={{
                                            caption: `${pendingMemberRequestsHeading} list`
                                        }} 
                                        shouldRenderCaption={false}
                                        className="u-mb-11" />

                                :   <p>This group currently has no outstanding membership requests</p>

                            }
                        </div>
                    }
                    <h2>{membersHeading}</h2>
                    <AriaLiveRegion>
                        {hasMembersList 
                        
                            ?   <>
                                    <DataGrid 
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

                            :   <p>This group currently contains no members</p>

                        }
                    </AriaLiveRegion>
                </LayoutColumn>
        </GroupLayout>

    )

}