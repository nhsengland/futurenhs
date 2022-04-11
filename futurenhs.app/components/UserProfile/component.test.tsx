import { render, screen } from '@testing-library/react'

import { UserProfile } from './index'

import { Props } from './interfaces'

const testProps: Props = {
    profile: {
        firstName: 'Mock',
        lastName: 'Mock',
        pronouns: 'Mock',
        email: 'Mock',
    },
    text: {
        heading: 'Mock heading text',
        firstNameLabel: 'Mock first name text',
        lastNameLabel: 'Mock last name text',
        pronounsLabel: 'Mock pronouns text',
        emailLabel: 'Mock email text',
    },
}

describe('User Profile', () => {
    it('renders passed in content', () => {
        const props = Object.assign({}, testProps)

        render(<UserProfile {...props} />)

        expect(screen.getByText('Mock heading text'))
        expect(screen.getByText('Mock first name text'))
        expect(screen.getByText('Mock last name text'))
        expect(screen.getByText('Mock pronouns text'))
        expect(screen.getByText('Mock email text'))
    })
})
