import * as React from 'react';
import * as nextRouter from 'next/router';
import { render, screen } from '@testing-library/react';

import { GroupListingTemplate } from './index';
import { Props } from './interfaces';

describe('GroupListingTemplate', () => {

    (nextRouter as any).useRouter = jest.fn();
    (nextRouter as any).useRouter.mockImplementation(() => ({ 
        asPath: '/groups' 
    }));

    const props: Props = {
        id: 'mockPageId',
        user: undefined,
        contentText: {
            title: 'Mock title text',
            metaDescription: 'Mock meta description text',
            mainHeading: 'Mock main heading html',
            intro: 'Mock intro html',
            navMenuTitle: 'Mock nav menu title text',
            secondaryHeading: 'Mock secondary heading html'
        },
        isGroupMember: true,
        groupsList: [
            {
                text: {
                    metaDescription: 'Mock meta description text',
                    title: 'Mock title text',
                    mainHeading: 'Mock Group card heading 1'
                },
                groupId: 'mock-group',
                totalDiscussionCount: 3,
                totalMemberCount: 4
            }
        ]
    };

    it('renders correctly', () => {

        render(<GroupListingTemplate {...props} />);

        expect(screen.getAllByText('Mock main heading html').length).toEqual(1);

    });

    it('renders a group list', () => {

        render(<GroupListingTemplate {...props} />);

        expect(screen.getAllByText('Mock Group card heading 1').length).toEqual(1);

    });
    
});
