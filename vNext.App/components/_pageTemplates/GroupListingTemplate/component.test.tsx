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

        render(<GroupListingTemplate {...props} />);

        expect(screen.getAllByText('Mock main heading html').length).toEqual(1);

    });

    it('renders a group list', () => {

        render(<GroupListingTemplate {...props} />);

        expect(screen.getAllByText('Mock Group card heading 1').length).toEqual(1);

    });
    
});
