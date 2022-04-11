import React from 'react'
import classNames from 'classnames'

import { initials } from '@helpers/formatters/initials'
import { Avatar } from '@components/Avatar'
import { Heading } from '@components/Heading'

import { Props } from './interfaces'

export const UserProfile: (props: Props) => JSX.Element = ({
    profile,
    text,
    image,
    children,
    className,
    headingLevel = 2,
}) => {
    const { firstName, lastName, pronouns, email } = profile ?? {}

    const {
        heading,
        firstNameLabel,
        lastNameLabel,
        pronounsLabel,
        emailLabel,
    } = text

    const userInitials: string = initials({ value: `${firstName} ${lastName}` })

    const generatedClasses: any = {
        wrapper: classNames('c-profile', className),
        image: classNames('c-profile_image'),
        heading: classNames('c-profile_heading'),
        data: classNames('c-profile_data'),
        label: classNames('c-profile_data-label', 'u-text-bold'),
        value: classNames('c-profile_data-value'),
    }

    return (
        <div className={generatedClasses.wrapper}>
            <Heading level={headingLevel} className={generatedClasses.heading}>
                {heading}
            </Heading>
            <Avatar
                image={image}
                initials={userInitials}
                className={generatedClasses.image}
            />
            <dl className={generatedClasses.data}>
                {firstName && (
                    <>
                        <dt className={generatedClasses.label}>
                            {firstNameLabel}
                        </dt>
                        <dd className={generatedClasses.value}>{firstName}</dd>
                    </>
                )}
                {lastName && (
                    <>
                        <dt className={generatedClasses.label}>
                            {lastNameLabel}
                        </dt>
                        <dd className={generatedClasses.value}>{lastName}</dd>
                    </>
                )}
                {pronouns && (
                    <>
                        <dt className={generatedClasses.label}>
                            {pronounsLabel}
                        </dt>
                        <dd className={generatedClasses.value}>{pronouns}</dd>
                    </>
                )}
                {email && (
                    <>
                        <dt className={generatedClasses.label}>{emailLabel}</dt>
                        <dd className={generatedClasses.value}>{email}</dd>
                    </>
                )}
            </dl>
            {children}
        </div>
    )
}
