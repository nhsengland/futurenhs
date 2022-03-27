import { useState, useEffect } from 'react';
import { useRouter } from 'next/router';
import classNames from 'classnames';

import { useDynamicElementClassName } from '@hooks/useDynamicElementClassName';
import { actions as actionConstants } from '@constants/actions';
import { themes } from '@constants/themes';
import { selectTheme } from '@selectors/themes';
import { RichText } from '@components/RichText';
import { LayoutColumn } from '@components/LayoutColumn';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { Theme } from '@appTypes/theme';

import { Props } from './interfaces';

export const PageManager: (props: Props) => JSX.Element = ({

}) => {

    const generatedClasses: any = {

    };

    return (

        <div className="u-py-10 u-px-8 u-bg-theme-3 u-text-center">
            <button className="c-button c-button-outline">Add content block</button>
        </div>

    )

}