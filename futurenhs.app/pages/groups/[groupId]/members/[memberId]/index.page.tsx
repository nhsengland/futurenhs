import { GetServerSideProps } from 'next'
import { useRef } from 'react'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { routeParams } from '@constants/routes'
import { layoutIds, groupTabIds } from '@constants/routes'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withGroup } from '@hofs/withGroup'
import { withTextContent } from '@hofs/withTextContent'
import { selectPageProps } from '@selectors/context'
import { getGroupMember } from '@services/getGroupMember'
import { selectUser, selectParam } from '@selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import { UserProfile } from '@components/UserProfile'
import { Image } from '@appTypes/image'
import { ActionLink } from '@components/ActionLink'
import { GroupPage } from '@appTypes/page'
import { GroupMember } from '@appTypes/group'

export interface Props extends GroupPage {
    member: GroupMember
}

/**
 * Group member template
 */
export const GroupMemberPage: (props: Props) => JSX.Element = ({
    contentText,
    member,
    routes,
}) => {
    const {
        secondaryHeading,
        firstNameLabel,
        lastNameLabel,
        pronounsLabel,
        emailLabel,
    } = contentText ?? {}

    const memberProfileImage: Image = member.image

    /**
     * Render
     */
    return (
        <LayoutColumn className="c-page-body">
            <ActionLink
                href={routes.groupMembersRoot}
                iconName="icon-chevron-left"
                className="u-mb-8"
                text={{
                    body: 'Back',
                    ariaLabel: 'Go back to list of group members',
                }}
            />
            <LayoutColumnContainer justify="centre">
                <LayoutColumn tablet={11}>
                    <UserProfile
                        image={memberProfileImage}
                        profile={member}
                        text={{
                            heading: secondaryHeading,
                            firstNameLabel: firstNameLabel,
                            lastNameLabel: lastNameLabel,
                            pronounsLabel: pronounsLabel,
                            emailLabel: emailLabel,
                        }}
                        className="tablet:u-justify-center tablet:u-mt-16"
                    ></UserProfile>
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
            routeId: '4502d395-7c37-4e80-92b7-65886de858ef',
        },
        [withUser, withRoutes, withGroup, withTextContent],
        async (context: GetServerSidePropsContext) => {
            /**
             * Get data from request context
             */
            const props: Partial<any> = selectPageProps(context)
            const user: User = selectUser(context)
            const groupId: string = selectParam(context, routeParams.GROUPID)
            const memberId: string = selectParam(context, routeParams.MEMBERID)

            /**
             * Get data from services
             */
            try {
                const [memberData] = await Promise.all([
                    getGroupMember({ groupId, user, memberId }),
                ])

                props.member = memberData.data
                props.layoutId = layoutIds.GROUP
                props.tabId = groupTabIds.MEMBERS
                props.pageTitle = `${props.entityText.title} - ${
                    props.member.firstName ?? ''
                } ${props.member.lastName ?? ''}`
            } catch (error) {
                return handleSSRErrorProps({ props, error })
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
export default GroupMemberPage
