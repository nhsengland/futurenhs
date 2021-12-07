import * as React from 'react';
import { render, screen } from '@testing-library/react';

import Page from './index.page';
import { Props } from './interfaces';

describe('Groups page', () => {

    const props: Props = {
        user: undefined,
        content: {
            titleText: 'Mock title text',
            metaDescriptionText: 'Mock meta description text',
            mainHeadingHtml: 'Mock main heading html'
        },
        groupsList: {
            data: [
                {
                    content: {
                        mainHeadingHtml: 'Mock Group card heading 1'
                    },
                    slug: 'mock-group',
                    totalDiscussionCount: 3,
                    totalMemberCount: 4
                }
            ]
        }
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
