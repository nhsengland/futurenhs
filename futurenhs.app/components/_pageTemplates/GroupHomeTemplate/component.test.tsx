import React from 'react'
import { render } from '@jestMocks/index'
import mockRouter from 'next-router-mock';

import { GroupHomeTemplate } from './index'
import { routes } from '@jestMocks/generic-props'

import { Props } from './interfaces'

const props: Props = {
    id: 'mockId',
    routes: routes,
    tabId: 'index',
    actions: [],
    contentText: {},
    user: undefined,
    entityText: {
        mainHeading: 'Mock heading',
    },
    image: {
        src: 'https://www.google.com',
        height: 100,
        width: 100,
        altText: 'Mock alt text',
    },
}

jest.mock('next/router', () => require('next-router-mock'));

describe('Group home template', () => {

    beforeEach(() => {
        mockRouter.setCurrentUrl('/groups/group');
    });

    it('renders correctly', () => {
        render(<GroupHomeTemplate {...props} />)

        //TODO: template WIP
    })
})
