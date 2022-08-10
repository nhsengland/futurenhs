import { addons } from '@storybook/addons'
import customTheme from './theme'

addons.setConfig({
    theme: customTheme,
})


const iconsPath = '/icons/icons.svg';
const basePath = process.env.NEXT_PUBLIC_ASSET_PREFIX || process.env.STORYBOOK_PUBLIC_ASSET_PREFIX || '';

const iconsSrc = basePath ? `${basePath}${iconsPath}` : iconsPath;

/**
 * Fetches list of svg icons and sets to window to provide access to all components/stories
 */
(async () => {

    const iconNames = []

    await fetch(iconsSrc).then((response) => {

        if (response.ok) {

            response.text().then((svg) => {

                const matches = Array.from(svg.matchAll(/<symbol id=".*" p/g));

                for (const match of matches) {
                    const parts = match[0].split('"');
                    iconNames.push(parts[1]);
                }

            })
        }
    })

    window.sbIconList = iconNames

})()