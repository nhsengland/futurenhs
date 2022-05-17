import { MainNav } from './index'

export default {
    title: 'MainNav',
    component: MainNav
}

const Template = (args) => <MainNav {...args} />

const mainNavMenuList = [
    {
        url: '/',
        text: 'Home',
        isActive: true,
        isActiveRoot: false,
        meta: {
            themeId: 8,
            iconName: 'icon-home'
        }
    },
    {
        url: '/',
        text: 'Groups',
        isActive: false,
        isActiveRoot: false,
        meta: {
            themeId: 14,
            iconName: 'icon-groups',
        },
    },
]

export const Basic = Template.bind({})
Basic.args = {
    navMenuList: mainNavMenuList
}