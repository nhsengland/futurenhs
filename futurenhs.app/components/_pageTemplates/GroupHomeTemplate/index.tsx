import { useState, useEffect } from 'react';
import classNames from 'classnames';

import { actions as actionConstants } from '@constants/actions';
import { themes } from '@constants/themes';
import { selectTheme } from '@selectors/themes';
import { LayoutColumn } from '@components/LayoutColumn';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { Theme } from '@appTypes/theme';

import { Props } from './interfaces';

export const GroupHomeTemplate: (props: Props) => JSX.Element = ({
    actions,
    themeId
}) => {

    const [shouldRenderAdminCallOut, setShouldRenderAdminCallOut] = useState(false);

    const isGroupAdmin: boolean = actions.includes(actionConstants.GROUPS_EDIT) || actions.includes(actionConstants.SITE_ADMIN_GROUPS_EDIT);

    const { background, accent }: Theme = selectTheme(themes, themeId);

    const generatedClasses: any = {
        wrapper: classNames('c-page-body'),
        adminCallOut: classNames('nhsuk-inset-text u-m-0 u-max-w-full', `u-border-l-theme-${background}`),
        adminCallOutText: classNames('nhsuk-heading-m u-text-bold'),
        adminCallOutButton: classNames('c-button c-button-outline c-button--min-width u-w-full u-mt-4 tablet:u-mt-0')
    };

    useEffect(() => {

        isGroupAdmin && setShouldRenderAdminCallOut(true);

    }, []);

    return (

        <LayoutColumn tablet={12} className={generatedClasses.wrapper}>
            {shouldRenderAdminCallOut &&
                <LayoutColumnContainer>
                    <LayoutColumn tablet={9}>
                        <div className={generatedClasses.adminCallOut}>
                            <p className={generatedClasses.adminCallOutText}>You are a Group Admin of this page. Please click edit to switch to editing mode.</p>
                        </div>
                    </LayoutColumn>
                    <LayoutColumn tablet={3} className="u-flex u-items-center">
                        <button className={generatedClasses.adminCallOutButton}>Edit page</button>
                    </LayoutColumn>
                </LayoutColumnContainer>
            }
        </LayoutColumn>

    )

}