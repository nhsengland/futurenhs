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
import { deleteCmsPageContentDraft } from '@services/deleteCmsPageContentDraft'
import { FormErrors } from '@appTypes/form'
import { CmsContentBlock } from '@appTypes/cmsContent'
import { useNotification } from '@hooks/useNotification'
import { notifications } from '@constants/notifications'
import { NotificationsContext } from '@contexts/index'

import { Props } from './interfaces'
import { cprud } from '@constants/cprud'

export const GroupHomeTemplate: (props: Props) => JSX.Element = ({
    user,
    actions,
    contentPageId,
    contentTemplate,
    contentPage,
    contentPageDraft,
    themeId,
}) => {

    const isGroupAdmin: boolean =
    actions.includes(actionConstants.GROUPS_EDIT) ||
    actions.includes(actionConstants.SITE_ADMIN_GROUPS_EDIT)

    const isDraftPageLatest: () => boolean = () => {

        try {

            const draftEditedAtDate: Date = new Date(contentPageDraft.item.editedAt);
            const publishedEditedAtDate: Date = new Date(contentPage.item.editedAt);

            if(draftEditedAtDate > publishedEditedAtDate){

                return true;

            }

        } catch(error){

            return false    

        }

        return false

    };
    const publishedBlocks: Array<CmsContentBlock> = contentPage?.content?.blocks ?? [];
    const draftBlocks: Array<CmsContentBlock> = contentPageDraft?.content?.blocks ?? [];
    const getBlocks = (): Array<CmsContentBlock> => (isGroupAdmin && isDraftPageLatest()) ? draftBlocks : publishedBlocks;
    const initialState: cprud = isGroupAdmin && isDraftPageLatest() ? cprud.UPDATE : cprud.READ; 

    const errorSummaryRef: any = useRef()
    const notificationsContext: any = useContext(NotificationsContext);
    const [blocks, setBlocks] = useState(getBlocks() ?? [])
    const [template] = useState(contentTemplate ?? [])
    const [errors, setErrors] = useState({})

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
    const handleDiscardUpdate = async () => {

        handleClearErrors();

        await deleteCmsPageContentDraft({
            user,
            pageId: contentPageId
        })

    }
    
    const handleChangeBlocks = (blocks: Array<CmsContentBlock>) => {
        
        handleClearErrors();

        putCmsPageContent({
            user,
            pageId: contentPageId,
            pageBlocks: blocks,
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

    const handlePublishBlocks = (
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
                                    response.data.content.blocks

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
                initialState={initialState}
                activeBlocks={blocks}
                referenceBlocks={publishedBlocks}
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
                        'Welcome to your group homepage. You are currently in editing mode. Changes to your draft will be auto-saved. You can preview your page, or publish your changes at any time. Once published, you can edit your page in the group actions. For more information and help, see our quick guide. For some inspiration, visit our knowledge hub.',
                    headerEnterUpdateButton: 'Edit page',
                    headerLeaveUpdateButton: 'Stop editing page',
                    headerDiscardUpdateButton: 'Discard updates',
                    headerPreviewUpdateButton: 'Preview page',
                    headerPublishUpdateButton: 'Publish group page',
                    createButton: 'Add content block',
                    cancelCreateButton: 'Cancel',
                }}
                shouldRenderEditingHeader={isGroupAdmin}
                discardUpdateAction={handleDiscardUpdate}
                stateChangeAction={handleStateChange}
                createBlockAction={handleCreateBlock}
                changeBlocksAction={handleChangeBlocks}
                publishBlocksAction={handlePublishBlocks}
                themeId={themeId}
            />
        </LayoutColumn>
    )
}
