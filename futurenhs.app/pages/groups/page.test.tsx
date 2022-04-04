import * as React from 'react';
import * as nextRouter from 'next/router';
import { fireEvent, render, screen } from '@testing-library/react';
import { routes } from '@jestMocks/generic-props';

import GroupListingTemplate, { getServerSideProps } from './index.page';
import { layoutIds } from '@constants/routes';
import { Props } from '@components/_pageTemplates/GroupListingTemplate/interfaces';

import { mswServer } from '../../jest-mocks/msw-server';
import { handlers } from '../../jest-mocks/handlers';
import { actions } from '@constants/actions';

const props: Props = {
    id: 'mockId',
    layoutId: layoutIds.BASE,
    user: { id: 'fake-admin-id', text: { userName: "Mock User Jest" } },
    contentText: {
        title: 'mockTitle',
        metaDescription: 'mockMetaDescriptionText',
        mainHeading: 'mockMainHeading',
    },
    groupsList: [{
        text: {
            mainHeading: "Mock super group",
        },
        themeId: '12',
        totalDiscussionCount: 11,
        totalMemberCount: 60,
        groupId: 'super-group'
    }],
    isGroupMember: true,
    routes: routes,
    actions: [
        actions.GROUPS_EDIT,
        actions.SITE_ADMIN_VIEW,
        actions.SITE_MEMBERS_ADD,
        actions.GROUPS_MEMBERS_EDIT,
        actions.GROUPS_MEMBERS_DELETE,
        actions.GROUPS_MEMBERS_ADD,
        actions.SITE_ADMIN_MEMBERS_DELETE,
    ]
};

beforeAll(() => mswServer.listen());
afterEach(() => mswServer.resetHandlers());
afterAll(() => mswServer.close());

describe('groups page', () => {

    const push = jest.fn();

    (nextRouter as any).useRouter = jest.fn();
    (nextRouter as any).useRouter.mockImplementation(() => ({
        asPath: '/groups',
        query: {},
        pathname: '/groups',
        push
    }));


    it('renders correctly', () => {

        render(<GroupListingTemplate {...props} />);

        expect(screen.getAllByText(props.groupsList[0].text.mainHeading).length).toEqual(1);

    });

    it('navigates to discover groups page correctly', () => {

        render(<GroupListingTemplate {...props} />);

        fireEvent.click(screen.getByText('Discover new groups'));

        expect(push).toHaveBeenCalledWith('/groups/discover', expect.anything(), expect.anything());

    });

    it('activates group link', () => {

        render(<GroupListingTemplate {...props} />);

        fireEvent.click(screen.getByText(props.groupsList[0].text.mainHeading));

        expect(push).toHaveBeenCalledWith(`/groups/${props.groupsList[0].groupId}`, expect.anything(), expect.anything());
    
    });


    it('gets required server side props', async () => {

        const serverSideProps = await getServerSideProps({ req: { cookies: "ew5tt4t3e" } } as any);

        expect(serverSideProps).toHaveProperty('props.contentText');
        expect(serverSideProps).toHaveProperty('props.pagination');
        expect(serverSideProps).not.toHaveProperty('props.errors');
        expect(serverSideProps["notFound"]).toBeFalsy();

    });

    it('returns notFound if user does not have admin view permission', async () => {
       
        mswServer.use(handlers.getGroups({ status: 404 }));

        const serverSideProps = await getServerSideProps({ req: { cookies: "ew5tt4t3e" } } as any);

        expect(serverSideProps["notFound"]).toBe(true);

    });

});