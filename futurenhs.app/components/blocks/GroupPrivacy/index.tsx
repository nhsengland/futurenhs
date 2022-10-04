import { SVGIcon } from '@components/generic/SVGIcon'
import classNames from 'classnames'
import { mdiLockOutline } from '@mdi/js'
import React from 'react'

type Props = {
    isPublic: boolean
}

function GroupPrivacy({ isPublic }: Props) {
    const generatedClasses: any = {
        wrapper: classNames('u-text-theme-7', 'c-group-privacy'),
        header: classNames(
            'u-text-theme-7',
            'u-d-inline',
            'c-group-privacy__header'
        ),
        icon: classNames('c-group-privacy__icon'),
    }

    const text = `${isPublic ? 'Public' : 'Private'} group`
    const iconPath = isPublic ? null : mdiLockOutline
    return (
        <div className={generatedClasses.wrapper}>
            <SVGIcon
                name={iconPath}
                material
                className={generatedClasses.icon}
            />
            <h6 className={generatedClasses.header}>{text}</h6>
        </div>
    )
}

export default GroupPrivacy
