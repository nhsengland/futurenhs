import React from 'react'
import { useAssetPath } from '@hooks/useAssetPath';
import { SVGIcon } from './index'

const sessionStorageKey: string = 'sbSvgIconNames';
const iconsPath: string = '/icons/icons.svg';
const iconsSrc: string = useAssetPath(iconsPath);

let iconNames: Array<string> = [];

try {

    iconNames = window.sessionStorage.getItem(sessionStorageKey) ? JSON.parse(window.sessionStorage.getItem(sessionStorageKey)) : [];

    /**
     * Seemingly necessary workaround - no obvious support in Storybook to asyncronously fetch config to pass into argTypes
     * Fetches the svgSprite and parses out the list of icon names, then saves in sessionStorage and reloads the story
     */
    if(!iconNames.length){

        fetch(iconsSrc).then((response) => {

            if(response.ok){

                response.text().then((svg) => {

                    const matches = Array.from(svg.matchAll(/<symbol id=".*" p/g));

                    for (const match of matches) {
                        const parts = match[0].split('"');
                        iconNames.push(parts[1]);
                    }

                    if(iconNames.length){

                        window.sessionStorage.setItem(sessionStorageKey, JSON.stringify(iconNames));
                        window.location.reload();

                    }

                })

            }

        })

    }

} catch(error){

    console.log(error);

}

export default {
    title: 'SVGIcon',
    component: SVGIcon,
    argTypes: {
        name: {
            options: iconNames,
            control: { type: 'select' },
        },
    },
}

const Template = (args) => <SVGIcon  {...args} />


export const Basic = Template.bind({})
Basic.args = {
    name: iconNames?.[0] || '',
    className: 'u-w-[300px] u-h-[300px] u-fill-theme-0'
}

export const Themed = Template.bind({})
Themed.args = {
    name: iconNames?.[0] || '',
    className: 'u-w-[300px] u-h-[300px] u-fill-theme-8'
}
