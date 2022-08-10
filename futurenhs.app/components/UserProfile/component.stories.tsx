import React from 'react'
import { useAssetPath } from '@hooks/useAssetPath';
import { UserProfile } from './index'

const exampleImageSrc: string = useAssetPath('/images/example-profile-image.svg');

export default {
    title: 'UserProfile',
    component: UserProfile,
    argTypes: {
        image: {
            control: { type: '' },
        },
    },
}

const Template = (args) => <UserProfile {...args} />

export const Basic = Template.bind({})
Basic.args = {
    text: {
        heading: 'User Profile',
        firstNameLabel: 'First name',
        lastNameLabel: 'Last name',
        pronounsLabel: 'Preferred pronouns',
        emailLabel: 'Email',
    },
    profile: {
        firstName: 'Anne',
        lastName: 'Example',
        pronouns: 'she/her',
        email: 'anne.example@example.com',
    },
    image: {
        src: exampleImageSrc,
        altText: 'Image of user',
        height: '250px',
        width: '250px'
    }
}

export const NamesOnly = Template.bind({})
NamesOnly.args = {
    text: {
        heading: 'User Profile',
        firstNameLabel: 'First name',
        lastNameLabel: 'Last name',
        pronounsLabel: 'Preferred pronouns',
        emailLabel: 'Email',
    },
    profile: {
        firstName: 'Anne',
        lastName: 'Example',
    },
    image: {
        src: exampleImageSrc,
        altText: 'Image of user',
        height: '250px',
        width: '250px'
    }
}

export const NoImage = Template.bind({})
NoImage.args = {
    text: {
        heading: 'User Profile',
        firstNameLabel: 'First name',
        lastNameLabel: 'Last name',
        pronounsLabel: 'Preferred pronouns',
        emailLabel: 'Email',
    },
    profile: {
        firstName: 'Anne',
        lastName: 'Example',
        pronouns: 'she/her',
        email: 'anne.example@example.com',
    },
    image: null 
}
