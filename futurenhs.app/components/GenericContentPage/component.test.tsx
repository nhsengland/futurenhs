import * as React from 'react';
import { render, screen } from '@testing-library/react';

import { GenericContentPage } from './index';

import { Props } from './interfaces';

const props: Props = {
    isAuthenticated: true,
    text: {
        title: 'mockTitle',
        metaDescription: 'mockMetaDescriptionText',
        mainHeading: 'mockMainHeading',
    }
};

describe('Generic content page', () => {

    it('renders correctly', () => {

        render(<GenericContentPage {...props} />);

        expect(screen.getAllByText('mockMainHeading').length).toEqual(1);

    });
    
});
