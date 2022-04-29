import { useState } from 'react';
import classNames from 'classnames';

import { getServiceErrorDataValidationErrors } from '@services/index';
import { getGenericFormError } from '@helpers/util/form';
import { useDynamicElementClassName } from '@hooks/useDynamicElementClassName';
import { actions as actionConstants } from '@constants/actions';
import { ContentBlockManager } from '@components/ContentBlockManager';
import { NoScript } from '@components/NoScript';
import { LayoutColumn } from '@components/LayoutColumn';
import { getCmsPageContent } from '@services/getCmsPageContent';
import { putCmsPageContent } from '@services/putCmsPageContent';
import { postCmsPageContent } from '@services/postCmsPageContent';

import { Props } from './interfaces';
import { CmsContentBlock } from '@appTypes/contentBlock';
import { FormErrors } from '@appTypes/form';

export const GroupHomeTemplate: (props: Props) => JSX.Element = ({
    user,
    csrfToken,
    forms,
    actions,
    contentPageId,
    contentTemplate,
    contentBlocks,
    themeId
}) => {

    const [blocks, setBlocks] = useState(contentBlocks);

    const isGroupAdmin: boolean = actions.includes(actionConstants.GROUPS_EDIT) || actions.includes(actionConstants.SITE_ADMIN_GROUPS_EDIT);

    const generatedClasses: any = {
        wrapper: classNames('c-page-body')
    };

    const handleSaveBlocks = (blocks: Array<CmsContentBlock>): Promise<FormErrors> => {

        return new Promise((resolve) => {

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
                        resolve({})
    
                    });

                });
        
            })
            .catch((error) => {

                const errors: FormErrors =
                getServiceErrorDataValidationErrors(error) ||
                getGenericFormError(error)

                resolve(errors)

            })

        }) 

    };
 
    useDynamicElementClassName({
        elementSelector: 'main',
        addClass: 'u-bg-theme-1',
        removeClass: 'u-bg-theme-3'
    });

    return (

        <LayoutColumn tablet={12} className={generatedClasses.wrapper}>
            {isGroupAdmin &&
                <NoScript headingLevel={2} text={{
                    heading: 'Important',
                    body: 'JavaScript must be enabled in your browser to manage the content of this page'
                }} />
            }
            <ContentBlockManager
                csrfToken={csrfToken}
                forms={forms}
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
                saveBlocksAction={handleSaveBlocks}
                themeId={themeId} />
        </LayoutColumn>

    )

}