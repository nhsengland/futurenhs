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
import { getPendingGroupMembers } from '@services/getPendingGroupMembers'
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
import { useMemo, useRef, useState } from 'react'
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
    const [pendingMembersList, setPendingMembersList] = useState(pendingMembers)

    const errorSummaryRef: any = useRef()

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

    /**
     * Handle client-side accept member submission
     */
    const handleAcceptMember = async (
        formData: FormData
    ): Promise<FormErrors> => {
        return new Promise((resolve) => {
            const headers = getStandardServiceHeaders({
                csrfToken,
                accessToken: user.accessToken,
            })

            postGroupMemberAccept({ user, groupId, body: formData, headers })
                .then(async () => {
                    const [updatedPendingMembers, updatedMembers] =
                        await Promise.all([
                            getPendingGroupMembers({ user, groupId }),
                            getGroupMembers({ user, groupId, pagination }),
                        ])
                    setPendingMembersList(updatedPendingMembers.data)
                    setMembersList(updatedMembers.data)

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
                    const [updatedPendingMembers, updatedMembers] =
                        await Promise.all([
                            getPendingGroupMembers({ user, groupId }),
                            getGroupMembers({ user, groupId, pagination }),
                        ])
                    setPendingMembersList(updatedPendingMembers.data)
                    setMembersList(updatedMembers.data)

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
            children: 'Name',
        },
        {
            children: 'Email',
        },
        {
            children: 'Request date',
        },
        {
            children: 'Actions',
        },
    ]

    const pendingMemberRowList = useMemo(
        () =>
            pendingMembersList?.map(({ fullName, email, requestDate, id }) => {
                const generatedCellClasses = {
                    name: classNames({
                        ['u-justify-between u-w-full tablet:u-w-1/4 u-align-middle']:
                            true,
                    }),
                    email: classNames({
                        ['u-justify-between u-w-full tablet:u-w-1/4 u-align-middle']:
                            true,
                    }),
                    requestDate: classNames({
                        ['u-justify-between u-w-full tablet:u-w-1/4 u-align-middle']:
                            true,
                    }),
                    actions: classNames({
                        ['u-w-full tablet:u-w-1/4']: true,
                    }),
                }

                const generatedHeaderCellClasses = {
                    name: classNames({
                        ['u-text-bold']: true,
                    }),
                    email: classNames({
                        ['u-text-bold']: true,
                    }),
                    requestDate: classNames({
                        ['u-text-bold']: true,
                    }),
                    actions: classNames({
                        ['u-hidden']: true,
                    }),
                }

                const rows = [
                    {
                        children: fullName,
                        className: generatedCellClasses.name,
                        headerClassName: generatedHeaderCellClasses.name,
                    },
                    {
                        children: email,
                        className: generatedCellClasses.email,
                        headerClassName: generatedHeaderCellClasses.email,
                    },
                    {
                        children: `${dateTime({ value: requestDate })}`,
                        className: generatedCellClasses.requestDate,
                        headerClassName: generatedHeaderCellClasses.requestDate,
                    },
                    {
                        children: (
                            <PendingMemberActions
                                acceptAction={handleAcceptMember}
                                rejectAction={handleRejectMember}
                                memberId={id}
                                text={{
                                    acceptMember,
                                    rejectMember,
                                }}
                            />
                        ),
                        className: generatedCellClasses.actions,
                        headerClassName: generatedHeaderCellClasses.actions,
                    },
                ]

                return rows
            }),
        [pendingMembersList]
    )

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
            children: 'Last logged in',
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

    const hasPendingMembersList: Boolean = pendingMemberRowList?.length > 0
    const hasMembersList: Boolean = memberRowList?.length > 0

    const handleGetPage = async ({
        pageNumber: requestedPageNumber,
        pageSize: requestedPageSize,
    }) => {
        const { data: additionalMembers, pagination } = await getGroupMembers({
            user: user,
            groupId: groupId,
            pagination: {
                pageNumber: requestedPageNumber,
                pageSize: requestedPageSize,
            },
        })

        setMembersList([...membersList, ...additionalMembers])
        setPagination(pagination)
    }

    return (
        <>
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
                            <DataGrid
                                id="group-table-pending-members"
                                columnList={pendingMemberColumnList}
                                rowList={pendingMemberRowList}
                                text={{
                                    caption: `${pendingMemberRequestsHeading} list`,
                                }}
                                shouldRenderCaption={false}
                                className="u-mb-11"
                            />
                        ) : (
                            <p>{noPendingMembers}</p>
                        )}
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
                    getPendingGroupMembers({ user, groupId }),
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
