import React from 'react'
import { UserProfile } from './index'
import { defaultGroupLogos } from '@constants/icons';

export default {
    title: 'User profile',
    component: UserProfile
}

const Template = (args) => <UserProfile {...args} />

export const Basic = Template.bind({})
Basic.args = {
    text: {
        heading: 'User Profile',
        firstNameLabel: 'First name',
        lastNameLabel: 'Last name',
        pronounsLabel: 'Preferred pronouns',
        emailLabel: 'Email'
    },
    profile: {
        firstName: 'Stephen',
        lastName: 'Stephenson',
        pronouns: 'he/him',
        email: 'stephen.stephenson@example.com'
    }
}

export const NamesOnly = Template.bind({})
NamesOnly.args = {
    text: {
        heading: 'User Profile',
        firstNameLabel: 'First name',
        lastNameLabel: 'Last name',
        pronounsLabel: 'Preferred pronouns',
        emailLabel: 'Email'
    },
    profile: {
        firstName: 'Stephen',
        lastName: 'Stephenson',
    }
}

export const Image = Template.bind({})
Image.args = {
    text: {
        heading: 'User Profile',
        firstNameLabel: 'First name',
        lastNameLabel: 'Last name',
        pronounsLabel: 'Preferred pronouns',
        emailLabel: 'Email'
    },
    profile: {
        firstName: 'Stephen',
        lastName: 'Stephenson',
        pronouns: 'he/him',
        email: 'stephen.stephenson@example.com'
    },
    image: defaultGroupLogos.large
}