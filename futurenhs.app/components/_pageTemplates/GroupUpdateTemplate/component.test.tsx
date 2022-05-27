import React from 'react'
import mockRouter from 'next-router-mock'
import { render, screen } from '@jestMocks/index'

import { GroupUpdateTemplate } from './index'
import { routes } from '@jestMocks/generic-props'
import forms from '@formConfigs/index'
import { Props } from './interfaces'

jest.mock('next/router', () => require('next-router-mock'))

describe('Group update template', () => {
    beforeEach(() => {
        mockRouter.setCurrentUrl('/groups/group/update')
    })

    const props: Props = {
        id: 'mockId',
        routes: routes,
        tabId: 'index',
        folderId: 'mockId',
        user: undefined,
        actions: [],
        forms: forms,
        contentText: {
            mainHeading: 'Mock main heading',
        },
        entityText: {
            title: 'Mock title text',
            metaDescription: 'Mock meta description text',
            mainHeading: 'Mock main heading html',
            intro: 'Mock intro html',
            navMenuTitle: 'Mock nav menu title text',
            secondaryHeading: 'Mock secondary heading html',
        },
        image: null,
    }

    it('renders correctly', () => {
        render(<GroupUpdateTemplate {...props} />)

        expect(screen.getAllByText('Mock main heading').length).toEqual(1)
    })
})
