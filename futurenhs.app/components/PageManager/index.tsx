import { useState, useEffect } from 'react';
import { useRouter } from 'next/router';
import classNames from 'classnames';

import { useDynamicElementClassName } from '@hooks/useDynamicElementClassName';
import { actions as actionConstants } from '@constants/actions';
import { themes } from '@constants/themes';
import { selectTheme } from '@selectors/themes';
import { RichText } from '@components/RichText';
import { SVGIcon } from '@components/SVGIcon';
import { LayoutColumn } from '@components/LayoutColumn';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { Theme } from '@appTypes/theme';

import { Props } from './interfaces';

export const PageManager: (props: Props) => JSX.Element = ({

}) => {

    const generatedClasses: any = {
        block: classNames('c-page-manager-block', 'u-text-center')
    };

    return (

        <div className={generatedClasses.block}>
            <button className="c-button c-button-outline u-drop-shadow">
                <SVGIcon name="icon-add-content" className="u-w-9 u-h-8 u-mr-4 u-align-middle" />
                <span className="u-align-middle">Add content block</span>
            </button>
        </div>

    )

}