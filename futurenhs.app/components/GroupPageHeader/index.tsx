import { useRouter } from 'next/router'
import { useState, useEffect } from 'react'
import classNames from 'classnames'

import { Link } from '@components/Link'
import { cacheNames } from '@constants/caches'
import { clearClientCaches } from '@helpers/util/data'
import { useTheme } from '@hooks/useTheme'
import { actions as actionsConstants } from '@constants/actions'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { Dialog } from '@components/Dialog'
import { LayoutColumn } from '@components/LayoutColumn'
import { RichText } from '@components/RichText'
import { Image } from '@components/Image'
import { SVGIcon } from '@components/SVGIcon'
import { Accordion } from '@components/Accordion'
import { TabbedNav } from '@components/TabbedNav'
import { mediaQueries } from '@constants/css'
import { iconNames } from '@constants/icons'
import { useMediaQuery } from '@hooks/useMediaQuery'
import { Theme } from '@appTypes/theme'

import { Props } from './interfaces'
import { groupMemberStatus } from '@constants/group-member-status'

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
}) => {
    const router = useRouter()

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
            `u-text-theme-${content}`
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

        // if (
        //     actions?.includes(actionsConstants.SITE_ADMIN_GROUPS_EDIT) ||
        //     actions?.includes(actionsConstants.GROUPS_EDIT)
        // ) {
        //     actionsMenuList.push({
        //         id: actionsConstants.GROUPS_MEMBERS_ADD,
        //         url: `${routes.groupRoot}?${queryParams.EDIT}=true`,
        //         text: 'Page manager',
        //     })
        // }

        // if (actions?.includes(actionsConstants.GROUPS_MEMBERS_ADD)) {

        //     actionsMenuList.push({
        //         id: actionsConstants.GROUPS_MEMBERS_ADD,
        //         url: '/',
        //         text: 'Add new member'
        //     });

        // }

        // if (actions?.includes(actionsConstants.SITE_MEMBERS_ADD)) {

        //     actionsMenuList.push({
        //         id: actionsConstants.SITE_MEMBERS_ADD,
        //         url: '/',
        //         text: 'Invite new user'
        //     });

        // }

        return actionsMenuList
    }

    const handleLeaveGroup = (): any => setIsLeaveGroupModalOpen(true)
    const handleLeaveGroupCancel = () => setIsLeaveGroupModalOpen(false)
    const handleLeaveGroupConfirm = () => {
        clearClientCaches([cacheNames.NEXT_DATA]).then(() => {
            router.push(routes.groupLeave)
            setIsLeaveGroupModalOpen(false)
        })
    }

    const handleJoinGroup = (event: any) => {
        event.preventDefault()

        clearClientCaches([cacheNames.NEXT_DATA]).then(() => {
            router.push(routes.groupJoin)
        })
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
                        className="u-flex u-flex-wrap tablet:u-block"
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
                        <h1 className={generatedClasses.heading}>
                            {mainHeading}
                        </h1>
                        {description && (
                            <RichText
                                className={generatedClasses.description}
                                wrapperElementType="p"
                                bodyHtml={description}
                            />
                        )}
                    </LayoutColumn>
                    {shouldRenderActionsMenu && (
                        <LayoutColumn
                            tablet={4}
                            desktop={3}
                            className={generatedClasses.actionsWrapper}
                        >
                            {shouldRenderPendingMessage &&
                                !shouldRenderGroupJoinLink && (
                                    <div className="nhsuk-body-m">
                                        <SVGIcon
                                            name="icon-clock"
                                            className="u-w-4 u-h-4 u-mr-2"
                                        />
                                        Awaiting Approval
                                    </div>
                                )}

                            {shouldRenderGroupJoinLink ? (
                                <a
                                    href={routes.groupJoin}
                                    onClick={handleJoinGroup}
                                    className="c-button u-w-full u-border-2 u-border-theme-1"
                                >
                                    Join Group
                                </a>
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
                                                const handleActionMenuItemClick =
                                                    (event: any) => {
                                                        if (
                                                            id ===
                                                            actionsConstants.GROUPS_LEAVE
                                                        ) {
                                                            event.preventDefault()

                                                            handleLeaveGroup()
                                                        }
                                                    }

                                                return (
                                                    <li
                                                        key={index}
                                                        className="u-m-0"
                                                    >
                                                        <Link href={url}>
                                                            <a
                                                                className="c-page-header_actions-content-item u-m-0 u-block u-break-words"
                                                                onClick={
                                                                    handleActionMenuItemClick
                                                                }
                                                            >
                                                                {text}
                                                            </a>
                                                        </Link>
                                                        {id ===
                                                            actionsConstants.GROUPS_LEAVE && (
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
                                                                    Are you sure
                                                                    you would
                                                                    like to
                                                                    leave the
                                                                    group?
                                                                </p>
                                                            </Dialog>
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
