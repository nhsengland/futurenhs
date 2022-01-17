import { useMemo, useState } from 'react';
import { useRouter } from 'next/router';

import { Link } from '@components/Link';
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
    pendingMembers,
    members,
    content,
    image,
    pagination
}) => {

    const { asPath } = useRouter();
    const [membersList, setMembersList] = useState(members);
    const [dynamicPagination, setPagination] = useState(pagination);

    const pendingMemberRowList = useMemo(() => pendingMembers?.map(({ 
        id, 
        fullName,
        email, 
        requestDate }) => {

            const rows = [
                {
                    children: <Link href={`${asPath}/${id}`}>{fullName}</Link>,
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

    const memberRowList = useMemo(() => membersList?.map(({ 
        id, 
        fullName,
        role,
        joinDate, 
        lastLogInDate }) => {

            const rows = [
                {
                    children: <Link href={`${asPath}/${id}`}>{fullName}</Link>,
                    className: 'u-w-1/4'
                },
                {
                    children: `${capitalise()(role)}`,
                    className: 'u-w-1/4'
                },
                {
                    children: `${dateTime({})(joinDate)}`,
                    className: 'u-w-1/4'
                },
                {
                    children: `${dateTime({})(lastLogInDate)}`,
                    className: 'u-w-1/4'
                }
            ];

            return rows;

    }), [membersList]);

    const shouldRenderPendingMembersList: Boolean = pendingMemberRowList?.length > 0;
    const shouldRenderMembersList: Boolean = memberRowList?.length > 0;

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

        <GroupLayout 
            id="members"
            user={user}
            content={content}
            image={image} 
            className="u-bg-theme-3">
                <LayoutColumn className="c-page-body">
                    {shouldRenderPendingMembersList &&
                        <DataGrid 
                            columnList={[
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
                            ]}
                            rowList={pendingMemberRowList}
                            content={{
                                captionHtml: "Pending member requests"
                            }} 
                            shouldRenderCaption={true}
                            className="u-mb-11" />
                    }
                    <AriaLiveRegion>
                        {shouldRenderMembersList &&
                            <>
                                <DataGrid 
                                    columnList={[
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
                                    ]}
                                    rowList={memberRowList}
                                    content={{
                                        captionHtml: "Group members"
                                    }} 
                                    shouldRenderCaption={true} />
                                <PaginationWithStatus 
                                    id="member-list-pagination"
                                    shouldEnableLoadMore={true}
                                    getPageAction={handleGetPage}
                                    {...dynamicPagination} />
                            </>
                        }
                    </AriaLiveRegion>
                </LayoutColumn>
        </GroupLayout>

    )

}