import { useState } from 'react';
import { useRouter } from 'next/router';

import { actions as actionConstants } from '@constants/actions';
import { Link } from '@components/Link';
import { PaginationWithStatus } from '@components/PaginationWithStatus';
import { AdminLayout } from '@components/_pageLayouts/AdminLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { AriaLiveRegion } from '@components/AriaLiveRegion';
import { DynamicListContainer } from '@components/DynamicListContainer';
import { PageBody } from '@components/PageBody';

import { Props } from './interfaces';

/**
 * Admin users dashboard template
 */
export const AdminUsersTemplate: (props: Props) => JSX.Element = ({
    contentText,
    user,
    actions,
    pagination,
    usersList
}) => {

    const router = useRouter();

    const [dynamicUsersList, setUsersList] = useState(usersList);
    const [dynamicPagination, setPagination] = useState(pagination);

    const { usersHeading, noUsers, createUser } = contentText ?? {};

    const hasUsers: boolean = true;
    const shouldEnableLoadMore: boolean = true;

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

        <AdminLayout
            user={user}
            actions={actions}
            contentText={contentText}
            className="u-bg-theme-3">
                <PageBody>
                    <h2>Users</h2>
                    <LayoutColumnContainer>
                        <LayoutColumn tablet={8}>
                            <h2>{usersHeading}</h2>
                            <AriaLiveRegion>
                                {hasUsers

                                    ?   <DynamicListContainer 
                                            containerElementType="ul" 
                                            shouldFocusLatest={shouldEnableLoadMore}
                                            className="u-list-none u-p-0">
                                                
                                        </DynamicListContainer>

                                    :   <p>{noUsers}</p>

                                }
                            </AriaLiveRegion>
                            <PaginationWithStatus
                                id="group-list-pagination"
                                shouldEnableLoadMore={shouldEnableLoadMore}
                                getPageAction={handleGetPage}
                                {...dynamicPagination} />
                        </LayoutColumn>
                        {actions.includes(actionConstants.SITE_ADMIN_MEMBERS_ADD) &&
                            <LayoutColumn tablet={4}>
                                <Link href={`${router.asPath}/create`}>
                                    <a className="c-button u-w-full">{createUser}</a>
                                </Link>
                            </LayoutColumn>
                        }
                    </LayoutColumnContainer>
                </PageBody>
        </AdminLayout>

    )

}