import { themes } from '@constants/themes'
import { selectTheme } from '@selectors/themes'
import { Theme } from '@appTypes/theme'

export const useTheme = (themeId: string): Theme => {
    return selectTheme(themes, themeId);
}
