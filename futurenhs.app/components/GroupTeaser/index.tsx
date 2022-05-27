import classNames from 'classnames'
import Link from 'next/link'

import { defaultGroupLogos } from '@constants/icons'
import { Card } from '@components/Card'
import { SVGIcon } from '@components/SVGIcon'
import { useTheme } from '@hooks/useTheme'

import { Props } from './interfaces'

export const GroupTeaser: (props: Props) => JSX.Element = ({
    image,
    text,
    groupId,
    themeId,
    totalDiscussionCount,
    totalMemberCount,
    className,
}) => {
    const { mainHeading, strapLine } = text ?? {}

    const imageToUse = image ? image : defaultGroupLogos.large
    const cardLinkHref: string = `/groups/${groupId}`
    const themeBorderId: number = useTheme(themeId).background

    const generatedClasses: any = {
        wrapper: classNames(
            'u-mb-4',
            `u-border-b-theme-${themeBorderId}`,
            `hover:u-border-b-theme-${themeBorderId}-darker`,
            className
        ),
    }

    return (
        <Card
            id={`group-${groupId}`}
            image={imageToUse}
            clickableHref={cardLinkHref}
            className={generatedClasses.wrapper}
        >
            <h3 className="c-card_heading o-truncated-text-lines-3">
                <Link href={cardLinkHref}>
                    <a>{mainHeading}</a>
                </Link>
            </h3>
            <div className="c-card_body">
                <p className="c-card_content u-text-theme-7 o-truncated-text-lines-2">
                    {strapLine}
                </p>
                <div className="c-card_footer u-text-theme-0">
                    <p className="c-card_footer-item">
                        <SVGIcon
                            name="icon-member"
                            className="c-card_footer-icon u-fill-theme-0"
                        />
                        <span>{`Members: ${totalMemberCount}`}</span>
                    </p>
                    <p className="c-card_footer-item">
                        <SVGIcon
                            name="icon-discussion"
                            className="c-card_footer-icon u-fill-theme-0"
                        />
                        <span>{`Discussions: ${totalDiscussionCount}`}</span>
                    </p>
                </div>
            </div>
        </Card>
    )
}
