import { useState, useRef, useContext } from 'react'
import classNames from 'classnames'

import { getServiceErrorDataValidationErrors } from '@services/index'
import { getGenericFormError } from '@helpers/util/form'
import { actions as actionConstants } from '@constants/actions'
import { ErrorSummary } from '@components/ErrorSummary'
import { ContentBlockManager } from '@components/ContentBlockManager'
import { WarningCallout } from '@components/WarningCallout'
import { LayoutColumn } from '@components/LayoutColumn'
import { getCmsPageContent } from '@services/getCmsPageContent'
import { putCmsPageContent } from '@services/putCmsPageContent'
import { postCmsPageContent } from '@services/postCmsPageContent'
import { postCmsBlock } from '@services/postCmsBlock'
import { FormErrors } from '@appTypes/form'
import { CmsContentBlock } from '@appTypes/contentBlock'
import { useNotification } from '@hooks/useNotification'
import { notifications } from '@constants/notifications'
import { NotificationsContext } from '@contexts/index'

import { Props } from './interfaces'

export const GroupHomeTemplate: (props: Props) => JSX.Element = ({
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

        handleClearErrors();
        useNotification({
            notificationsContext, 
            shouldClearQueue: true
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
                                        body: "Your changes have been published"
                                    }
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
