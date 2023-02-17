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

export interface Props extends GroupPage {}

/**
 * Group member invite template
 */
export const GroupMemberInvitePage: (props: Props) => JSX.Element = ({
    csrfToken,
    user,
    groupId,
}) => {
    const [validation, setValidation] = useState<Array<VMessage>>([])
    const [emails, setEmails] = useState<Array<string>>([])
    const notificationsContext: any = useContext(NotificationsContext)
    const onChange = (e) => {
        const el = e.target as HTMLInputElement
        const validation = validate(el.value, [VRules.EMAIL, VRules.REQUIRED])
        setValidation(validation)
    }
    const handleInvite = async () => {
        /**
         * TODO: This only sends the first email as an invite, project finished. Extend API to accept the full array of emails
         */
        if (!emails.length) return
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
