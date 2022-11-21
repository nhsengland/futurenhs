import { useRouter } from 'next/router'
import { useState, useEffect } from 'react'
import classNames from 'classnames'

import { setFetchOpts, fetchJSON } from '@helpers/fetch'
import { Link } from '@components/generic/Link'
import { cacheNames } from '@constants/caches'
import { clearClientCaches } from '@helpers/util/data'
import { useTheme } from '@helpers/hooks/useTheme'
import { actions as actionsConstants } from '@constants/actions'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { Dialog } from '@components/generic/Dialog'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { RichText } from '@components/generic/RichText'
import { Image } from '@components/generic/Image'
import { SVGIcon } from '@components/generic/SVGIcon'
import { Accordion } from '@components/generic/Accordion'
import { TabbedNav } from '@components/blocks/TabbedNav'
import { mediaQueries } from '@constants/css'
import { iconNames } from '@constants/icons'
import { useMediaQuery } from '@helpers/hooks/useMediaQuery'
import { useCsrf } from '@helpers/hooks/useCsrf'
import { Theme } from '@appTypes/theme'

import { Props } from './interfaces'
import PrivateGroup from '@components/blocks/PrivateGroup'
import { groupMemberStatus } from '@constants/group-member-status'
import { requestMethods } from '@constants/fetch'
import { flags } from '@constants/feature-flags'

/**
 * Header for group listings and for individual groups
 */
export const GroupPageHeader: (props: Props) => JSX.Element = ({
    id,
    themeId,
    image,
    text,
    navMenuList,
    actions,
    routes,
    shouldRenderActionsMenu = true,
    memberStatus,
    className,
    isPublic,
    isDiscover,
    featureFlags,
}) => {
    const router = useRouter()

    const csrfToken: string = useCsrf()

    const [isActionsAccordionOpen] = useState(false)
    const [isMenuAccordionOpen, setIsMenuAccordionOpen] = useState(true)
    const [isLeaveGroupModalOpen, setIsLeaveGroupModalOpen] = useState(false)

    const actionsMenuTitleText: string = 'Actions'
    const { mainHeading, description, navMenuTitle } = text ?? {}

    const hasMenuItems: boolean = navMenuList?.length > 0
    const isMobile: boolean = useMediaQuery(mediaQueries.MOBILE)
    const isDesktop: boolean = useMediaQuery(mediaQueries.DESKTOP)
    const shouldRenderGroupJoinLink: boolean = actions?.includes(
        actionsConstants.GROUPS_JOIN
    )
    const shouldRenderGroupLeaveLink: boolean = actions?.includes(
        actionsConstants.GROUPS_LEAVE
    )
    const shouldRenderGroupEditLink: boolean = actions?.includes(
        actionsConstants.GROUPS_EDIT
    )

    const inviteEnabled = featureFlags?.some(
        (f) => f.id === flags.groupInvite && f.enabled
    )

    const hasInvitePermission = actions?.includes(
        actionsConstants.GROUPS_MEMBERS_INVITE
    )

    const shouldRenderGroupInviteLink: boolean =
        inviteEnabled && hasInvitePermission

    const shouldRenderPendingMessage: boolean =
        memberStatus === groupMemberStatus.PENDING

    const activeMenuItemText: string = navMenuList?.find(
        ({ isActive }) => isActive
    )?.text
    const { background, content }: Theme = shouldRenderActionsMenu
        ? useTheme(themeId)
        : {
              background: 14,
              content: 1,
              accent: 1,
          }

    const generatedIds: any = {
        actionsAccordion: `${id}-actions`,
        menuAccordion: `${id}-menu`,
    }

    const generatedClasses: any = {
        wrapper: classNames(
            'c-page-header',
            `u-bg-theme-${background}`,
            `u-text-theme-${content}`,
            className
        ),
        header: classNames('c-page-header_header'),
        heading: classNames(
            'c-page-header_heading',
            `u-text-theme-${content}`,
            'o-truncated-text-lines-3'
        ),
        hero: classNames('c-page-header_hero'),
        heroBody: classNames('c-page-header_hero-body'),
        description: classNames(
            'c-page-header_description',
            'tablet:o-truncated-text-lines-2'
        ),
        actionsWrapper: classNames('u-self-end', 'u-mt-8'),
        actions: classNames('c-page-header_actions', 'u-relative'),
        actionsTrigger: classNames('c-page-header_actions-trigger'),
        actionsTriggerIcon: classNames('c-page-header_actions-trigger-icon'),
        actionsContent: classNames(
            'c-page-header_actions-content',
            'u-list-none',
            'u-pt-1.5'
        ),
        navTrigger: classNames('c-page-header_nav-trigger'),
        navTriggerIcon: classNames('c-page-header_nav-trigger-icon'),
        navContent: classNames('c-page-header_nav-content'),
    }

    const getActionNavMenuList = (): Array<{
        id: actionsConstants
        url: string
        text: string
    }> => {
        const actionsMenuList = []

        if (shouldRenderGroupEditLink) {
            actionsMenuList.push({
                id: actionsConstants.GROUPS_EDIT,
                url: routes.groupUpdate,
                text: 'Edit group information',
            })
        }

        if (shouldRenderGroupLeaveLink) {
            actionsMenuList.push({
                id: actionsConstants.GROUPS_LEAVE,
                url: routes.groupLeave,
                text: 'Leave group',
            })
        }

        if (shouldRenderGroupInviteLink) {
            actionsMenuList.push({
                id: actionsConstants.GROUPS_MEMBERS_INVITE,
                url: routes.groupInvite,
                text: 'Invite',
            })
        }

        return actionsMenuList
    }

    const handleLeaveGroup = (event: any): any => {
        event.preventDefault()
        setIsLeaveGroupModalOpen(true)
    }
    const handleLeaveGroupCancel = () => setIsLeaveGroupModalOpen(false)
    const handleLeaveGroupConfirm = async () => {
        setIsLeaveGroupModalOpen(false)
        await clearClientCaches([cacheNames.NEXT_DATA])
        await fetchJSON(
            routes.groupLeave,
            setFetchOpts({
                method: requestMethods.POST,
                body: {
                    ['_csrf']: csrfToken,
                },
            }),
            30000
        )
        router.reload()
    }

    const handleJoinGroup = async (event: any) => {
        event.preventDefault()
        await clearClientCaches([cacheNames.NEXT_DATA])
        await fetchJSON(
            routes.groupJoin,
            setFetchOpts({
                method: requestMethods.POST,
                body: {
                    ['_csrf']: csrfToken,
                },
            }),
            30000
        )
        router.push(routes.groupRoot)
    }

    useEffect(() => {
        setIsMenuAccordionOpen(isDesktop)
    }, [isDesktop])

    return (
        <div className={generatedClasses.wrapper}>
            <LayoutColumn>
                <LayoutColumnContainer className={generatedClasses.header}>
                    <LayoutColumn
                        tablet={8}
                        desktop={9}
                        className="u-flex u-flex-column "
                    >
                        {image && (
                            <div className={generatedClasses.hero}>
                                <div className={generatedClasses.heroBody}>
                                    <Image
                                        src={image.src}
                                        alt={image.altText}
                                        height={image.height}
                                        width={image.width}
                                    />
                                </div>
                            </div>
                        )}
                        <div>
                            <h1 className={generatedClasses.heading}>
                                {mainHeading}
                            </h1>
                            {isDiscover ? (
                                <RichText
                                    className={generatedClasses.description}
                                    wrapperElementType="p"
                                    bodyHtml={description}
                                />
                            ) : !isPublic ? (
                                <PrivateGroup isHeader />
                            ) : null}
                        </div>
                    </LayoutColumn>
                    {shouldRenderActionsMenu && (
                        <LayoutColumn
                            tablet={4}
                            desktop={3}
                            className={generatedClasses.actionsWrapper}
                        >
                            {shouldRenderPendingMessage &&
                                !shouldRenderGroupJoinLink && (
                                    <div className="nhsuk-heading-s u-flex u-justify-end u-items-center u-mb-0">
                                        <SVGIcon
                                            name="icon-clock-bold"
                                            className="u-w-10 u-h-10 u-mr-2 u-fill-theme-1"
                                        />
                                        <p className="u-mb-0">
                                            Awaiting Approval
                                        </p>
                                    </div>
                                )}

                            {shouldRenderGroupJoinLink ? (
                                <form
                                    action={routes.groupJoin}
                                    method={requestMethods.POST}
                                    encType="multipart/form-data"
                                    onSubmit={handleJoinGroup}
                                >
                                    <input
                                        type="hidden"
                                        name="_csrf"
                                        value={csrfToken}
                                    />
                                    <button
                                        type="submit"
                                        className="c-button u-w-full u-border-2 u-border-theme-1"
                                    >
                                        Join group
                                    </button>
                                </form>
                            ) : getActionNavMenuList().length > 0 ? (
                                <Accordion
                                    id={generatedIds.actionsAccordion}
                                    isOpen={isActionsAccordionOpen}
                                    shouldCloseOnLeave={true}
                                    shouldCloseOnContentClick={true}
                                    toggleClassName={
                                        generatedClasses.actionsTrigger
                                    }
                                    toggleOpenChildren={
                                        <>
                                            {actionsMenuTitleText}
                                            <SVGIcon
                                                name={iconNames.CHEVRON_UP}
                                                className={
                                                    generatedClasses.actionsTriggerIcon
                                                }
                                            />
                                        </>
                                    }
                                    toggleClosedChildren={
                                        <>
                                            {actionsMenuTitleText}
                                            <SVGIcon
                                                name={iconNames.CHEVRON_DOWN}
                                                className={
                                                    generatedClasses.actionsTriggerIcon
                                                }
                                            />
                                        </>
                                    }
                                    className={generatedClasses.actions}
                                >
                                    <ul
                                        className={
                                            generatedClasses.actionsContent
                                        }
                                    >
                                        {getActionNavMenuList().map(
                                            ({ id, url, text }, index) => {
                                                return (
                                                    <li
                                                        key={index}
                                                        className="u-m-0"
                                                    >
                                                        {id ===
                                                        actionsConstants.GROUPS_LEAVE ? (
                                                            <>
                                                                <form
                                                                    action={
                                                                        routes.groupLeave
                                                                    }
                                                                    method={
                                                                        requestMethods.POST
                                                                    }
                                                                    encType="multipart/form-data"
                                                                    onSubmit={
                                                                        handleLeaveGroup
                                                                    }
                                                                >
                                                                    <input
                                                                        type="hidden"
                                                                        name="_csrf"
                                                                        value={
                                                                            csrfToken
                                                                        }
                                                                    />
                                                                    <button
                                                                        type="submit"
                                                                        className="c-page-header_actions-content-item u-mb-0 u-block u-break-words"
                                                                    >
                                                                        Leave
                                                                        group
                                                                    </button>
                                                                </form>
                                                                <Dialog
                                                                    id="dialog-leave-group"
                                                                    isOpen={
                                                                        isLeaveGroupModalOpen
                                                                    }
                                                                    text={{
                                                                        cancelButton:
                                                                            'Cancel',
                                                                        confirmButton:
                                                                            'Yes, leave group',
                                                                        heading:
                                                                            'Leave this group',
                                                                    }}
                                                                    cancelAction={
                                                                        handleLeaveGroupCancel
                                                                    }
                                                                    confirmAction={
                                                                        handleLeaveGroupConfirm
                                                                    }
                                                                >
                                                                    <p className="u-text-bold">
                                                                        Are you
                                                                        sure you
                                                                        would
                                                                        like to
                                                                        leave
                                                                        the
                                                                        group?
                                                                    </p>
                                                                </Dialog>
                                                            </>
                                                        ) : (
                                                            <Link href={url}>
                                                                <a className="c-page-header_actions-content-item u-m-0 u-block u-break-words">
                                                                    {text}
                                                                </a>
                                                            </Link>
                                                        )}
                                                    </li>
                                                )
                                            }
                                        )}
                                    </ul>
                                </Accordion>
                            ) : null}
                        </LayoutColumn>
                    )}
                </LayoutColumnContainer>
                {hasMenuItems && (
                    <Accordion
                        id={generatedIds.menuAccordion}
                        isOpen={isMenuAccordionOpen}
                        shouldCloseOnLeave={isMobile}
                        shouldCloseOnContentClick={isMobile}
                        toggleClassName={generatedClasses.navTrigger}
                        toggleOpenChildren={
                            <>
                                {activeMenuItemText}
                                <SVGIcon
                                    name={iconNames.CHEVRON_UP}
                                    className={generatedClasses.navTriggerIcon}
                                />
                            </>
                        }
                        toggleClosedChildren={
                            <>
                                {activeMenuItemText}
                                <SVGIcon
                                    name={iconNames.CHEVRON_DOWN}
                                    className={generatedClasses.navTriggerIcon}
                                />
                            </>
                        }
                        className={generatedClasses.navContent}
                    >
                        <TabbedNav
                            shouldNoneRole={true}
                            text={{
                                ariaLabel: navMenuTitle,
                            }}
                            navMenuList={navMenuList}
                            shouldFocusActiveLink={isDesktop}
                        />
                    </Accordion>
                )}
            </LayoutColumn>
        </div>
    )
}
