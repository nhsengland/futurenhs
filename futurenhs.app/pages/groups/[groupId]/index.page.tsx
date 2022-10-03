import { GetServerSideProps } from 'next'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { layoutIds, groupTabIds, routeParams } from '@constants/routes'
import { withUser } from '@helpers/hofs/withUser'
import { withGroup } from '@helpers/hofs/withGroup'
import { withRoutes } from '@helpers/hofs/withRoutes'
import { withTextContent } from '@helpers/hofs/withTextContent'
import { User } from '@appTypes/user'
import {
    selectUser,
    selectParam,
    selectPageProps,
} from '@helpers/selectors/context'
import { getGroupHomePageCmsContentIds } from '@services/getGroupHomePageCmsContentIds'
import { getCmsPageTemplate } from '@services/getCmsPageTemplate'
import { getCmsPageContent } from '@services/getCmsPageContent'
import { GetServerSidePropsContext } from '@appTypes/next'
import { useState, useRef, useContext } from 'react'
import classNames from 'classnames'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { getGenericFormError } from '@helpers/util/form'
import { actions as actionConstants } from '@constants/actions'
import { ErrorSummary } from '@components/generic/ErrorSummary'
import { ContentBlockManager } from '@components/blocks/ContentBlockManager'
import { WarningCallout } from '@components/generic/WarningCallout'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { putCmsPageContent } from '@services/putCmsPageContent'
import { postCmsPageContent } from '@services/postCmsPageContent'
import { postCmsBlock } from '@services/postCmsBlock'
import { FormErrors } from '@appTypes/form'
import { CmsContentBlock } from '@appTypes/contentBlock'
import { useNotification } from '@helpers/hooks/useNotification'
import { notifications } from '@constants/notifications'
import { NotificationsContext } from '@helpers/contexts/index'
import { GroupPage } from '@appTypes/page'

export interface Props extends GroupPage {}

export const GroupHomePage: (props: Props) => JSX.Element = ({
    user,
    actions,
    contentPageId,
    contentTemplate,
    contentBlocks,
    themeId,
}) => {
    const notificationsContext: any = useContext(NotificationsContext)
    const errorSummaryRef: any = useRef()

    const [blocks, setBlocks] = useState(contentBlocks ?? [])
    const [template, setTemplate] = useState(contentTemplate ?? [])
    const [errors, setErrors] = useState({})

    const isGroupAdmin: boolean =
        actions.includes(actionConstants.GROUPS_EDIT) ||
        actions.includes(actionConstants.SITE_ADMIN_GROUPS_EDIT)

    const generatedClasses: any = {
        wrapper: classNames('c-page-body'),
    }

    const handleClearErrors = () => setErrors({})
    const handleStateChange = () => {
        handleClearErrors()
        useNotification({
            notificationsContext,
            shouldClearQueue: true,
        })
    }

    const handleCreateBlock = (
        blockContentTypeId: string,
        parentBlockId?: string
    ): Promise<string> => {
        return new Promise((resolve, reject) => {
            postCmsBlock({
                user,
                blockContentTypeId,
                parentBlockId,
                pageId: contentPageId,
            })
                .then((newBlock) => {
                    resolve(newBlock.data)
                })
                .catch((error) => {
                    const errors: FormErrors =
                        getServiceErrorDataValidationErrors(error) ||
                        getGenericFormError(error)

                    setErrors(errors)
                    window.scrollTo(0, 0)
                    errorSummaryRef?.current?.focus?.()

                    reject()
                })
        })
    }

    const handleSaveBlocks = (
        blocks: Array<CmsContentBlock>,
        localErrors: FormErrors
    ): Promise<FormErrors> => {
        return new Promise((resolve) => {
            if (Object.keys(localErrors).length) {
                setErrors(localErrors)
                window.scrollTo(0, 0)
                errorSummaryRef?.current?.focus?.()

                resolve(localErrors)
            } else {
                putCmsPageContent({
                    user,
                    pageId: contentPageId,
                    pageBlocks: blocks,
                })
                    .then(() => {
                        postCmsPageContent({
                            user,
                            pageId: contentPageId,
                        }).then(() => {
                            getCmsPageContent({
                                user,
                                pageId: contentPageId,
                            }).then((response) => {
                                const updatedBlocks: Array<CmsContentBlock> =
                                    response.data

                                setBlocks(updatedBlocks)
                                setErrors({})

                                useNotification({
                                    notificationsContext,
                                    text: {
                                        heading: notifications.SUCCESS,
                                        body: 'Your changes have been published',
                                    },
                                })

                                resolve({})
                            })
                        })
                    })
                    .catch((error) => {
                        const errors: FormErrors =
                            getServiceErrorDataValidationErrors(error) ||
                            getGenericFormError(error)

                        setErrors(errors)
                        window.scrollTo(0, 0)
                        errorSummaryRef?.current?.focus?.()

                        resolve(errors)
                    })
            }
        })
    }

    return (
        <LayoutColumn tablet={12} className={generatedClasses.wrapper}>
            <ErrorSummary
                ref={errorSummaryRef}
                errors={errors}
                className="u-mb-6"
            />
            {isGroupAdmin && (
                <noscript>
                    <WarningCallout
                        headingLevel={2}
                        text={{
                            heading: 'Important',
                            body: 'JavaScript must be enabled in your browser to manage the content of this page',
                        }}
                    />
                </noscript>
            )}
            <ContentBlockManager
                blocks={blocks}
                blocksTemplate={template}
                text={{
                    headerReadBody:
                        'You are a Group Admin of this page. Please click edit to switch to editing mode.',
                    headerPreviewBody:
                        'You are previewing the group homepage in editing mode.',
                    headerCreateHeading: 'Add content block',
                    headerCreateBody:
                        'Choose a content block to add to your group homepage',
                    headerUpdateHeading: 'Editing group homepage',
                    headerUpdateBody:
                        'Welcome to your group homepage. You are currently in editing mode. You can preview any changes you make to your page before you publish them. For more information and help, see our <a href="https://support-futurenhs.zendesk.com/hc/en-gb/articles/5820646602653">quick guide</a>.',
                    headerEnterUpdateButton: 'Edit page',
                    headerLeaveUpdateButton: 'Stop editing page',
                    headerDiscardUpdateButton: 'Discard updates',
                    headerPreviewUpdateButton: 'Preview page',
                    headerPublishUpdateButton: 'Publish group page',
                    createButton: 'Add content block',
                    cancelCreateButton: 'Cancel',
                }}
                shouldRenderEditingHeader={isGroupAdmin}
                discardUpdateAction={handleClearErrors}
                stateChangeAction={handleStateChange}
                blocksChangeAction={handleClearErrors}
                saveBlocksAction={handleSaveBlocks}
                createBlockAction={handleCreateBlock}
                themeId={themeId}
            />
        </LayoutColumn>
    )
}

export const getServerSideProps: GetServerSideProps = async (
    context: GetServerSidePropsContext
) =>
    await pipeSSRProps(
        context,
        {
            routeId: '7a9bdd18-45ea-4976-9810-2fcb66242e27',
        },
        [withUser, withRoutes, withGroup, withTextContent],
        async (context: GetServerSidePropsContext) => {
            const props: Partial<Props> = selectPageProps(context)
            const user: User = selectUser(context)
            const groupId: string = selectParam(context, routeParams.GROUPID)

            props.layoutId = layoutIds.GROUP
            props.tabId = groupTabIds.INDEX
            props.pageTitle = props.entityText?.title

            /**
             * Get data from services
             */
            try {
                const contentTemplateId: string =
                    '0b955a4a-9e26-43e8-bb4b-51010e264d64'
                const groupHomePageCmsContentIds =
                    await getGroupHomePageCmsContentIds({
                        user,
                        groupId,
                    })
                const contentPageId: string =
                    groupHomePageCmsContentIds.data.contentRootId

                const [contentBlocks, contentTemplate] = await Promise.all([
                    getCmsPageContent({
                        user,
                        pageId: contentPageId,
                        isPublished: true,
                    }),
                    getCmsPageTemplate({
                        user,
                        templateId: contentTemplateId,
                    }),
                ])

                ;(props.contentPageId = contentPageId),
                    (props.contentTemplateId = contentTemplateId)

                props.contentBlocks = contentBlocks.data
                props.contentTemplate = contentTemplate.data
            } catch (error) {
                return handleSSRErrorProps({
                    props,
                    error,
                    shouldSurface: false,
                })
            }

            /**
             * Return data to page template
             */
            return handleSSRSuccessProps({ props, context })
        }
    )

/**
 * Export page template
 */
export default GroupHomePage
