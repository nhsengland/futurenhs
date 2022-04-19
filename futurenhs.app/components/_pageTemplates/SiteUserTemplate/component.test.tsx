import { render, screen, cleanup } from '@testing-library/react'
import { SiteUserTemplate } from './index'
import { Props } from './interfaces'
import * as nextRouter from 'next/router'
import forms from '@formConfigs/index'
import { routes } from '@jestMocks/generic-props'
import { actions } from '@constants/actions'


describe('Site User Template', () => {
    
    afterEach(cleanup)
    
    ;(nextRouter as any).useRouter = jest.fn()
    ;(nextRouter as any).useRouter.mockImplementation(() => ({
        asPath: '/users/userId',
    }))

    const props: Props = {
        id: '',
        user: {
            id: 'userId',
            text: {
                userName: 'username'
            }
        },
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
            emailLabel: 'Email'
        },
        actions: [],
        forms: forms,
        routes: routes
    }

    it('renders correctly', () => {
        render(<SiteUserTemplate { ...props }/>)

        expect(screen.getAllByText(props.siteUser.firstName).length).toBe(1)
        expect(screen.getAllByText(props.siteUser.lastName).length).toBe(1)
    })

    it('conditionally renders edit profile button', () => {

        render(<SiteUserTemplate { ...props }/>);

        expect(screen.queryByText('Edit button')).toBeNull();

        const propsCopy: Props = Object.assign({}, props, {
            actions: [
                actions.SITE_ADMIN_MEMBERS_EDIT
            ]
        })

        render(<SiteUserTemplate { ...propsCopy }/>);

        expect(screen.getAllByText('Edit button').length).toBe(1);

    })

    it('conditionally renders edit user form', () => {

        render(<SiteUserTemplate { ...props }/>);

        expect(screen.queryByText('Edit profile')).toBeNull();

        ;(nextRouter as any).useRouter.mockImplementation(() => ({
            asPath: '/users/userId',
            query: {
                edit: true
            }
        }))

        const propsCopy: Props = Object.assign({}, props, {
            actions: [
                actions.SITE_ADMIN_MEMBERS_EDIT
            ]
        })

        render(<SiteUserTemplate { ...propsCopy }/>);

        expect(screen.getAllByText('Edit profile').length).toBe(1)
    })
})
