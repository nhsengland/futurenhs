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
    ['0fca6809-a71f-4733-a622-343967acbad9']: {
        background: 14,
        content: 1,
        accent: 15
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
    },
    ['53bce171-d6a3-4721-8199-92f10fca5ef2']: {
        background: 10,
        content: 1,
        accent: 15
    },
    ['217a1f99-5b25-4e3b-be3d-29c55c46be05']: {
        background: 19,
        content: 0,
        accent: 20
    },
    ['a7101d5f-acce-4ef7-b1c9-5dbf20d54580']: {
        background: 15,
        content: 0,
        accent: 14
    },
    ['a9d8566d-162a-4fa3-b159-f604b12c214d']: {
        background: 6,
        content: 1,
        accent: 8
    }
}

/**
 * Default theme variant
 */
export const defaultThemeId: string = '0fca6809-a71f-4733-a622-343967acbad9';
