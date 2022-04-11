import { render, screen } from '@testing-library/react'
import { SiteUserTemplate } from './index'
import { Props } from './interfaces'
import * as nextRouter from 'next/router'
import forms from '@formConfigs/index'
import { routes } from '@jestMocks/generic-props'


describe('Site User Template', () => {

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
        contentText: {} as any,
        actions: [],
        forms: forms,
        routes: routes
    }

    it('renders correctly', () => {
        render(<SiteUserTemplate { ...props }/>)

        expect(screen.getAllByText(props.siteUser.firstName).length).toBe(1)
        expect(screen.getAllByText(props.siteUser.lastName).length).toBe(1)
    })
})
