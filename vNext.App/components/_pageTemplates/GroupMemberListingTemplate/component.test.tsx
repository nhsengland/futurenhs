import * as React from 'react';
import * as nextRouter from 'next/router';
import { render, screen } from '@testing-library/react';

import { GroupMemberListingTemplate } from './index';
import { Props } from './interfaces';

describe('Group member listing template', () => {

    (nextRouter as any).useRouter = jest.fn();
    (nextRouter as any).useRouter.mockImplementation(() => ({ 
        asPath: '/groups/group/members',
        query: {
            groupId: 'group'
        } 
    }));

    const props: Props = {
        id: 'mockPageId',
        user: undefined,
        actions: [],
        contentText: null,
        entityText: {
            title: 'Mock title text',
            metaDescription: 'Mock meta description text',
            mainHeading: 'Mock main heading html',
            intro: 'Mock intro html',
            navMenuTitle: 'Mock nav menu title text',
            secondaryHeading: 'Mock secondary heading html',
            membersHeading: 'Mock members heading',
            pendingMemberRequestsHeading: 'Mock pending members heading'
        },
        image: null,
        pendingMembers: [],
        members: []
    };

    it('renders correctly', () => {

        render(<GroupMemberListingTemplate {...props} />);

        expect(screen.getAllByText('Mock main heading html').length).toEqual(1);

    });
    
});
