import classNames from 'classnames'
import Link from 'next/link'
import { defaultGroupLogos } from '@constants/icons'
import { Heading } from '@components/layouts/Heading'
import { Card } from '@components/generic/Card'
import { SVGIcon } from '@components/generic/SVGIcon'
import { useTheme } from '@helpers/hooks/useTheme'
import PrivateGroup from '@components/blocks/PrivateGroup'
import { Props } from './interfaces'
import { useCsrf } from '@helpers/hooks/useCsrf'
import { deleteGroupInvite } from '@services/deleteGroupInvite'
import { getStandardServiceHeaders } from '@helpers/fetch'

/**
 * Group teaser link card for use in group listings
 */
export const GroupTeaser: (props: Props) => JSX.Element = ({
    image,
    text,
    groupId,
    themeId,
    totalDiscussionCount,
    totalMemberCount,
    headingLevel = 3,
    className,
    isPublic,
    isSignUp,
    isPending,
    user,
}) => {
    const { mainHeading, strapLine } = text ?? {}
    const csrfToken: string = useCsrf()

    const imageToUse = image ? image : defaultGroupLogos.large
    const cardLinkHref: string = `/groups/${groupId}`
    const themeBorderId: number = useTheme(themeId).background

    const generatedClasses: any = {
        box: classNames('group-teaser'),
        wrapper: classNames(
            'u-mb-4',
            `u-border-b-theme-${themeBorderId}`,
            `hover:u-border-b-theme-${themeBorderId}-darker`,
            className
        ),
        button: classNames('group-teaser__button-decline'),
    }

    const handleDeclineGroup = async (e) => {
        e.preventDefault()
        try {
            if (!groupId || !user) return
            const headers = getStandardServiceHeaders({
                csrfToken,
            })
            await deleteGroupInvite({ headers, user, groupId })

            // const joinRes = await postGroupMembership({
            //     groupId,
            //     csrfToken,
            //     user,
            // })
            // console.log('JOIN TRIGGERED: ', joinRes)
            // // const headers = getStandardServiceHeaders({
            // //     csrfToken,
            // // })
            // // const leaveRes = await deleteGroupMembership({
            // //     groupId,
            // //     headers,
            // //     user,
            // // })
            // // console.log('LEAVE TRIGGERED: ', leaveRes)
        } catch (e) {}
    }

    return (
        <div className={generatedClasses.box}>
            {isPending ? (
                <span
                    className={generatedClasses.button}
                    onClick={(e) => {
                        console.log('CLICKING DECLINE')
                        handleDeclineGroup(e)
                    }}
                >
                    X
                </span>
            ) : null}
            <Card
                id={`group-${groupId}`}
                image={imageToUse}
                clickableHref={!isSignUp ? cardLinkHref : undefined}
                className={generatedClasses.wrapper}
            >
                <Heading
                    level={headingLevel}
                    className="c-card_heading o-truncated-text-lines-3"
                >
                    {!isSignUp ? (
                        <Link href={cardLinkHref}>
                            <a>{mainHeading}</a>
                        </Link>
                    ) : (
                        <a className="u-text-theme-0">{mainHeading}</a>
                    )}
                    {!isPublic ? <PrivateGroup /> : null}
                </Heading>
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
        </div>
    )
}
