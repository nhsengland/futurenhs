export const useAssetPath = (path: string): string => {

    const basePath: string = process.env.NEXT_PUBLIC_ASSET_PREFIX || process.env.STORYBOOK_PUBLIC_ASSET_PREFIX || '';

    return basePath ? `${basePath}${path}` : path;
}
