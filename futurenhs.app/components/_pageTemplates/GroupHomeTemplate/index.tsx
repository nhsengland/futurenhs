import { useState, useRef } from 'react';
import classNames from 'classnames';

import { getServiceErrorDataValidationErrors } from '@services/index';
import { getGenericFormError } from '@helpers/util/form';
import { actions as actionConstants } from '@constants/actions';
import { ErrorSummary } from '@components/ErrorSummary';
import { ContentBlockManager } from '@components/ContentBlockManager';
import { NoScript } from '@components/NoScript';
import { LayoutColumn } from '@components/LayoutColumn';
import { getCmsPageContent } from '@services/getCmsPageContent';
import { putCmsPageContent } from '@services/putCmsPageContent';
import { postCmsPageContent } from '@services/postCmsPageContent';
import { postCmsBlock } from '@services/postCmsBlock';
import { FormErrors } from '@appTypes/form';
import { CmsContentBlock } from '@appTypes/contentBlock';

import { Props } from './interfaces'

export const GroupHomeTemplate: (props: Props) => JSX.Element = ({
    user,
    actions,
    contentPageId,
    contentTemplate,
    contentBlocks,
    themeId
}) => {

    const errorSummaryRef: any = useRef()

    const [blocks, setBlocks] = useState(contentBlocks);
    const [errors, setErrors] = useState({});

    const isGroupAdmin: boolean = actions.includes(actionConstants.GROUPS_EDIT) || actions.includes(actionConstants.SITE_ADMIN_GROUPS_EDIT);

    const generatedClasses: any = {
        wrapper: classNames('c-page-body')
    };

    const handleClearErrors = () => {

        if (Object.keys(errors).length > 0) {

            setErrors({});

        }

    }

    const handleCreateBlock = (blockContentTypeId: string, parentBlockId?: string): Promise<string> => {

        return new Promise((resolve, reject) => {

            postCmsBlock({
                user,
                blockContentTypeId,
                parentBlockId,
                pageId: contentPageId,
            })
            .then((newBlock) => {

                resolve(newBlock.data);
                
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

    const handleSaveBlocks = (blocks: Array<CmsContentBlock>, localErrors: FormErrors): Promise<FormErrors> => {

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
                    pageBlocks: blocks
                })
                    .then(() => {

                        postCmsPageContent({
                            user,
                            pageId: contentPageId
                        })
                            .then(() => {

                                getCmsPageContent({
                                    user,
                                    pageId: contentPageId
                                })
                                    .then((response) => {

                                        const updatedBlocks: Array<CmsContentBlock> = response.data;

                                        setBlocks(updatedBlocks);
                                        setErrors({});
                                        resolve({})

                                    });

                            });

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

    };

    return (
        <LayoutColumn tablet={12} className={generatedClasses.wrapper}>
            <ErrorSummary
                ref={errorSummaryRef}
                errors={errors}
                className="u-mb-6" />
            {isGroupAdmin &&
                <NoScript headingLevel={2} text={{
                    heading: 'Important',
                    body: 'JavaScript must be enabled in your browser to manage the content of this page'
                }} />
            }
            <ContentBlockManager
                blocks={blocks}
                blocksTemplate={contentTemplate}
                text={{
                    headerReadBody: "You are a Group Admin of this page. Please click edit to switch to editing mode.",
                    headerPreviewBody: "You are previewing the group homepage in editing mode.",
                    headerCreateHeading: "Add content block",
                    headerCreateBody: "Choose a content block to add to your group homepage",
                    headerUpdateHeading: "Editing group homepage",
                    headerUpdateBody: "Welcome to your group homepage. You are currently in editing mode. You can save a draft at any time, preview your page, or publish your changes. Once published, you can edit your page in the group actions. For more information and help, see our quick guide. For some inspiration, visit our knowledge hub.",
                    headerEnterUpdateButton: "Edit page",
                    headerLeaveUpdateButton: "Stop editing page",
                    headerDiscardUpdateButton: "Discard updates",
                    headerPreviewUpdateButton: "Preview page",
                    headerPublishUpdateButton: "Publish group page",
                    createButton: "Add content block",
                    cancelCreateButton: "Cancel"
                }}
                shouldRenderEditingHeader={isGroupAdmin}
                discardUpdateAction={handleClearErrors}
                stateChangeAction={handleClearErrors}
                blocksChangeAction={handleClearErrors}
                saveBlocksAction={handleSaveBlocks}
                createBlockAction={handleCreateBlock}
                themeId={themeId} />
        </LayoutColumn>
    )
}