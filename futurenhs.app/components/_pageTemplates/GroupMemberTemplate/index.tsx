import { useRef, useState } from 'react'

import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import { BackLink } from '@components/BackLink'
import { UserProfile } from '@components/UserProfile'

import { Props } from './interfaces'

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

    /**
     * Render
     */
    return (
        <LayoutColumn className="c-page-body">
            <BackLink
                href={routes.groupMembersRoot}
                text={{
                    link: 'Back',
                }}
            />
            <LayoutColumnContainer justify="centre">
                <LayoutColumn tablet={11}>
                    <UserProfile
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
