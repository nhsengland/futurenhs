import { SVGIcon } from '@components/generic/SVGIcon'
import classNames from 'classnames'
import {
    mdiEarth,
    mdiGlobeLightOutline,
    mdiGlobeModel,
    mdiLockOutline,
} from '@mdi/js'
import React from 'react'

type Props = {
    isPublic: boolean
    isHeader?: boolean
}

function GroupPrivacy({ isPublic, isHeader = false }: Props) {
    const textColour = `${isHeader ? 'u-text-theme-1' : 'u-text-theme-7'}`
    const text = `${isPublic ? 'Public' : 'Private'} group`
    const iconPath = isPublic ? mdiEarth : mdiLockOutline
    const generatedClasses: any = {
        wrapper: classNames(textColour, 'c-group-privacy'),
        header: classNames(textColour, 'u-d-inline', 'c-group-privacy__header'),
        icon: classNames(`c-group-privacy__icon`),
    }

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
