import { useState, useEffect } from 'react';
import { useRouter } from 'next/router';
import classNames from 'classnames';

import { useDynamicElementClassName } from '@hooks/useDynamicElementClassName';
import { actions as actionConstants } from '@constants/actions';
import { themes } from '@constants/themes';
import { crud } from '@constants/crud';
import { selectTheme } from '@selectors/themes';
import { ContentBlockManager } from '@components/ContentBlockManager';
import { RichText } from '@components/RichText';
import { NoScript } from '@components/NoScript';
import { LayoutColumn } from '@components/LayoutColumn';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { Theme } from '@appTypes/theme';

import { Props } from './interfaces';

export const GroupHomeTemplate: (props: Props) => JSX.Element = ({
    actions,
    contentBlocks,
    contentTemplate,
    themeId
}) => {

    const router = useRouter();

    const [mode, setMode] = useState(Boolean(router.query.edit) ? crud.UPDATE : crud.READ);
    const [isPreview, setIsPreview] = useState(false);
    const [isClient, setIsClient] = useState(false);

    const isGroupAdmin: boolean = actions.includes(actionConstants.GROUPS_EDIT) || actions.includes(actionConstants.SITE_ADMIN_GROUPS_EDIT);
    const { background }: Theme = selectTheme(themes, themeId);

    const generatedClasses: any = {
        wrapper: classNames('c-page-body'),
        adminCallOut: classNames('nhsuk-inset-text u-m-0 u-pr-0 u-max-w-full', `u-border-l-theme-${background}`),
        adminCallOutText: classNames('nhsuk-heading-m u-text-bold'),
        adminCallOutButton: classNames('c-button c-button-outline c-button--min-width u-w-full u-drop-shadow u-mt-4 tablet:u-mt-0'),
        previewButton: classNames('c-button c-button-outline c-button--min-width u-w-full u-mt-4 u-drop-shadow tablet:u-mt-0'),
        publishButton: classNames('c-button c-button--min-width u-w-full u-mt-4 tablet:u-mt-0 tablet:u-ml-5')
    };

    const handleSetToEditMode = (): void => {
        setIsPreview(false);
        setMode(crud.UPDATE);
    }

    const handleContentBlockManagerStateChange = (state: crud) => {
        setIsPreview(false);
        setMode(state)
    }

    const handleCreateBlock = (blockTypeId: string): void => {
        console.log(blockTypeId);
    }

    const handlePreviewUpdate = (): void => {
        setIsPreview(true);
        setMode(crud.READ);
    }

    const handlePublishUpdate = (): void => {
        setIsPreview(false);
        setMode(crud.READ);
    }

    const EditButton: () => JSX.Element = () => <button className={generatedClasses.adminCallOutButton} onClick={handleSetToEditMode}>Edit page</button>
    const PreviewButton: () => JSX.Element = () => <button className={generatedClasses.previewButton} onClick={handlePreviewUpdate}>Preview page</button>
    const PublishButton: () => JSX.Element = () => <button className={generatedClasses.publishButton} onClick={handlePublishUpdate}>Publish group page</button>

    useDynamicElementClassName({
        elementSelector: (mode === crud.CREATE || mode === crud.UPDATE) ? 'main' : null,
        addClass: 'u-bg-theme-1',
        removeClass: 'u-bg-theme-3'
    });

    useEffect(() => {

        setIsClient(true);

    }, []);

    return (

        <LayoutColumn tablet={12} className={generatedClasses.wrapper}>
            {isGroupAdmin &&
                <NoScript headingLevel={2} text={{
                    heading: 'Important',
                    body: 'JavaScript must be enabled in your browser to manage this page'
                }} />
            }
            {(isClient && isGroupAdmin && mode === crud.READ) &&
                <>

                    {isPreview

                        ? <LayoutColumnContainer>
                            <LayoutColumn tablet={6}>
                                <div className={generatedClasses.adminCallOut}>
                                    <p className={generatedClasses.adminCallOutText}>You are previewing the group homepage in editing mode.</p>
                                </div>
                            </LayoutColumn>
                            <LayoutColumn tablet={6} className="u-flex u-items-center">
                                <EditButton />
                                <PublishButton />
                            </LayoutColumn>
                        </LayoutColumnContainer>

                        : <LayoutColumnContainer>
                            <LayoutColumn tablet={9}>
                                <div className={generatedClasses.adminCallOut}>
                                    <p className={generatedClasses.adminCallOutText}>You are a Group Admin of this page. Please click edit to switch to editing mode.</p>
                                </div>
                            </LayoutColumn>
                            <LayoutColumn tablet={3} className="u-flex u-items-center">
                                <EditButton />
                            </LayoutColumn>
                        </LayoutColumnContainer>

                    }

                </>
            }
            {(isClient && isGroupAdmin && (mode === crud.UPDATE || mode === crud.CREATE)) &&
                <>
                    {mode === crud.CREATE

                        ? <>
                            <h2 className="nhsuk-heading-xl u-mb-8">Add content block</h2>
                            <RichText wrapperElementType="p" bodyHtml="Choose a content block to add to your group homepage" className="u-text-lead u-text-theme-7" />
                        </>

                        : <div className={generatedClasses.adminCallOut}>
                            <LayoutColumnContainer className="u-mb-6">
                                <LayoutColumn tablet={6}>
                                    <h2 className="nhsuk-heading-l u-m-0">Editing group homepage</h2>
                                </LayoutColumn>
                                <LayoutColumn tablet={6} className="tablet:u-flex u-items-center">
                                    <PreviewButton />
                                    <PublishButton />
                                </LayoutColumn>
                            </LayoutColumnContainer>
                            <RichText
                                wrapperElementType="div"
                                bodyHtml="Welcome to your group homepage. You are currently in editing mode. You can save a draft at any time, preview your page, or publish your changes. Once published, you can edit your page in the group actions. For more information and help, see our quick guide.
            For some inspiration, visit our knowledge hub."
                                className="u-text-lead u-text-theme-7" />
                        </div>

                    }
                </>
            }
            <ContentBlockManager
                blocks={contentBlocks}
                templateBlocks={contentTemplate}
                currentState={mode}
                stateChangeAction={handleContentBlockManagerStateChange}
                createBlockAction={handleCreateBlock}
                className="u-mt-14" />
        </LayoutColumn>

    )

}