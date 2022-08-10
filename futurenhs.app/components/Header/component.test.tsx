import { render, screen } from '@jestMocks/index'

import { Header } from './index'

import { Props } from './interfaces'

const testProps: Props = {
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
            userName: 'Mock Name',
        },
        image: {
            src: '/img.jpg',
            altText: 'Image of Mock Name',
            height: 100,
            width: 100
        },
    },
    navMenuList: [],
    shouldRenderSearch: true,
    shouldRenderNavigation: true,
}

describe('Header', () => {
    it('Renders a search input', () => {
        const props = Object.assign({}, testProps)

        render(<Header {...props} />)

        expect(screen.getByRole('searchbox'))
    })

    it('Renders a user nav', () => {
        const props = Object.assign({}, testProps)

        render(<Header {...props} />)

        expect(screen.getByRole('navigation'))
    })

    it('Renders content', () => {
        const props = Object.assign({}, testProps)

        render(<Header {...props} />)

        expect(screen.getByText('Log out'))
    })
})
