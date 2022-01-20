import * as React from 'react';
import { render, screen } from '@testing-library/react';

import { LoggedOutTemplate } from './index';

import { Props } from './interfaces';

const props: Props = {
    id: 'mockId',
    text: {
        title: 'mockTitle',
        metaDescription: 'mockMetaDescriptionText',
        mainHeading: 'mockMainHeading',
    },
    logOutUrl: '/mock'
};

describe('Logged out template', () => {

    it('renders correctly', () => {

        render(<LoggedOutTemplate {...props} />);

        expect(screen.getAllByText('mockMainHeading').length).toEqual(1);

    });
    
});
