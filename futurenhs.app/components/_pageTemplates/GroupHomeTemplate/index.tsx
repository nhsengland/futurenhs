import { useState } from 'react';
import classNames from 'classnames';

import { getServiceErrorDataValidationErrors } from '@services/index';
import { getGenericFormError } from '@helpers/util/form';
import { useDynamicElementClassName } from '@hooks/useDynamicElementClassName';
import { actions as actionConstants } from '@constants/actions';
import { ContentBlockManager } from '@components/ContentBlockManager';
import { NoScript } from '@components/NoScript';
import { LayoutColumn } from '@components/LayoutColumn';
import { putCmsPageContent } from '@services/putCmsPageContent';

import { Props } from './interfaces';
import { CmsContentBlock } from '@appTypes/contentBlock';
import { FormErrors } from '@appTypes/form';

export const GroupHomeTemplate: (props: Props) => JSX.Element = ({
    user,
    csrfToken,
    forms,
    actions,
    contentTemplateId,
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
                pageId: contentTemplateId, 
                pageBlocks: blocks 
            })
            .then(() => resolve({}))
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
                    body: 'JavaScript must be enabled in your browser to manage this page'
                }} />
            }
            <ContentBlockManager
                csrfToken={csrfToken}
                forms={forms}
                blocks={blocks}
                blocksTemplate={contentTemplate}
                saveBlocksAction={handleSaveBlocks}
                themeId={themeId} />
        </LayoutColumn>

    )

}