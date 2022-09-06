import { useState } from 'react'
import { useRouter } from 'next/router'
import classNames from 'classnames'

import { actions as actionConstants } from '@constants/actions'
import { initials } from '@helpers/formatters/initials'
import { dateTime } from '@helpers/formatters/dateTime'
import { Link } from '@components/Link'
import { DynamicListContainer } from '@components/DynamicListContainer'
import { UserMeta } from '@components/UserMeta'
import { LayoutColumn } from '@components/LayoutColumn'
import { PaginationWithStatus } from '@components/PaginationWithStatus'
import { Card } from '@components/Card'
import { SVGIcon } from '@components/SVGIcon'
import { getGroupDiscussions } from '@services/getGroupDiscussions'

import { Props } from './interfaces'
import { Image } from '@appTypes/image'

/**
 * Group forum discussion listing template
 */
export const GroupForumTemplate: (props: Props) => JSX.Element = ({
    groupId,
    user,
    contentText,
    pagination,
    actions,
    routes,
    discussionsList,
}) => {
    const router = useRouter()

    const [dynamicDiscussionsList, setDiscussionsList] =
        useState(discussionsList)
    const [dynamicPagination, setPagination] = useState(pagination)

    const hasDiscussions: boolean = dynamicDiscussionsList.length > 0
    const shouldEnableLoadMore: boolean = true
    const shouldRenderCreateDiscussionLink: boolean = actions.includes(
        actionConstants.GROUPS_DISCUSSIONS_ADD
    )

    const {
        discussionsHeading,
        noDiscussions,
        createDiscussion,
        createdByLabel,
        lastCommentLabel,
        stickyLabel,
    } = contentText ?? {}

    /**
     * Client-side list pagination
     */
    const handleGetPage = async ({
        pageNumber: requestedPageNumber,
        pageSize: requestedPageSize,
    }) => {
        try {
            const { data: additionalDiscussions, pagination } =
                await getGroupDiscussions({
                    user: user,
                    groupId: groupId,
                    pagination: {
                        pageNumber: requestedPageNumber,
                        pageSize: requestedPageSize,
                    },
                })

            setDiscussionsList([
                ...dynamicDiscussionsList,
                ...additionalDiscussions,
            ])
            setPagination(pagination)
        } catch (error) {
            console.log(error)
        }
    }

    const generatedClasses = {
        wrapper: classNames('u-w-full', 'u-flex', 'u-flex-col', {
            ['tablet:u-flex-row']: !shouldRenderCreateDiscussionLink,
            ['tablet:u-flex-row-reverse']: shouldRenderCreateDiscussionLink,
        }),
    }

    /**
     * Render
     */
    return (
        <>
            <div className={generatedClasses.wrapper}>
                {shouldRenderCreateDiscussionLink && (
                    <LayoutColumn tablet={4} className="c-page-body">
                        <Link href={`${router.asPath}/create`}>
                            <a className="c-button u-w-full">
                                {createDiscussion}
                            </a>
                        </Link>
                    </LayoutColumn>
                )}
                <LayoutColumn tablet={8} className="c-page-body">
                    <h2 className="nhsuk-heading-l">{discussionsHeading}</h2>
                    {hasDiscussions ? (
                        <DynamicListContainer
                            containerElementType="ul"
                            shouldEnableLoadMore={shouldEnableLoadMore}
                            className="u-list-none u-p-0"
                        >
                            {dynamicDiscussionsList?.map?.(
                                (
                                    {
                                        text,
                                        discussionId,
                                        viewCount,
                                        responseCount,
                                        created,
                                        createdBy,
                                        modified,
                                        modifiedBy,
                                        isSticky,
                                    },
                                    index
                                ) => {
                                    const { title } = text ?? {}
                                    const creatorUserInitials: string =
                                        initials({
                                            value: createdBy.text.userName,
                                        })
                                    const creatorUserName: string =
                                        createdBy.text.userName
                                    const profileImage: Image = createdBy.image
                                    const creatorUserId: string = createdBy.id
                                    const createdDate: string = dateTime({
                                        value: created,
                                    })
                                    const lastCommentUserId: string =
                                        modifiedBy?.id
                                    const lastCommentUserName: string =
                                        modifiedBy?.text.userName
                                    const lastCommentDate: string = dateTime({
                                        value: modified,
                                    })
                                    const cardLinkHref: string = `${routes.groupForumRoot}/${discussionId}`
                                    const hasLastCommentData: boolean =
                                        Boolean(lastCommentUserId) &&
                                        Boolean(lastCommentUserName) &&
                                        Boolean(lastCommentDate)
                                    const hasResponses: boolean =
                                        responseCount > 0

                                    return (
                                        <li key={index}>
                                            <Card
                                                id={`discussion-${discussionId}`}
                                                clickableHref={cardLinkHref}
                                                className="u-border-b-theme-10 hover:u-border-b-theme-10-darker u-mb-4"
                                            >
                                                <h3 className="c-card_heading_discussion-title desktop:u-mb-4">
                                                    <Link href={cardLinkHref}>
                                                        <a>
                                                            {isSticky && (
                                                                <span className="u-sr-only">
                                                                    {
                                                                        stickyLabel
                                                                    }{' '}
                                                                </span>
                                                            )}
                                                            {title}
                                                        </a>
                                                    </Link>
                                                </h3>
                                                <UserMeta
                                                    image={profileImage}
                                                    text={{
                                                        initials:
                                                            creatorUserInitials,
                                                    }}
                                                    className="u-text-theme-7"
                                                >
                                                    <span className="u-text-bold u-block">
                                                        {createdByLabel}{' '}
                                                        <Link
                                                            href={`${routes.groupMembersRoot}/${creatorUserId}`}
                                                        >
                                                            {creatorUserName}
                                                        </Link>{' '}
                                                        {createdDate}
                                                    </span>
                                                    {hasResponses &&
                                                        hasLastCommentData && (
                                                            <span className="u-block u-mt-1">
                                                                {
                                                                    lastCommentLabel
                                                                }{' '}
                                                                <Link
                                                                    href={`${routes.groupMembersRoot}/${lastCommentUserId}`}
                                                                >
                                                                    {
                                                                        lastCommentUserName
                                                                    }
                                                                </Link>{' '}
                                                                {
                                                                    lastCommentDate
                                                                }
                                                            </span>
                                                        )}
                                                </UserMeta>
                                                <div className="c-card_footer u-text-theme-0 u-flex u-justify-between">
                                                    <div className="u-flex u-flex-col tablet:u-flex-row">
                                                        <p className="c-card_footer-item">
                                                            <SVGIcon
                                                                name="icon-comments"
                                                                className="c-card_footer-icon u-fill-theme-0"
                                                            />
                                                            <span>{`Comments: ${responseCount}`}</span>
                                                        </p>
                                                        {/* <p className="c-card_footer-item">
                                                            <SVGIcon name="icon-view" className="c-card_footer-icon u-fill-theme-0" />
                                                            <span>{`Views: ${viewCount}`}</span>
                                                        </p> */}
                                                    </div>
                                                    {isSticky && (
                                                        <SVGIcon
                                                            name="icon-pin"
                                                            className="u-fill-theme-0 u-float-right u-w-4 u-h-4 u-mb-1 u-self-end"
                                                        />
                                                    )}
                                                </div>
                                            </Card>
                                        </li>
                                    )
                                }
                            )}
                        </DynamicListContainer>
                    ) : (
                        <p>{noDiscussions}</p>
                    )}
                    <PaginationWithStatus
                        id="group-list-pagination"
                        shouldEnableLoadMore={shouldEnableLoadMore}
                        getPageAction={handleGetPage}
                        {...dynamicPagination}
                    />
                </LayoutColumn>
            </div>
        </>
    )
}
