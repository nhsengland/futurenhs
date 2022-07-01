import { TabbedNav } from "./index";

export default {
    title: 'TabbedNav',
    component: TabbedNav
}

const Template = (args) => <TabbedNav {...args}/>

export const Basic = Template.bind({})
Basic.args = {
    text: 'Navigation',
    shouldFocusActiveLink: true,
    navMenuList: [
        {
            isActive: true,
            text: 'Home',
            url: '/'
        },
        {
            isActive: false,
            text: 'Forum',
            url: '/forum'
        },
        {
            isActive: false,
            text: 'Files',
            url: '/files'
        },
        {
            isActive: false,
            text: 'Members',
            url: '/members'
        }
    ]

}