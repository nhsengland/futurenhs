import { useState } from 'react';
import classNames from 'classnames';

import { useDynamicElementClassName } from '@hooks/useDynamicElementClassName';
import { actions as actionConstants } from '@constants/actions';
import { ContentBlockManager } from '@components/ContentBlockManager';
import { NoScript } from '@components/NoScript';
import { LayoutColumn } from '@components/LayoutColumn';

import { Props } from './interfaces';
import { CmsContentBlock } from '@appTypes/contentBlock';

export const GroupHomeTemplate: (props: Props) => JSX.Element = ({
    csrfToken,
    forms,
    actions,
    contentBlocks,
    contentTemplate,
    themeId
}) => {

    const [blocks, setBlocks] = useState(contentBlocks);

    const isGroupAdmin: boolean = actions.includes(actionConstants.GROUPS_EDIT) || actions.includes(actionConstants.SITE_ADMIN_GROUPS_EDIT);

    const generatedClasses: any = {
        wrapper: classNames('c-page-body')
    };

    const handleBlocksChange = (blocks: Array<CmsContentBlock>): void => {

        // TODO save blocks to API

        //setBlocks(blocks);

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
                blocksChangeAction={handleBlocksChange}
                themeId={themeId} />
        </LayoutColumn>

    )

}