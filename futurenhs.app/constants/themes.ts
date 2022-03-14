import { Theme } from '@appTypes/theme';

/**
 * Allowed theme variants
 */
export const themes: Record<string, Theme> = {
    ['36d49305-eca8-4176-bfea-d25af21469b9']: {
        background: 8,
        content: 1,
        accent: 14
    },
    ['9a3c911b-c3d3-4f58-a32a-d541e0f5bf56']: {
        background: 11,
        content: 1,
        accent: 8
    },
    ['5053a8c6-ea4d-4125-9dc3-475e3e931fee']: {
        background: 12,
        content: 1,
        accent: 4
    }
}

/**
 * Default theme variant
 */
export const defaultThemeId: string = '36d49305-eca8-4176-bfea-d25af21469b9';
