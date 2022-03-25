import { defaultThemeId } from "@constants/themes";
import { Theme } from "@appTypes/theme";

export const selectTheme = (themes: Record<string, Theme>, themeId: string): Theme => themes[themeId] ?? themes[defaultThemeId];
export const selectThemeBackgroundId = (themes: Record<string, Theme>, themeId: string): number => selectTheme(themes, themeId)?.background;
export const selectThemeContentId = (themes: Record<string, Theme>, themeId: string): number => selectTheme(themes, themeId)?.content;
export const selectThemeAccentId = (themes: Record<string, Theme>, themeId: string): number => selectTheme(themes, themeId)?.accent;
