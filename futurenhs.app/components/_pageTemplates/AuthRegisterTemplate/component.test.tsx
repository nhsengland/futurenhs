import { routes } from '@jestMocks/generic-props'
import { cleanup, render, screen } from '@jestMocks/index'
import forms from '@formConfigs/index'
import mockRouter from 'next-router-mock'
import { actions as actionConstants } from '@constants/actions'

import { SiteUserUpdateTemplate } from './index'

import { Props } from './interfaces'

describe('Site user update template', () => {
    beforeEach(() => {
        mockRouter.setCurrentUrl('/users/:userId')
    })

    const props: Props = {
        siteUser: {
            id: '1',
            firstName: 'Steven',
            lastName: 'Stevenson',
        },
        contentText: {
            firstNameLabel: 'First name',
            lastNameLabel: 'Surname',
            pronounsLabel: 'Pronouns',
            emailLabel: 'Email',
            editHeading: 'Edit user',
        },
        id: '123',
        routes: routes,
        forms: forms,
        actions: [],
    }

    it('renders correctly', () => {
        render(<SiteUserUpdateTemplate {...props} />)

        expect(screen.getAllByText('Edit user').length).toBe(1)
    })

    it('conditionally renders role update form', () => {
        render(<SiteUserUpdateTemplate {...props} />)

        expect(screen.queryByText('Update role')).toBeNull()

        cleanup()

        const propsCopy: Props = Object.assign({}, props, {
            actions: [actionConstants.SITE_ADMIN_MEMBERS_EDIT],
        })

        render(<SiteUserUpdateTemplate {...propsCopy} />)

        expect(screen.getAllByText('Update role').length).toBe(1)
    })
})
