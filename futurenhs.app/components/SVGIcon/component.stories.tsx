import React from 'react'
import { SVGIcon } from './index'

const iconsSrc: string = '/icons/icons.svg';
const iconNames: Array<string> = window.sessionStorage.getItem('sbSvgIconNames') ?  JSON.parse(window.sessionStorage.getItem('sbSvgIconNames')) : [];

/**
 * Seemingly necessary workaround - no obvious support in Storybook to asyncronously fetch config to pass into argTypes
 * Fetches the svgSprite and parses out the list of icon names, then saves in sessionStorage and reloads the story
 */
if(!iconNames.length){

    fetch(iconsSrc).then((response) => {

        response.text().then((svg) => {
    
            const matches = Array.from(svg.matchAll(/<symbol id=".*" p/g));
    
            for (const match of matches) {
                const parts = match[0].split('"');
                iconNames.push(parts[1]);
            }
    
            window.sessionStorage.setItem('sbSvgIconNames', JSON.stringify(iconNames));
            window.location.reload();

        })
    
    })

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

const Template = (args) => <SVGIcon {...args} />

export const Basic = Template.bind({})
Basic.args = {
    url: iconsSrc,
    name: iconNames?.[0] || '',
    className: 'u-w-[200px] u-h-[200px] u-fill-theme-0'
}

export const Themed = Template.bind({})
Themed.args = {
    url: iconsSrc,
    name: iconNames?.[0] || '',
    className: 'u-w-[200px] u-h-[200px] u-fill-theme-8'
}
