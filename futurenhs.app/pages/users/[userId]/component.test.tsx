import { render, screen, cleanup } from '@jestMocks/index'
import UserPage, { Props } from '@pages/users/[userId]/index.page'
import mockRouter from 'next-router-mock'
import forms from '@config/form-configs/index'
import { routes } from '@jestMocks/generic-props'
import { actions } from '@constants/actions'
import { mockUser } from '@helpers/hofs/withUser/hof.test'

jest.mock('next/router', () => require('next-router-mock'))

describe('Site User Template', () => {
    beforeEach(() => {
        mockRouter.setCurrentUrl('/users/userId')
    })

    afterEach(cleanup)

    const props: Props = {
        id: '',
        user: mockUser,
        siteUser: {
            id: 'string',
            role: 'string',
            firstName: 'MockFirstName',
            lastName: 'MockLastName',
        },
        contentText: {
            editHeading: 'Edit profile',
            editButtonLabel: 'Edit button',
            firstNameLabel: 'First name',
            lastNameLabel: 'Last name',
            pronounsLabel: 'he/him',
            emailLabel: 'Email',
        },
        actions: [],
        forms: forms,
        routes: routes,
    }

    it('renders correctly', () => {
        render(<UserPage {...props} />)

        expect(screen.getAllByText(props.siteUser.firstName).length).toBe(1)
        expect(screen.getAllByText(props.siteUser.lastName).length).toBe(1)
    })

    it('conditionally renders edit profile button', () => {
        render(<UserPage {...props} />)

        expect(screen.queryByText('Edit button')).toBeNull()

        const propsCopy: Props = Object.assign({}, props, {
            actions: [actions.SITE_ADMIN_MEMBERS_EDIT],
        })

        render(<UserPage {...propsCopy} />)

        expect(screen.getAllByText('Edit button').length).toBe(1)
    })
})
