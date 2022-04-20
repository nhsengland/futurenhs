import React from 'react'
import { Header } from './index'

export default {
    title: 'Header',
    component: Header,
    
};

const Template = (args) => <Header {...args}/>

export const Basic = Template.bind({});
Basic.args = {
    text: {
        admin: 'Admin',
        editProfile: 'Edit profile',
        logOut: 'Log out',
        logOutHeading: 'Log out',
        logOutBody: 'Do you want to log out?',
        logOutCancel: 'Cancel',
        logOutConfirm: 'Yes, log out',
    },
    user: {
        id: '12345',
        text: {
            userName: 'Members Name',
        },
        image: null,
    },
    navMenuList: [{
            isActive: false,
            url: '/',
            text: 'Groups',
        }],
    shouldRenderSearch: true,
    shouldRenderNavigation: true,
}
  