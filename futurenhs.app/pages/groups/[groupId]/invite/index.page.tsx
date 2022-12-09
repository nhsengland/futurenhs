import { GetServerSideProps } from 'next'
import { useContext, useState } from 'react'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { actions as actionConstants } from '@constants/actions'
import classNames from 'classnames'
import {
    features,
    groupTabIds,
    layoutIds,
    routeParams,
} from '@constants/routes'
import { withUser } from '@helpers/hofs/withUser'
import { withRoutes } from '@helpers/hofs/withRoutes'
import { GetServerSidePropsContext } from '@appTypes/next'
import { withTextContent } from '@helpers/hofs/withTextContent'
import { withGroup } from '@helpers/hofs/withGroup'
import { formTypes } from '@constants/forms'
import {
    selectCsrfToken,
    selectFormData,
    selectParam,
    selectRequestMethod,
    selectUser,
    selectPageProps,
    selectPagination,
} from '@helpers/selectors/context'
import { User } from '@appTypes/user'
import { getGenericFormError, ServerSideFormData } from '@helpers/util/form'
import { requestMethods } from '@constants/fetch'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { notifications } from '@constants/notifications'
import { FormConfig, FormErrors } from '@appTypes/form'
import { FormWithErrorSummary } from '@components/forms/FormWithErrorSummary'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { NotificationsContext } from '@helpers/contexts/index'
import { useFormConfig } from '@helpers/hooks/useForm'
import { useNotification } from '@helpers/hooks/useNotification'
import { GroupPage } from '@appTypes/page'
import { postGroupUserInvite } from '@services/postGroupUserInvite'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { getFeatureEnabled } from '@services/getFeatureEnabled'
import { GenericPageTextContent } from '@appTypes/content'
import { DynamicListContainer } from '@components/layouts/DynamicListContainer'
import { DataGrid } from '@components/layouts/DataGrid'
import { getPendingGroupMembers } from '@services/getPendingGroupMembers'
import { PaginationWithStatus } from '@components/generic/PaginationWithStatus'
import { Pagination } from '@appTypes/pagination'

declare interface ContentText extends GenericPageTextContent {
    noPendingInvites: string
}

export interface Props extends GroupPage {
    noUsers: string
    contentText: ContentText
    pendingList: Array<any>
}

/**
 * Group member invite template
 */
export const GroupMemberInvitePage: (props: Props) => JSX.Element = ({
    csrfToken,
    forms,
    user,
    contentText,
    groupId,
    pagination,
    pendingList,
}) => {
    const formConfig: FormConfig = useFormConfig(
        formTypes.INVITE_USER,
        forms[formTypes.INVITE_USER]
    )
    const [dynamicPendingList, setPendingList] = useState(pendingList)
    const [dynamicPagination, setPagination] = useState(pagination)

    const [errors, setErrors] = useState(formConfig?.errors)
    const notificationsContext: any = useContext(NotificationsContext)

    const { secondaryHeading, noPendingInvites } = contentText

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
            children: `Cancel`,
            className: 'tablet:u-text-right',
        },
    ]

    const rowList = dynamicPendingList?.map(({ email, rowVersion }) => {
        const generatedCellClasses = {
            domain: classNames({
                ['u-justify-between u-w-full tablet:u-w-1/4 o-truncated-text-lines-1']:
                    true,
            }),
        }

        const generatedHeaderCellClasses = {
            domain: classNames({
                ['u-text-bold']: true,
            }),
        }

        const rows = [
            {
                children: <span>{email}</span>,
                className: generatedCellClasses.domain,
                headerClassName: generatedHeaderCellClasses.domain,
            },
            {
                children: (
                    // <ClickLink
                    //     onClick={() => {
                    //         setDomainToDelete({
                    //             domain,
                    //             id,
                    //             rowVersion,
                    //         })
                    //     }}
                    //     text={{
                    //         body: 'Delete',
                    //         ariaLabel: `Delete domain ${domain}`,
                    //     }}
                    //     iconName="icon-delete"
                    // />
                    <span>test</span>
                ),
                className: 'u-w-full tablet:u-w-1/8 tablet:u-text-right',
                headerClassName: 'u-hidden',
            },
        ]

        return rows
    })

    const hasPendingInvites = !!dynamicPendingList?.length

    /**
     * Client-side submission handler - TODO: Pending API
     */
    const handleSubmit = async (formData: FormData): Promise<FormErrors> => {
        const headers = getStandardServiceHeaders({
            csrfToken,
            accessToken: user.accessToken,
        })
        try {
            await postGroupUserInvite({
                user,
                headers,
                body: formData as any,
                groupId,
            })

            const emailAddress: FormDataEntryValue = formData.get('Email')
            useNotification({
                notificationsContext,
                text: {
                    heading: notifications.SUCCESS,
                    body: `Invite sent to ${emailAddress}`,
                },
            })

            return Promise.resolve({})
        } catch (error) {
            const errors: FormErrors =
                getServiceErrorDataValidationErrors(error) ||
                getGenericFormError(error)

            setErrors(errors)

            return Promise.resolve(errors)
        }
    }

    /**
     * Handle client-side pagination
     */
    const handleGetPage = async ({
        pageNumber: requestedPageNumber,
        pageSize: requestedPageSize,
    }) => {
        const { data: additionalPending, pagination } =
            await getPendingGroupMembers({
                user: user,
                groupId: groupId,
                pagination: {
                    pageNumber: requestedPageNumber,
                    pageSize: requestedPageSize,
                },
            })

        setPendingList([...dynamicPendingList, ...additionalPending])
        setPagination(pagination)
    }

    /**
     * Render
     */
    return (
        <LayoutColumn className="c-page-body">
            <LayoutColumnContainer>
                <LayoutColumn tablet={8}>
                    <FormWithErrorSummary
                        csrfToken={csrfToken}
                        formConfig={formConfig}
                        errors={errors}
                        text={{
                            form: {
                                submitButton: 'Send invite',
                            },
                        }}
                        submitAction={handleSubmit}
                        shouldClearOnSubmitSuccess={true}
                    >
                        <h2 className="nhsuk-heading-l">{secondaryHeading}</h2>
                    </FormWithErrorSummary>
                </LayoutColumn>
            </LayoutColumnContainer>
            <LayoutColumnContainer className="c-page-body">
                <h2 className="nhsuk-heading-l">{secondaryHeading}</h2>
                {hasPendingInvites ? (
                    <DynamicListContainer
                        containerElementType="div"
                        shouldEnableLoadMore
                        className="u-list-none u-p-0"
                    >
                        <DataGrid
                            id="admin-table-domains"
                            columnList={columnList}
                            rowList={rowList}
                            text={{
                                caption: 'Allowed domains',
                            }}
                        />
                    </DynamicListContainer>
                ) : (
                    <p>{noPendingInvites}</p>
                )}
                <PaginationWithStatus
                    id="group-list-pagination"
                    shouldEnableLoadMore
                    getPageAction={handleGetPage}
                    {...dynamicPagination}
                />
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
            const pagination: Pagination = {
                pageNumber: selectPagination(context).pageNumber ?? 1,
                pageSize: selectPagination(context).pageSize ?? 20,
            }

            try {
                const result = await getPendingGroupMembers({
                    user: user,
                    slug: props.groupId,
                    pagination,
                })

                props.pagination = result.pagination

                props.pendingList = result.data
            } catch (e) {}

            /**
             * Handle server-side form post
             */
            if (formData && requestMethod === requestMethods.POST) {
                props.forms[formTypes.INVITE_USER].initialValues = formData

                try {
                    const emailAddress: string = formData.get('Email')
                    props.notifications = [
                        {
                            heading: notifications.SUCCESS,
                            main: `Invite sent to ${emailAddress}`,
                        },
                    ]

                    return {
                        props: props,
                    }
                } catch (error) {
                    const validationErrors: FormErrors =
                        getServiceErrorDataValidationErrors(error)

                    if (validationErrors) {
                        props.forms[formTypes.INVITE_USER].errors =
                            validationErrors
                    } else {
                        return handleSSRErrorProps({ props, error })
                    }
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
export default GroupMemberInvitePage
