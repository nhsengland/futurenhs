import * as React from 'react'
import { render, screen } from '@jestMocks/index'

import Page, { getServerSideProps } from './index.page'
import { routes } from '@jestMocks/generic-props'
import { layoutIds } from '@constants/routes'

import { Props } from '@components/_pageTemplates/GenericContentTemplate/interfaces'

const props: Props = {
    layoutId: layoutIds.BASE,
    id: 'mockId',
    routes: routes,
    user: undefined,
    contentText: {
        title: 'mockTitle',
        metaDescription: 'mockMetaDescriptionText',
        mainHeading: 'mockMainHeading',
    },
}

describe('Terms and conditions page', () => {
    it('renders correctly', () => {
        render(<Page {...props} />)

        expect(screen.getAllByText('mockMainHeading').length).toEqual(1)
    })

    it('gets required server side props', async () => {
        const serverSideProps = await getServerSideProps({
            req: {},
            res: {}
        } as any)

        expect(serverSideProps).toHaveProperty('props.contentText')
    })
})
