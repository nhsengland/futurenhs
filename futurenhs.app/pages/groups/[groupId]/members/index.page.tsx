import { GetServerSideProps } from 'next'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { routeParams } from '@constants/routes'
import { layoutIds, groupTabIds } from '@constants/routes'
import { withUser } from '@helpers/hofs/withUser'
import { withRoutes } from '@helpers/hofs/withRoutes'
import { withGroup } from '@helpers/hofs/withGroup'
import { withTextContent } from '@helpers/hofs/withTextContent'
import { getGroupMembers } from '@services/getGroupMembers'
import {
    getPendingGroupMembers,
    InviteType,
    PendingMember,
} from '@services/getPendingGroupMembers'
import {
    selectUser,
    selectPagination,
    selectParam,
    selectCsrfToken,
    selectFormData,
    selectRequestMethod,
    selectPageProps,
} from '@helpers/selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { Pagination } from '@appTypes/pagination'
import { User } from '@appTypes/user'
import { formTypes } from '@constants/forms'
import { FormErrors } from '@appTypes/form'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { requestMethods } from '@constants/fetch'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { checkMatchingFormType } from '@helpers/util/form'
import { postGroupMemberAccept } from '@services/postGroupMemberAccept'
import { postGroupMemberReject } from '@services/postGroupMemberReject'
import { useContext, useMemo, useRef, useState } from 'react'
import classNames from 'classnames'
import { actions as userActions } from '@constants/actions'
import { Link } from '@components/generic/Link'
import { ActionLink } from '@components/generic/ActionLink'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { DataGrid } from '@components/layouts/DataGrid'
import { PaginationWithStatus } from '@components/generic/PaginationWithStatus'
import { dateTime } from '@helpers/formatters/dateTime'
import { capitalise } from '@helpers/formatters/capitalise'
import { getGenericFormError } from '@helpers/util/form'
import { ErrorSummary } from '@components/generic/ErrorSummary'
import { PendingMemberActions } from '@components/blocks/PendingMemberActions'
import { GroupPage } from '@appTypes/page'
import { GroupMember } from '@appTypes/group'
import { GroupsPageTextContent } from '@appTypes/content'
import { DynamicListContainer } from '@components/layouts/DynamicListContainer'
import { getDateFromUTC } from '@helpers/util/date'
import { ClickLink } from '@components/generic/ClickLink'
import { mdiCancel } from '@mdi/js'
import { deleteGroupInvite } from '@services/deleteGroupInvite'
import { deletePlatformInvite } from '@services/deletePlatformInvite'
import { Dialog } from '@components/generic/Dialog'
import { useNotification } from '@helpers/hooks/useNotification'
import { notifications } from '@constants/notifications'
import { NotificationsContext } from '@helpers/contexts'

declare interface ContentText extends GroupsPageTextContent {
    pendingMemberRequestsHeading: string
    membersHeading: string
    noPendingMembers: string
    noMembers: string
    acceptMember: string
    rejectMember: string
    editMember: string
}

export interface Props extends GroupPage {
    contentText: ContentText
    pendingMembers: Array<any>
    members: Array<GroupMember>
}

/**
 * Group member listing template
 */
export const GroupMemberListingPage: (props: Props) => JSX.Element = ({
    groupId,
    user,
    actions,
    routes,
    pendingMembers,
    members,
    contentText,
    pagination,
    isPublic,
    csrfToken,
    forms,
}) => {
    const [membersList, setMembersList] = useState(members)
    const [dynamicPagination, setPagination] = useState(pagination)
    const [pendingPagination, setPendingPagination] = useState(pagination)

    const [pendingMembersList, setPendingMembersList] = useState(pendingMembers)
    const [inviteToDelete, setInviteToDelete] = useState<PendingMember | null>(
        null
    )
    const errorSummaryRef: any = useRef()
    const notificationsContext: any = useContext(NotificationsContext)
    const [errors, setErrors] = useState(forms.initial.errors ?? {})

    const {
        pendingMemberRequestsHeading,
        membersHeading,
        noPendingMembers,
        noMembers,
        acceptMember,
        rejectMember,
        editMember,
    } = contentText ?? {}

    const shouldRenderMemberEditColumn: Boolean = actions?.includes(
        userActions.GROUPS_MEMBERS_EDIT
    )
    const shouldRenderPendingMembersList: Boolean =
        actions?.includes(userActions.GROUPS_MEMBERS_PENDING_VIEW) && !isPublic

    const handleDeleteInvite = async () => {
        if (!inviteToDelete) return
        const { id, email, rowVersion: etag, inviteType } = inviteToDelete
        try {
            const headers = getStandardServiceHeaders({
                csrfToken,
                etag,
            })
            try {
                if (inviteType === InviteType.GROUP) {
                    await deleteGroupInvite({
                        inviteId: id,
                        user,
                        headers,
                    })
                } else if (inviteType === InviteType.PLATFORM) {
                    await deletePlatformInvite({
                        inviteId: id,
                        user,
                        headers,
                    })
                }
            } catch (e) {
                console.log(e)
                return
            }
            handleGetPage({
                pageNumber: pagination.pageNumber,
                pageSize: pagination.pageSize,
                refresh: true,
            })
            useNotification({
                notificationsContext,
                text: {
                    heading: notifications.SUCCESS,
                    body: `Cancelled invite to ${email}.`,
                },
            })
            return Promise.resolve({})
        } catch (error) {
            console.log(error)
        }
    }

    // /**
    //  * Handle client-side accept member submission
    //  */
    // const handleAcceptMember = async (
    //     formData: FormData
    // ): Promise<FormErrors> => {
    //     return new Promise((resolve) => {
    //         const headers = getStandardServiceHeaders({
    //             csrfToken,
    //             accessToken: user.accessToken,
    //         })

    //         postGroupMemberAccept({ user, groupId, body: formData, headers })
    //             .then(async () => {
    //                 const [updatedPendingMembers] = await Promise.all([
    //                     // getPendingGroupMembers({ user, slug: groupId }),
    //                     getGroupMembers({ user, groupId, pagination }),
    //                 ])
    //                 setPendingMembersList(updatedPendingMembers.data)
    //                 // setMembersList(updatedMembers.data)

    //                 resolve({})
    //             })
    //             .catch((error) => {
    //                 const errors: FormErrors =
    //                     getServiceErrorDataValidationErrors(error) ||
    //                     getGenericFormError(error)

    //                 setErrors(errors)
    //                 errorSummaryRef.current?.focus?.()
    //                 resolve(errors)
    //             })

    //         resolve({})
    //     })
    // }
    const isDeleteInviteOpen = !!inviteToDelete

    /**
     * Handle client-side reject member submission
     */
    const handleRejectMember = async (
        formData: FormData
    ): Promise<FormErrors> => {
        return new Promise((resolve) => {
            const headers = getStandardServiceHeaders({
                csrfToken,
                accessToken: user.accessToken,
            })

            postGroupMemberReject({ user, groupId, body: formData, headers })
                .then(async () => {
                    const [updatedPendingMembers] = await Promise.all([
                        // getPendingGroupMembers({ user, slug: groupId }),
                        getGroupMembers({ user, groupId, pagination }),
                    ])
                    setPendingMembersList(updatedPendingMembers.data)
                    // setMembersList(updatedMembers.data)

                    resolve({})
                })
                .catch((error) => {
                    const errors: FormErrors =
                        getServiceErrorDataValidationErrors(error) ||
                        getGenericFormError(error)

                    setErrors(errors)
                    errorSummaryRef.current?.focus?.()
                    resolve(errors)
                })

            resolve({})
        })
    }

    const pendingMemberColumnList = [
        {
            children: 'User',
        },
        {
            children: 'Invited',
        },
        {
            children: 'Actions',
        },
    ]

    const pendingMemberRowList = pendingMembersList?.map((invite) => {
        const rows = [
            {
                children: <span>{invite.email}</span>,
                className: 'u-w-full tablet:u-w-1/8 tablet:u-text-left',
                headerClassName: 'u-hidden',
            },
            {
                children: <span>{getDateFromUTC(invite.createdAtUTC)}</span>,
                className: 'u-w-full tablet:u-w-1/8 tablet:u-text-right',
                headerClassName: 'u-hidden',
            },
            {
                children: (
                    <ClickLink
                        onClick={() => {
                            setInviteToDelete(invite)
                        }}
                        text={{
                            body: 'Cancel',
                            ariaLabel: `Cancel invite to user ${invite.email}`,
                        }}
                        iconName={mdiCancel}
                        material
                    />
                ),
                className: 'u-w-full tablet:u-w-1/8 tablet:u-text-right',
                headerClassName: 'u-hidden',
            },
        ]

        return rows
    })

    const memberColumnList = [
        {
            children: 'Name',
            className: '',
        },
        {
            children: 'Role',
            className: '',
        },
        {
            children: 'Date joined',
            className: '',
        },
        {
            children: 'Last activity',
            className: '',
        },
    ]

    if (shouldRenderMemberEditColumn) {
        memberColumnList.push({
            children: `Actions`,
            className: 'tablet:u-text-right',
        })
    }

    const memberRowList = useMemo(
        () =>
            membersList?.map(
                ({ id, fullName, role, joinDate, lastLogInDate }) => {
                    const generatedCellClasses = {
                        name: classNames({
                            ['u-justify-between u-w-full tablet:u-w-1/4']: true,
                        }),
                        role: classNames({
                            ['u-justify-between u-w-full tablet:u-w-1/4']: true,
                        }),
                        joinDate: classNames({
                            ['u-justify-between u-w-full tablet:u-w-1/6']: true,
                        }),
                        lastLoginDate: classNames({
                            ['u-justify-between u-w-full tablet:u-w-1/6']: true,
                        }),
                    }

                    const generatedHeaderCellClasses = {
                        name: classNames({
                            ['u-text-bold']: true,
                        }),
                        role: classNames({
                            ['u-text-bold']: true,
                        }),
                        joinDate: classNames({
                            ['u-text-bold']: true,
                        }),
                        lastLoginDate: classNames({
                            ['u-text-bold']: true,
                        }),
                    }

                    const rows = [
                        {
                            children: (
                                <Link href={`${routes.groupMembersRoot}/${id}`}>
                                    <a className="o-truncated-text-lines-3">
                                        {fullName || role}
                                    </a>
                                </Link>
                            ),
                            className: generatedCellClasses.name,
                            headerClassName: generatedHeaderCellClasses.name,
                        },
                        {
                            children: `${capitalise({ value: role })}`,
                            shouldRenderCellHeader: true,
                            className: generatedCellClasses.role,
                            headerClassName: generatedHeaderCellClasses.role,
                        },
                        {
                            children: `${dateTime({ value: joinDate })}`,
                            shouldRenderCellHeader: true,
                            className: generatedCellClasses.joinDate,
                            headerClassName:
                                generatedHeaderCellClasses.joinDate,
                        },
                        {
                            children: `${dateTime({ value: lastLogInDate })}`,
                            shouldRenderCellHeader: true,
                            className: generatedCellClasses.lastLoginDate,
                            headerClassName:
                                generatedHeaderCellClasses.lastLoginDate,
                        },
                    ]

                    if (shouldRenderMemberEditColumn) {
                        rows.push({
                            children: (
                                <ActionLink
                                    href={`${routes.groupMembersRoot}/${id}/update`}
                                    text={{
                                        body: editMember,
                                        ariaLabel: `Edit member ${
                                            fullName || role
                                        }`,
                                    }}
                                    iconName="icon-edit"
                                />
                            ),
                            className:
                                'u-w-full tablet:u-w-1/8 tablet:u-text-right',
                            headerClassName: 'u-hidden',
                        })
                    }

                    return rows
                }
            ),
        [membersList]
    )

    const hasPendingMembersList: Boolean = pendingMembersList?.length > 0
    const hasMembersList: Boolean = memberRowList?.length > 0

    const handleGetPage = async ({
        pageNumber: requestedPageNumber,
        pageSize: requestedPageSize,
        refresh,
    }) => {
        const { data: additionalMembers, pagination: pendingPagination } =
            await getGroupMembers({
                user: user,
                groupId: groupId,
                pagination: {
                    pageNumber: requestedPageNumber,
                    pageSize: requestedPageSize,
                },
            })

        const { data: additionalPending, pagination } =
            await getPendingGroupMembers({
                user,
                slug: groupId,
                pagination: {
                    pageNumber: requestedPageNumber,
                    pageSize: requestedPageSize,
                },
            })

        setMembersList(
            refresh ? additionalMembers : [...membersList, ...additionalMembers]
        )
        setPendingMembersList(
            refresh
                ? additionalPending
                : [...pendingMembersList, ...additionalPending]
        )
        setPagination(pagination)
        setPendingPagination(pendingPagination)
    }

    return (
        <>
            <Dialog
                id="dialog-delete-domain"
                isOpen={isDeleteInviteOpen}
                text={{
                    cancelButton: 'Cancel',
                    confirmButton: 'Yes, cancel invite',
                    heading: 'Cancel invite',
                }}
                cancelAction={() => {
                    setInviteToDelete(null)
                }}
                confirmAction={() => {
                    handleDeleteInvite()
                    setInviteToDelete(null)
                }}
            >
                <p className="u-text-bold">
                    {`Are you sure you would like to cancel the invite for ${inviteToDelete?.email}?`}
                </p>
            </Dialog>
            <LayoutColumn className="c-page-body">
                {shouldRenderPendingMembersList && (
                    <div className="u-mb-12">
                        <ErrorSummary
                            ref={errorSummaryRef}
                            errors={errors}
                            className="u-mb-10"
                        />
                        <h2 className="nhsuk-heading-l">
                            {pendingMemberRequestsHeading}
                        </h2>
                        {hasPendingMembersList ? (
                            <DynamicListContainer
                                containerElementType="div"
                                shouldEnableLoadMore
                                className="u-list-none u-p-0"
                            >
                                <DataGrid
                                    id="admin-table-domains"
                                    columnList={pendingMemberColumnList}
                                    rowList={pendingMemberRowList}
                                    text={{
                                        caption: 'Allowed domains',
                                    }}
                                />
                            </DynamicListContainer>
                        ) : (
                            <p>{noMembers}</p>
                        )}
                        <PaginationWithStatus
                            id="group-list-pagination"
                            shouldEnableLoadMore
                            getPageAction={handleGetPage}
                            {...pendingPagination}
                        />
                    </div>
                )}
                <h2 className="nhsuk-heading-l">{membersHeading}</h2>
                {hasMembersList ? (
                    <>
                        <DataGrid
                            id="group-table-members"
                            columnList={memberColumnList}
                            rowList={memberRowList}
                            text={{
                                caption: `${membersHeading} list`,
                            }}
                            shouldRenderCaption={false}
                        />
                        <PaginationWithStatus
                            id="member-list-pagination"
                            shouldEnableLoadMore={true}
                            getPageAction={handleGetPage}
                            {...dynamicPagination}
                        />
                    </>
                ) : (
                    <p>{noMembers}</p>
                )}
            </LayoutColumn>
        </>
    )
}

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = async (
    context: GetServerSidePropsContext
) =>
    await pipeSSRProps(
        context,
        {
            routeId: '3d4a3b47-ba2c-43fa-97cf-90de93eeb4f8',
        },
        [withUser, withRoutes, withGroup, withTextContent],
        async (context: GetServerSidePropsContext) => {
            /**
             * Get data from request context
             */
            const props: Partial<any> = selectPageProps(context)
            const user: User = selectUser(context)
            const csrfToken: string = selectCsrfToken(context)
            const requestMethod: requestMethods = selectRequestMethod(context)
            const currentValues: any = selectFormData(context)
            const groupId: string = selectParam(context, routeParams.GROUPID)
            const pagination: Pagination = {
                pageNumber: selectPagination(context).pageNumber ?? 1,
                pageSize: selectPagination(context).pageSize ?? 10,
            }

            props.csrfToken = csrfToken
            props.layoutId = layoutIds.GROUP
            props.tabId = groupTabIds.MEMBERS
            props.pageTitle = `${props.entityText.title} - ${props.contentText.subTitle}`

            props.forms = {
                initial: {},
            }

            /**
             * Get data from services
             */
            try {
                const [groupMembers, groupPendingMembers] = await Promise.all([
                    getGroupMembers({ user, groupId, pagination }),
                    getPendingGroupMembers({ user, slug: groupId }),
                ])

                props.members = groupMembers.data
                props.pagination = groupMembers.pagination
                props.pendingMembers = groupPendingMembers.data

                /**
                 * Handle server-side form post
                 */
                if (currentValues && requestMethod === requestMethods.POST) {
                    const headers = getStandardServiceHeaders({
                        csrfToken,
                        accessToken: user.accessToken,
                    })

                    const isAcceptForm: boolean = checkMatchingFormType(
                        currentValues,
                        formTypes.ACCEPT_GROUP_MEMBER
                    )
                    const isRejectForm: boolean = checkMatchingFormType(
                        currentValues,
                        formTypes.REJECT_GROUP_MEMBER
                    )

                    if (isAcceptForm) {
                        await postGroupMemberAccept({
                            groupId,
                            user,
                            headers,
                            body: currentValues,
                        })
                    }

                    if (isRejectForm) {
                        await postGroupMemberReject({
                            groupId,
                            user,
                            headers,
                            body: currentValues,
                        })
                    }
                }
            } catch (error) {
                const validationErrors: FormErrors =
                    getServiceErrorDataValidationErrors(error)

                if (validationErrors) {
                    props.forms.initial.errors = validationErrors
                } else {
                    return handleSSRErrorProps({ props, error })
                }
            }

            /**
             * Return data to page template
             */
            return handleSSRSuccessProps({ props, context })
        }
    )

/**
 * Export page template
 */
export default GroupMemberListingPage
