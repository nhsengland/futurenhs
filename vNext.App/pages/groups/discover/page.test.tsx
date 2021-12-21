import * as React from 'react';
import * as nextRouter from 'next/router';
import { render, screen } from '@testing-library/react';

import Page from './index.page';
import { Props } from '@components/_pageTemplates/GroupListingTemplate/interfaces';

describe('Discover groups page', () => {

    (nextRouter as any).useRouter = jest.fn();
    (nextRouter as any).useRouter.mockImplementation(() => ({ route: '/groups/discover' }));

    const props: Props = {
        id: 'mockPageId',
        user: undefined,
        content: {
            titleText: 'Mock title text',
            metaDescriptionText: 'Mock meta description text',
            mainHeadingHtml: 'Mock main heading html',
            introHtml: '',
            secondaryHeadingHtml: '',
            navMenuTitleText: ''
        },
        groupsList: [
            {
                content: {
                    metaDescriptionText: 'Mock meta description text',
                    titleText: 'Mock title text',
                    mainHeadingHtml: 'Mock Group card heading 1'
                },
                slug: 'mock-group',
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
