import * as React from 'react';
import * as nextRouter from 'next/router';
import { render, screen } from '@testing-library/react';

import { GroupMemberListingTemplate } from './index';
import { Props } from './interfaces';

describe('Group member listing template', () => {

    (nextRouter as any).useRouter = jest.fn();
    (nextRouter as any).useRouter.mockImplementation(() => ({ 
        asPath: '/groups/group/members' 
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
        image: null
    };

    it('renders correctly', () => {

        render(<GroupMemberListingTemplate {...props} />);

        expect(screen.getAllByText('Mock main heading html').length).toEqual(1);

    });
    
});
