import * as React from 'react'
import * as nextRouter from 'next/router'
import { render, screen } from '@testing-library/react'
import { routes } from '@jestMocks/generic-props'

import GroupFileDetailTemplate, { getServerSideProps } from './index.page'
import { Props } from '@components/_pageTemplates/GroupFileDetailTemplate/interfaces'

import { mswServer } from '../../../../../../jest-mocks/msw-server'
import { handlers } from '../../../../../../jest-mocks/handlers'

const props: Props = {
    id: 'mockId',
    user: { id: 'fake-admin-id', text: { userName: 'Mock User Jest' } },
    contentText: {
        title: 'mockTitle',
        metaDescription: 'mockMetaDescriptionText',
        mainHeading: 'mockMainHeading',
    },
    tabId: 'files',
    image: null,
    entityText: 'Mock test file',
    file: {
        id: 'file',
        type: 'file',
        name: 'mock test file',
        modifiedBy: {
            id: 'mock user',
            text: {
                userName: 'Mock user',
            },
        },
        modified: '2022-12-11',
    },
    fileId: 'mock-test-file',
    routes: routes,
}

beforeAll(() => mswServer.listen())
afterEach(() => mswServer.resetHandlers())
afterAll(() => mswServer.close())

describe('file detail page', () => {
    ;(nextRouter as any).useRouter = jest.fn()
    ;(nextRouter as any).useRouter.mockImplementation(() => ({
        asPath: '/files',
        query: { fileId: 'fileId' },
    }))

    it('renders correctly', () => {
        render(<GroupFileDetailTemplate {...props} />)

        expect(screen.getAllByText(props.file.name).length).not.toEqual(0)
        expect(
            screen.getAllByText(props.file.modifiedBy.text.userName).length
        ).toEqual(1)
    })

    it('gets required server side props', async () => {
        const serverSideProps = await getServerSideProps({
            req: { cookies: 'fake-cookie-101' },
        } as any)

        expect(serverSideProps).toHaveProperty('props.contentText')
        expect(serverSideProps).toHaveProperty('props.entityText')
        expect(serverSideProps).not.toHaveProperty('props.errors')
        expect(serverSideProps['notFound']).toBeFalsy()
    })

    it('returns notFound if user does not have admin view permission', async () => {
        mswServer.use(handlers.getGroup({ status: 404 }))

        const serverSideProps = await getServerSideProps({
            req: { cookies: 'fake-cookie-101' },
        } as any)

        expect(serverSideProps['notFound']).toBe(true)
    })
})
