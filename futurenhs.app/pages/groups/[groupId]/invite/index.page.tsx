import { GetServerSideProps } from 'next'
import { useContext, useState } from 'react'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { actions as actionConstants } from '@constants/actions'
import { features, groupTabIds, layoutIds } from '@constants/routes'
import { withUser } from '@helpers/hofs/withUser'
import { withRoutes } from '@helpers/hofs/withRoutes'
import { GetServerSidePropsContext } from '@appTypes/next'
import { withTextContent } from '@helpers/hofs/withTextContent'
import { withGroup } from '@helpers/hofs/withGroup'
import { formTypes } from '@constants/forms'
import {
    selectFormData,
    selectRequestMethod,
    selectPageProps,
    selectPagination,
    selectUser,
} from '@helpers/selectors/context'
import { ServerSideFormData } from '@helpers/util/form'
import { requestMethods } from '@constants/fetch'
import { notifications } from '@constants/notifications'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { NotificationsContext } from '@helpers/contexts/index'
import { useNotification } from '@helpers/hooks/useNotification'
import { GroupPage } from '@appTypes/page'
import { postGroupUserInvite } from '@services/postGroupUserInvite'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { getFeatureEnabled } from '@services/getFeatureEnabled'
import { VMessage, VRules, validate } from '@helpers/validation'
import { MultiInput } from '@components/forms/MultiInput'
import { GenericPageTextContent } from '@appTypes/content'
import { DynamicListContainer } from '@components/layouts/DynamicListContainer'
import { DataGrid } from '@components/layouts/DataGrid'
import {
    getPendingGroupMembers,
    InviteType,
    PendingMember,
} from '@services/getPendingGroupMembers'
import { PaginationWithStatus } from '@components/generic/PaginationWithStatus'
import { Pagination } from '@appTypes/pagination'
import { getDateFromUTC } from '@helpers/util/date'
import { ClickLink } from '@components/generic/ClickLink'
import { mdiCancel } from '@mdi/js'
import { Dialog } from '@components/generic/Dialog'
import { deletePlatformInvite } from '@services/deletePlatformInvite'
import { deleteGroupInvite } from '@services/deleteGroupInvite'
import { LayoutWidthContainer } from '@components/layouts/LayoutWidthContainer'
import { User } from '@appTypes/user'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { FormErrors } from '@appTypes/form'

declare interface ContentText extends GenericPageTextContent {
    noPendingInvites: string
}

export interface Props extends GroupPage {
    noUsers: string
    contentText: ContentText
    pendingList: Array<PendingMember>
}

/**
 * Group member invite template
 */
export const GroupMemberInvitePage: (props: Props) => JSX.Element = ({
    csrfToken,
    user,
    groupId,
    pagination,
    pendingList,
    contentText,
}) => {
    const [validation, setValidation] = useState<Array<VMessage>>([])
    const [emails, setEmails] = useState<Array<string>>([])
    const notificationsContext: any = useContext(NotificationsContext)
    const onChange = (e) => {
        const el = e.target as HTMLInputElement
        const validation = validate(el.value, [VRules.EMAIL, VRules.REQUIRED])
        setValidation(validation)
    }

    const [dynamicPendingList, setPendingList] = useState(pendingList)
    const [dynamicPagination, setPagination] = useState(pagination)
    const [inviteToDelete, setInviteToDelete] = useState<PendingMember | null>(
        null
    )
    const isDeleteInviteOpen = !!inviteToDelete

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

    const { mainHeading, secondaryHeading, noPendingInvites } = contentText

    const columnList = [
        {
            children: 'User',
            className: '',
        },
        {
            children: `Invited`,
            className: 'tablet:u-text-right',
        },
        {
            children: `Actions`,
            className: 'tablet:u-text-right',
        },
    ]

    const rowList = dynamicPendingList?.map((invite) => {
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

    const hasPendingInvites = !!dynamicPendingList?.length

    /**
     *TODO: Only accepts first email selected. Project ended. Extend API endpoint to take full array of emails
     */
    const handleInvite = async () => {
        if (Array.isArray(emails) && emails.length) {
            return
        }
        const [email] = emails
        const headers = getStandardServiceHeaders({
            csrfToken,
            accessToken: user.accessToken,
        })
        try {
            await postGroupUserInvite({
                user,
                headers,
                email: emails[0],
                groupId,
            })
            useNotification({
                notificationsContext,
                text: {
                    heading: notifications.SUCCESS,
                    body: `Invite sent to ${email}`,
                },
            })
        } catch (error) {}
        handleGetPage({
            pageNumber: pagination.pageNumber,
            pageSize: pagination.pageSize,
            refresh: true,
        })
        return Promise.resolve({})
    }

    /**
     * Handle client-side pagination
     */
    const handleGetPage = async ({
        pageNumber: requestedPageNumber,
        pageSize: requestedPageSize,
        refresh = false,
    }) => {
        const { data: additionalPending, pagination } =
            await getPendingGroupMembers({
                user,
                slug: groupId,
                pagination: {
                    pageNumber: requestedPageNumber,
                    pageSize: requestedPageSize,
                },
            })

        setPendingList(
            refresh
                ? additionalPending
                : [...dynamicPendingList, ...additionalPending]
        )
        setPagination(pagination)
    }

    /**
     * Render
     */
    return (
        <LayoutColumn className="c-page-body">
            <LayoutColumnContainer>
                <LayoutColumn tablet={8}>
                    <MultiInput
                        label="Email address"
                        type="text"
                        validation={validation}
                        onChange={onChange}
                        getMulti={setEmails}
                    />
                    <button
                        className={`c-button u-mt-10 ${
                            !emails.length ? 'c-button--disabled' : null
                        }`}
                        onClick={handleInvite}
                        disabled={!emails.length}
                    >
                        Send invite
                    </button>
                    <div>
                        <h2 className="nhsuk-heading-l">{secondaryHeading}</h2>
                        {hasPendingInvites ? (
                            <DynamicListContainer
                                containerElementType="div"
                                shouldEnableLoadMore
                                className="u-list-none u-p-0"
                            >
                                <DataGrid
                                    id="admin-table-pending-invites"
                                    columnList={columnList}
                                    rowList={rowList}
                                    text={{
                                        caption: 'Pending invites',
                                    }}
                                />

                                <PaginationWithStatus
                                    id="group-list-pagination"
                                    shouldEnableLoadMore
                                    getPageAction={handleGetPage}
                                    {...dynamicPagination}
                                />
                            </DynamicListContainer>
                        ) : (
                            <div>
                                <p>{noPendingInvites}</p>
                            </div>
                        )}
                    </div>
                </LayoutColumn>
            </LayoutColumnContainer>
        </LayoutColumn>
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
            routeId: 'f872b71a-0449-4821-a8da-b75bbd451b2d',
        },
        [withUser, withRoutes, withGroup, withTextContent],
        async (context: GetServerSidePropsContext) => {
            /**
             * Get data from request context
             */
            const props: Partial<Props> = selectPageProps(context)
            const formData: ServerSideFormData = selectFormData(context)
            const requestMethod: requestMethods = selectRequestMethod(context)
            const user: User = selectUser(context)

            props.layoutId = layoutIds.GROUP
            props.tabId = groupTabIds.MEMBERS
            props.pageTitle = `${props.entityText.title} - ${props.contentText.subTitle}`

            props.forms = {
                [formTypes.INVITE_USER]: {},
            }
            /**
             * Return page not found if user doesn't have permissions to invite a user - TODO: Pending API
             */
            const hasPermisson = props.actions?.includes(
                actionConstants.GROUPS_MEMBERS_INVITE
            )

            let groupInviteEnabled = false

            try {
                const { data: enabled } = await getFeatureEnabled({
                    slug: features.GROUP_INVITE,
                })
                groupInviteEnabled = enabled
            } catch (e) {}

            if (!hasPermisson || !groupInviteEnabled) {
                return {
                    notFound: true,
                }
            }

            try {
                const pagination: Pagination = {
                    pageNumber: selectPagination(context).pageNumber ?? 1,
                    pageSize: selectPagination(context).pageSize ?? 20,
                }

                const result = await getPendingGroupMembers({
                    user: user,
                    slug: props.groupId,
                    pagination,
                })

                props.pagination = result.pagination

                props.pendingList = result.data
            } catch (e) {}

            /**
             * Return data to page template
             */
            return handleSSRSuccessProps({ props, context })
        }
    )

/**
 * Export page template
 */
export default GroupMemberInvitePage
