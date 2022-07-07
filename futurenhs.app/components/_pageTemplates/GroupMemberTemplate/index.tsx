import { useRef, useState } from 'react'

import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import { UserProfile } from '@components/UserProfile'
import { Image } from '@appTypes/image'

import { Props } from './interfaces'
import { ActionLink } from '@components/ActionLink'

/**
 * Group member template
 */
export const GroupMemberTemplate: (props: Props) => JSX.Element = ({
    contentText,
    member,
    routes,
}) => {
    const errorSummaryRef: any = useRef()

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
