import * as React from 'react';
import { render, screen } from '@testing-library/react';
import * as nextRouter from 'next/router';

import Page from './index.page';
import { Props } from '@components/_pageTemplates/GroupListingTemplate/interfaces';

describe('Groups page', () => {

    (nextRouter as any).useRouter = jest.fn();
    (nextRouter as any).useRouter.mockImplementation(() => ({ route: '/groups' }));

    const props: Props = {
        id: 'mockPageId',
        user: undefined,
        content: {
            titleText: 'Mock title text',
            metaDescriptionText: 'Mock meta description text',
            mainHeadingHtml: 'Mock main heading html',
            introHtml: 'Mock intro html',
            navMenuTitleText: 'Mock nav menu title text',
            secondaryHeadingHtml: 'Mock secondary heading html'
        },
        groupsList: [
            {
                content: {
                    metaDescriptionText: 'Mock meta description test',
                    titleText: 'Mock title text',
                    mainHeadingHtml: 'Mock Group card heading 1'
                },
                groupId: 'mock-group',
                totalDiscussionCount: 3,
                totalMemberCount: 4
            }
        ]
    };

    it('renders correctly', () => {

        render(<Page {...props} />);

        expect(screen.getAllByText('Mock main heading html').length).toEqual(1);

    });

    it('renders a group list', () => {

        render(<Page {...props} />);

        expect(screen.getAllByText('Mock Group card heading 1').length).toEqual(1);

    });
    
});
