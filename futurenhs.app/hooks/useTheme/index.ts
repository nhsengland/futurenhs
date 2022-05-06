import { useContext } from 'react';

import { ThemesContext } from '@contexts/index';
import { selectTheme } from '@selectors/themes'
import { Theme } from '@appTypes/theme'

export const useTheme = (themeId: string): Theme => {

    const config: any = useContext(ThemesContext);
    const themes: any = config.themes;

    return selectTheme(themes, themeId);
}
